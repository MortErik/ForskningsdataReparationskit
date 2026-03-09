using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ForskningsdataReparationskit.Models;

namespace ForskningsdataReparationskit
{
    /// <summary>
    /// XML FK Repair implementation
    /// Parsing direkte fra XML + optional tableIndex metadata
    /// </summary>
    public class ScalableXmlFKRepair
    {
        private const int CHUNK_SIZE = 1_000_000;
        private const int PARALLEL_DEGREE = 6;
        private const int BUFFER_SIZE = 8 * 1024 * 1024;

        /// <summary>
        /// Memory-efficient string storage
        /// </summary>
        private class FastStringSet
        {
            private readonly ConcurrentDictionary<string, byte> _set;

            public FastStringSet(int capacity = 10_000_000)
            {
                _set = new ConcurrentDictionary<string, byte>(
                    concurrencyLevel: Environment.ProcessorCount,
                    capacity: capacity);
            }

            public bool Add(string key) => _set.TryAdd(key, 0);
            public bool Contains(string key) => _set.ContainsKey(key);
            public int Count => _set.Count;
            public void Clear() => _set.Clear();
        }

        /// <summary>
        /// Extracts key values from XML using string-based parsing
        /// </summary>
        public class XmlKeyExtractor
        {
            private readonly string[] _keyColumns;

            public XmlKeyExtractor(List<string> keyColumns)
            {
                _keyColumns = keyColumns.ToArray();
            }

            public string ExtractKey(string rowContent)
            {
                var keyParts = new string[_keyColumns.Length];

                for (int i = 0; i < _keyColumns.Length; i++)
                {
                    string columnName = _keyColumns[i];
                    string startTag = $"<{columnName}>";
                    string endTag = $"</{columnName}>";

                    int startIndex = rowContent.IndexOf(startTag);
                    if (startIndex == -1) continue;

                    startIndex += startTag.Length;
                    int endIndex = rowContent.IndexOf(endTag, startIndex);
                    if (endIndex == -1) continue;

                    keyParts[i] = rowContent.Substring(startIndex, endIndex - startIndex).Trim();
                }

                return string.Join("|", keyParts);
            }
        }

        /// <summary>
        /// Analyze missing FK values
        /// </summary>
        public async Task<List<string>> AnalyzeMissingKeysAsync(
            string parentXmlPath, string childXmlPath,
            List<string> parentKeyColumns, List<string> childKeyColumns,
            IProgress<(long processed, string stage)> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var parentKeys = new FastStringSet();
            var missingKeys = new List<string>();

            // STEP 1: Load parent keys
            progress?.Report((0, "Loading parent keys"));
            await Task.Run(() => LoadKeysFromXml(parentXmlPath, parentKeys, parentKeyColumns, progress, cancellationToken));

            progress?.Report((parentKeys.Count, "Parent keys loaded"));

            // STEP 2: Find missing keys in child
            progress?.Report((0, "Analyzing child keys"));
            await Task.Run(() => AnalyzeChildKeys(childXmlPath, parentKeys, childKeyColumns, missingKeys, progress, cancellationToken));

            progress?.Report((missingKeys.Count, "Analysis complete"));

            return missingKeys;
        }

