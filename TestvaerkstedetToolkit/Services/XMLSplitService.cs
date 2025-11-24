using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using TestvaerkstedetToolkit.Models;

namespace TestvaerkstedetToolkit.Services
{
    /// <summary>
    /// Orchestrator service for XML table splitting operations
    /// Koordinerer split execution workflow mellem services og håndterer file system operationer
    /// </summary>
    public class XMLSplitService
    {
        private readonly TableIndexService tableIndexService;
        private readonly XMLFileGenerationService fileGenerationService;
        private readonly SplitConfigurationService splitConfigService;

        public XMLSplitService()
        {
            tableIndexService = new TableIndexService();
            fileGenerationService = new XMLFileGenerationService();
            splitConfigService = new SplitConfigurationService();
        }

        /// <summary>
        /// Resultat fra split operation
        /// </summary>
        public class SplitOperationResult
        {
            public bool Success { get; set; }
            public string OutputDirectory { get; set; }
            public string ErrorMessage { get; set; }
            public int TablesGenerated { get; set; }
            public int TotalRows { get; set; }
            public List<string> GeneratedFiles { get; set; } = new List<string>();
        }

        /// <summary>
        /// Interface for progress reporting
        /// </summary>
        public interface IProgressReporter
        {
            void Report(int percentage, string message);
        }

        /// <summary>
        /// Interface for logging operations
        /// </summary>
        public interface ISplitLogger
        {
            void LogInfo(string message);
            void LogWarning(string message);
            void LogError(string message);
            void SaveToFile(string logPath);
        }

        /// <summary>
        /// Hovedmetode: Udfør split operation med ATOMIC OPERATIONS
        /// Genererer i temp folder først, validerer, derefter flytter til final destination
        /// </summary>
        public SplitOperationResult ExecuteSplit(
            UIDataContainer uiData,
            IProgressReporter progress = null,
            ISplitLogger logger = null)
        {
            var result = new SplitOperationResult();
            string tempDirectory = null;
            string finalOutputDirectory = null;

            try
            {
                // STEP 1: Opret temp directory
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string parentFolder = Path.Combine(desktopPath, "XML_Table_Splits");

                // Parse original table nummer
                int originalTableNumber = int.Parse(Regex.Match(uiData.OriginalTableEntry.Folder, @"table(\d+)").Groups[1].Value);

                string versionNumber = GetNextVersionNumber(parentFolder, uiData.OriginalTableName);
                string splitFolderName = $"split_{uiData.OriginalTableName}_table{originalTableNumber}_{versionNumber}";

                // TEMP folder for generation
                tempDirectory = Path.Combine(Path.GetTempPath(), $"xml_split_temp_{Guid.NewGuid():N}");
                Directory.CreateDirectory(tempDirectory);

                // FINAL destination
                finalOutputDirectory = Path.Combine(parentFolder, splitFolderName);

                logger?.LogInfo($"Arbejder i temp mappe: {tempDirectory}");
                logger?.LogInfo($"Final destination: {finalOutputDirectory}");

                progress?.Report(5, "Initialiserer opdeling...");

                // Parse next table nummer
                int nextTableNumber = GetNextAvailableTableNumber(uiData.TableIndexPath);

                progress?.Report(10, "Genererer split tabeller i temp...");

                // STEP 2: Generer ALLE filer i temp directory
                for (int i = 0; i < uiData.Tables.Count; i++)
                {
                    int currentTableNumber = (i == 0) ? originalTableNumber : nextTableNumber + (i - 1);
                    fileGenerationService.GenerateTableFiles(uiData.Tables[i], tempDirectory, uiData, currentTableNumber);
                }

                progress?.Report(75, "Opdaterer tableIndex.xml...");

                // STEP 3: Opdater tableIndex.xml
                if (!string.IsNullOrEmpty(uiData.TableIndexPath))
                {
                    logger?.LogInfo("Starter opdatering af tableIndex.xml med sammensat PK support");
                    try
                    {
                        string tableIndexLog = tableIndexService.GenerateUpdatedTableIndex(
                            uiData.Tables,
                            tempDirectory,
                            uiData,
                            originalTableNumber,
                            nextTableNumber);

                        logger?.LogInfo("Afsluttet opdatering af tableIndex.xml");
                        logger?.LogInfo("");
                        logger?.LogInfo("=== TABLEINDEX TRANSFORMATION DETAILS ===");
                        logger?.LogInfo(tableIndexLog);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError($"Fejl ved opdatering af tableIndex.xml: {ex.Message}");
                        throw; // Re-throw for rollback
                    }
                }

                progress?.Report(85, "Validerer output...");

                // STEP 4: Validér output før commit
                logger?.LogInfo("Starter validering af genererede filer...");
                ValidateSplitOutput(tempDirectory, uiData, logger);
                logger?.LogInfo("Validering bestået - alle filer er korekte");

                progress?.Report(90, "Flytter filer til final destination...");

                // STEP 5: Opret final directory og flyt alt
                Directory.CreateDirectory(Path.GetDirectoryName(finalOutputDirectory));
                Directory.Move(tempDirectory, finalOutputDirectory);
                tempDirectory = null; // Mark som flyttet

                logger?.LogInfo($"Filer flyttet til final destination: {finalOutputDirectory}");

                progress?.Report(95, "Afslutter...");

                // STEP 6: Populate result
                result.Success = true;
                result.OutputDirectory = finalOutputDirectory;
                result.TablesGenerated = uiData.Tables.Count;
                result.TotalRows = uiData.TotalRows;
                result.GeneratedFiles = Directory.GetFiles(finalOutputDirectory, "*", SearchOption.AllDirectories).ToList();

                progress?.Report(100, "Opdeling fuldført");
                return result;
            }
            catch (Exception ex)
            {
                logger?.LogError($"KRITISK FEJL: {ex.Message}");
                logger?.LogError($"Stack trace: {ex.StackTrace}");

                // Slet temp directory hvis det stadig eksisterer (rollback)
                if (tempDirectory != null && Directory.Exists(tempDirectory))
                {
                    try
                    {
                        Directory.Delete(tempDirectory, true);
                        logger?.LogInfo("Temp directory slettet efter fejl (rollback)");
                    }
                    catch (Exception cleanupEx)
                    {
                        logger?.LogWarning($"Kunne ikke slette temp directory: {cleanupEx.Message}");
                    }
                }

                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Validér at split output er korrekt før commit
        /// </summary>
        private void ValidateSplitOutput(string outputDirectory, UIDataContainer uiData, ISplitLogger logger)
        {
            var errors = new List<string>();

            // 1. Find alle split XML filer (EXCLUDE tableIndex)
            var xmlFiles = Directory.GetFiles(outputDirectory, "*.xml", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f).StartsWith("table") &&
                           !Path.GetFileName(f).Contains("tableIndex"))
                .ToList();

            // 2. Tjek at alle forventede XML filer findes
            if (xmlFiles.Count < uiData.Tables.Count)
            {
                errors.Add($"Forventede {uiData.Tables.Count} split XML filer, fandt kun {xmlFiles.Count}");
            }

            // 3. Tjek at tableIndex findes
            string tableIndexPath = Path.Combine(outputDirectory, "tableIndex_updated.xml");
            if (!File.Exists(tableIndexPath))
            {
                errors.Add("tableIndex_updated.xml mangler");
            }
            else
            {
                // 4. Tjek at tableIndex er valid XML
                try
                {
                    var doc = XDocument.Load(tableIndexPath);

                    // 5. Tjek at alle split tabeller er i tableIndex
                    var ns = doc.Root.GetDefaultNamespace();
                    foreach (var table in uiData.Tables)
                    {
                        var tableElement = doc.Descendants(ns + "table")
                            .FirstOrDefault(t => t.Element(ns + "name")?.Value == table.TableName);

                        if (tableElement == null)
                        {
                            errors.Add($"Split tabel '{table.TableName}' mangler i tableIndex");
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"tableIndex_updated.xml er ikke valid XML: {ex.Message}");
                }
            }

            // 6. Validér row counts og struktur i SPLIT filer
            foreach (var xmlFile in xmlFiles)
            {
                try
                {
                    int rowCount = 0;
                    bool hasTableRoot = false;

                    using (var reader = XmlReader.Create(xmlFile))
                    {
                        // Tjek root element
                        reader.MoveToContent();
                        if (reader.Name == "table")
                        {
                            hasTableRoot = true;
                        }

                        // Count rows
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
                            {
                                rowCount++;
                            }
                        }
                    }

                    // Valider struktur
                    if (!hasTableRoot)
                    {
                        errors.Add($"{Path.GetFileName(xmlFile)}: Invalid root element (expected <table>)");
                    }

                    // Valider row count
                    if (rowCount != uiData.TotalRows)
                    {
                        errors.Add($"{Path.GetFileName(xmlFile)}: Forventede {uiData.TotalRows} rows, fandt {rowCount}");
                    }
                    else if (hasTableRoot)
                    {
                        logger?.LogInfo($"{Path.GetFileName(xmlFile)}: Validation OK ({rowCount} rows, valid structure)");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(xmlFile)}: Kunne ikke validere - {ex.Message}");
                }
            }

            if (errors.Count > 0)
            {
                logger?.LogError("VALIDERING FEJLEDE:");
                foreach (var error in errors)
                {
                    logger?.LogError($"  - {error}");
                }

                throw new InvalidOperationException(
                    $"Split output validering fejlede med {errors.Count} fejl:\n" +
                    string.Join("\n", errors)
                );
            }

            logger?.LogInfo($"Validering OK - {uiData.Tables.Count} tabeller, {uiData.TotalRows} rows per tabel");
        }

        /// <summary>
        /// Find næste version nummer baseret på eksisterende mapper
        /// </summary>
        private string GetNextVersionNumber(string parentFolder, string tableName)
        {
            try
            {
                if (!Directory.Exists(parentFolder))
                    return "v1.0";

                var existingFolders = Directory.GetDirectories(parentFolder)
                    .Where(dir => Path.GetFileName(dir).StartsWith($"split_{tableName}_v"))
                    .ToList();

                if (existingFolders.Count == 0)
                    return "v1.0";

                // Find højeste version nummer
                double maxVersion = 0;
                foreach (var folder in existingFolders)
                {
                    var folderName = Path.GetFileName(folder);
                    var versionStart = folderName.IndexOf("_v") + 2;
                    var versionEnd = folderName.IndexOf("_", versionStart);

                    if (versionStart > 1 && versionEnd > versionStart)
                    {
                        var versionStr = folderName.Substring(versionStart, versionEnd - versionStart);
                        if (double.TryParse(versionStr, out double version))
                        {
                            maxVersion = Math.Max(maxVersion, version);
                        }
                    }
                }

                double newVersion = maxVersion + 0.1;
                return $"v{newVersion:F1}";
            }
            catch
            {
                return "v1.0";
            }
        }

        /// <summary>
        /// Find næste tilgængelige table nummer fra tableIndex.xml
        /// </summary>
        private int GetNextAvailableTableNumber(string tableIndexPath)
        {
            var doc = XDocument.Load(tableIndexPath);
            var ns = doc.Root.GetDefaultNamespace();

            var highestNumber = doc.Descendants(ns + "folder")
                .Select(f => Regex.Match(f.Value, @"table(\d+)$"))
                .Where(m => m.Success)
                .Select(m => int.Parse(m.Groups[1].Value))
                .DefaultIfEmpty(0)
                .Max();

            return highestNumber + 1;
        }
    }
}