        /// <summary>
        /// Load keys from XML
        /// </summary>
        private void LoadKeysFromXml(
            string xmlPath, FastStringSet keys, List<string> keyColumns,
            IProgress<(long processed, string stage)> progress, CancellationToken cancellationToken)
        {
            var extractor = new XmlKeyExtractor(keyColumns);
            long processedRows = 0;
            bool inRow = false;
            var buffer = new StringBuilder(8192);

            using (var reader = new StreamReader(xmlPath, Encoding.UTF8, true, BUFFER_SIZE))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    line = line.Trim();
                    if (line == "<row>")
                    {
                        inRow = true;
                        buffer.Clear();
                        continue;
                    }
                    else if (line == "</row>" && inRow)
                    {
                        var key = extractor.ExtractKey(buffer.ToString());
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            keys.Add(key);
                            processedRows++;

                            if (processedRows % 10000 == 0)
                                progress?.Report((processedRows, "Loading parent keys"));
                        }

                        inRow = false;
                        buffer.Clear();
                    }
                    else if (inRow)
                    {
                        buffer.AppendLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Analyze child keys og find missing
        /// </summary>
        private void AnalyzeChildKeys(
            string xmlPath, FastStringSet parentKeys, List<string> keyColumns,
            List<string> missingKeys,
            IProgress<(long processed, string stage)> progress, CancellationToken cancellationToken)
        {
            var extractor = new XmlKeyExtractor(keyColumns);
            var processedChildKeys = new HashSet<string>();
            long processedRows = 0;
            bool inRow = false;
            var buffer = new StringBuilder(8192);

            using (var reader = new StreamReader(xmlPath, Encoding.UTF8, true, BUFFER_SIZE))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    line = line.Trim();
                    if (line == "<row>")
                    {
                        inRow = true;
                        buffer.Clear();
                        continue;
                    }
                    else if (line == "</row>" && inRow)
                    {
                        var key = extractor.ExtractKey(buffer.ToString());
                        if (!string.IsNullOrWhiteSpace(key) && processedChildKeys.Add(key))
                        {
                            if (!parentKeys.Contains(key))
                            {
                                missingKeys.Add(key);
                            }

                            processedRows++;

                            if (processedRows % 10000 == 0)
                                progress?.Report((processedRows, "Analyzing child keys"));
                        }

                        inRow = false;
                        buffer.Clear();
                    }
                    else if (inRow)
                    {
                        buffer.AppendLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// Generate repaired XML
        /// Optionally brug tableIndex metadata for bedre defaults
        /// </summary>
        public async Task GenerateRepairedXmlAsync(
            string sourceXmlPath,
            string outputXmlPath,
            List<string> missingKeys,
            List<string> keyColumns,      // ["c1", "c2", "c3"]
            string integrityDescription = null,
            TableIndexEntry tableEntry = null,
            IProgress<(long processed, string stage)> progress = null,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate
            if (keyColumns == null || keyColumns.Count == 0)
                throw new ArgumentException("keyColumns cannot be null or empty");

            // 2. Parse column structure
            Dictionary<string, ColumnDefaultInfo> columnDefaults;

            if (tableEntry != null && tableEntry.Columns != null)
            {
                // Strategy 1: Brug tableIndex metadata (bedst)
                columnDefaults = BuildColumnDefaultsFromTableIndex(tableEntry, integrityDescription);
            }
            else
            {
                // Strategy 2: Parse fra XML
                columnDefaults = BuildColumnDefaultsFromXml(sourceXmlPath, integrityDescription);
            }

            // 3. Generate repaired XML
            await GenerateXmlWithMissingRowsAsync(
                sourceXmlPath, outputXmlPath, missingKeys, keyColumns,
                integrityDescription, columnDefaults, progress, cancellationToken);
        }

        /// <summary>
        /// Column default information
        /// </summary>
        private class ColumnDefaultInfo
        {
            public string ColumnName { get; set; }
            public string DefaultValue { get; set; }
            public bool IsNillable { get; set; }
            public string DataType { get; set; }  // Fra tableIndex
        }

        /// <summary>
        /// Build column defaults fra TableIndex metadata
        /// </summary>
        private Dictionary<string, ColumnDefaultInfo> BuildColumnDefaultsFromTableIndex(
            TableIndexEntry tableEntry, string integrityDescription)
        {
            var defaults = new Dictionary<string, ColumnDefaultInfo>();
            int maxColumnNumber = tableEntry.Columns.Max(c => ExtractColumnNumber(c.ColumnID));

            foreach (var column in tableEntry.Columns.OrderBy(c => c.Position))
            {
                defaults[column.ColumnID] = new ColumnDefaultInfo
                {
                    ColumnName = column.ColumnID,
                    DefaultValue = column.GetDefaultValue(),  // Bruger TableIndexColumn metode
                    IsNillable = column.IsNullable,
                    DataType = column.DataType
                };
            }

            // Tilføj integrity error kolonne
            string newColumnID = $"c{maxColumnNumber + 1}";
            defaults[newColumnID] = new ColumnDefaultInfo
            {
                ColumnName = newColumnID,
                DefaultValue = integrityDescription,
                IsNillable = false,
                DataType = "VARCHAR(500)"
            };

            return defaults;
        }

        /// <summary>
        /// Build column defaults fra XML parsing - FALLBACK STRATEGI
        /// </summary>
        private Dictionary<string, ColumnDefaultInfo> BuildColumnDefaultsFromXml(
            string xmlPath, string integrityDescription)
        {
            var defaults = new Dictionary<string, ColumnDefaultInfo>();
            int maxColumnNumber = 0;

            // Parse første row for at finde kolonner
            var settings = new XmlReaderSettings
            {
                Async = false,
                IgnoreWhitespace = true,
                IgnoreComments = true,
                DtdProcessing = DtdProcessing.Ignore
            };

            using (var reader = XmlReader.Create(xmlPath, settings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
                    {
                        var rowSubtree = reader.ReadSubtree();

                        while (rowSubtree.Read())
                        {
                            if (rowSubtree.NodeType == XmlNodeType.Element && rowSubtree.Name.StartsWith("c"))
                            {
                                string columnName = rowSubtree.Name;
                                int columnNumber = ExtractColumnNumber(columnName);
                                maxColumnNumber = Math.Max(maxColumnNumber, columnNumber);

                                bool isNillable = rowSubtree.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true";

                                defaults[columnName] = new ColumnDefaultInfo
                                {
                                    ColumnName = columnName,
                                    DefaultValue = GetIntelligentDefault(columnName, isNillable),
                                    IsNillable = isNillable,
                                    DataType = "UNKNOWN"  // Kan ikke bestemmes fra XML
                                };
                            }
                        }
                        break; // Kun første row
                    }
                }
            }

            // Tilføj integrity error kolonne
            string newColumnID = $"c{maxColumnNumber + 1}";
            defaults[newColumnID] = new ColumnDefaultInfo
            {
                ColumnName = newColumnID,
                DefaultValue = integrityDescription,
                IsNillable = false,
                DataType = "VARCHAR(500)"
            };

            return defaults;
        }

        /// <summary>
        /// Intelligent default value gætværk uden metadata
        /// </summary>
        private string GetIntelligentDefault(string columnName, bool isNillable)
        {
            if (isNillable) return null;  // Will generate xsi:nil="true"

            // Column position-based heuristics (simple fallback)
            int colNumber = ExtractColumnNumber(columnName);

            if (colNumber <= 3) return "0";       // Ofte IDs/keys
            if (colNumber >= 15) return "";       // Ofte text felter

            return "0.0";  // Default numeric
        }

        /// <summary>
        /// Extract column number fra "c1", "c2" etc.
        /// </summary>
        private int ExtractColumnNumber(string columnID)
        {
            if (string.IsNullOrEmpty(columnID) || !columnID.StartsWith("c"))
                return 0;

            if (int.TryParse(columnID.Substring(1), out int number))
                return number;

            return 0;
        }

        /// <summary>
        /// Generate XML with missing rows
        /// </summary>
        private async Task GenerateXmlWithMissingRowsAsync(string sourceXmlPath, string outputXmlPath, List<string> missingKeys, List<string> keyColumns, string integrityDescription, 
            Dictionary<string, ColumnDefaultInfo> columnDefaults, IProgress<(long processed, string stage)> progress, CancellationToken cancellationToken)
        {
            // Find den nye kolonne (c10, c11, etc.)
            var newColumnName = columnDefaults.Keys.OrderByDescending(k => ExtractColumnNumber(k)).First();

            using (var reader = new StreamReader(sourceXmlPath, Encoding.UTF8, true, BUFFER_SIZE))
            using (var writer = new StreamWriter(outputXmlPath, false, Encoding.UTF8, BUFFER_SIZE))
            {
                string line;
                bool inRow = false;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string trimmedLine = line.TrimEnd();

                    // Detect start af row
                    if (trimmedLine.Trim() == "<row>")
                    {
                        inRow = true;
                        await writer.WriteLineAsync(line);
                        continue;
                    }

                    // Detect end af row - INDSÆT ny kolonne FØR </row>
                    if (inRow && (trimmedLine.Trim() == "</row>"))
                    {
                        // Tilføj den nye kolonne til eksisterende række - TOM (ikke nil)
                        string indentation = "    "; // Match eksisterende indentation
                        await writer.WriteLineAsync($"{indentation}<{newColumnName}></{newColumnName}>");
                
                        // Skriv </row>
                        await writer.WriteLineAsync(line);
                        inRow = false;
                        continue;
                    }

                    // Detect </table> - INDSÆT missing rows FØR </table>
                    if (trimmedLine == "</table>" || trimmedLine.Trim() == "</table>")
                    {
                        await AddMissingRowsLineBasedAsync(
                            writer, missingKeys, keyColumns, columnDefaults, progress, cancellationToken);

                        await writer.WriteLineAsync(line);
                        break;
                    }

                    // Kopier alle andre linjer direkte
                    await writer.WriteLineAsync(line);
                }

                // Kopier eventuelle linjer efter </table>
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    await writer.WriteLineAsync(line);
                }
            }
        }

        /// <summary>
        /// Add missing rows med linje-baseret string formatting
        /// </summary>
        private async Task AddMissingRowsLineBasedAsync(
            StreamWriter writer,
            List<string> missingKeys,     // ["123|ABC|999", "456|DEF|888"]
            List<string> keyColumns,      // ["c1", "c2", "c3"]
            Dictionary<string, ColumnDefaultInfo> columnDefaults,
            IProgress<(long processed, string stage)> progress,
            CancellationToken cancellationToken)
        {
            foreach (var missingKey in missingKeys)
            {
                // Split sammensatte PK
                var keyParts = missingKey.Split('|');  // ["123", "ABC", "999"]

                // Validate
                if (keyParts.Length != keyColumns.Count)
                    throw new InvalidOperationException($"Key mismatch");

                // Start row
                await writer.WriteLineAsync("  <row>");

                // Write ALL columns in order
                foreach (var kvp in columnDefaults.OrderBy(x => ExtractColumnNumber(x.Key)))
                {
                    var columnName = kvp.Key;
                    var info = kvp.Value;

                    // Check hvis denne kolonne er en key column
                    int keyIndex = keyColumns.IndexOf(columnName);

                    if (keyIndex >= 0)
                    {
                        // KEY COLUMN
                        string keyValue = keyParts[keyIndex];
                        string escaped = SecurityElement.Escape(keyValue);
                        await writer.WriteLineAsync($"    <{columnName}>{escaped}</{columnName}>");
                    }
                    else if (info.IsNillable && info.DefaultValue == null)
                    {
                        // NULLABLE
                        await writer.WriteLineAsync($"    <{columnName} xsi:nil=\"true\"/>");
                    }
                    else
                    {
                        // DEFAULT
                        string escaped = SecurityElement.Escape(info.DefaultValue ?? "");
                        await writer.WriteLineAsync($"    <{columnName}>{escaped}</{columnName}>");
                    }
                }

                await writer.WriteLineAsync("  </row>");
            }
        }
    }
}
