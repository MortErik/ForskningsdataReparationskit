using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestvaerkstedetToolkit.Models;

namespace TestvaerkstedetToolkit.Models
{

    /// <summary>
    /// Result fra tableIndex opdatering med logs
    /// </summary>
    public class TableIndexUpdateResult
    {
        public string TechnicalLog { get; set; }
        public string ReferenceLog { get; set; }
    }

    /// <summary>
    /// TableIndex entry med komplet metadata
    /// </summary>
    public class TableIndexEntry
    {
        public string Name { get; set; }
        public string Folder { get; set; }
        public string Description { get; set; }
        public List<TableIndexColumn> Columns { get; set; } = new List<TableIndexColumn>();
        public List<ForeignKeyInfo> ForeignKeys { get; set; } = new List<ForeignKeyInfo>();

        // Bevar for backward compatibility
        public string PrimaryKeyName { get; set; }

        //Support for multiple PK kolonner
        public List<string> PrimaryKeyColumns { get; set; } = new List<string>();

        public int TotalRows { get; set; }

        /// <summary>
        /// Display tekst til ComboBox - OPDATERET MED PK INFO
        /// </summary>
        public string DisplayText => $"{Name} (folder: {Folder}, {Columns.Count} kolonner, {GetPrimaryKeyDisplayText()})";

        /// <summary>
        /// Få alle PK kolonner som kommasepareret string
        /// </summary>
        public string GetPrimaryKeyDisplayText()
        {
            if (PrimaryKeyColumns == null || PrimaryKeyColumns.Count == 0)
                return "Ingen PK";

            if (PrimaryKeyColumns.Count == 1)
                return $"PK: {PrimaryKeyColumns[0]}";

            return $"sammensat PK: {string.Join(", ", PrimaryKeyColumns)}";
        }

        /// <summary>
        /// BACKWARD COMPATIBILITY: Første PK kolonne (som før)
        /// </summary>
        public string PrimaryKey => PrimaryKeyColumns?.FirstOrDefault() ?? "";

        /// <summary>
        ///  XML fil søgning med multiple patterns og fallbacks (uændret)
        /// </summary>
        public string FindXmlPath(string tableIndexDirectory)
        {
            // Gå op til AVID base directory
            string indicesDirectory = Path.GetDirectoryName(tableIndexDirectory); // C:\AVID.SA.18005.1\Indices
            string baseDirectory = Path.GetDirectoryName(indicesDirectory);       // C:\AVID.SA.18005.1\
            string tablesDirectory = Path.Combine(baseDirectory, "Tables");       // C:\AVID.SA.18005.1\Tables

            System.Diagnostics.Debug.WriteLine($"=== XML SEARCH DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"TableIndex path: {tableIndexDirectory}");
            System.Diagnostics.Debug.WriteLine($"Indices dir: {indicesDirectory}");
            System.Diagnostics.Debug.WriteLine($"Base dir: {baseDirectory}");
            System.Diagnostics.Debug.WriteLine($"Tables dir: {tablesDirectory}");
            System.Diagnostics.Debug.WriteLine($"Target folder: {Folder}");

            if (!Directory.Exists(tablesDirectory))
            {
                System.Diagnostics.Debug.WriteLine($"Tables directory ikke fundet!");
                return null;
            }

            // PATTERN 1: Direct match - Tables/tableX/tableX.xml
            string exactMatch = Path.Combine(tablesDirectory, Folder, $"{Folder}.xml");
            System.Diagnostics.Debug.WriteLine($"Prøver exact match: {exactMatch}");

            if (File.Exists(exactMatch))
            {
                System.Diagnostics.Debug.WriteLine($" FUNDET: {exactMatch}");
                return exactMatch;
            }

            // PATTERN 2: List XML filer i target folder (backup)
            string folderPath = Path.Combine(tablesDirectory, Folder);
            if (Directory.Exists(folderPath))
            {
                var xmlFiles = Directory.GetFiles(folderPath, "*.xml", SearchOption.TopDirectoryOnly);
                System.Diagnostics.Debug.WriteLine($"XML filer i {folderPath}: {string.Join(", ", xmlFiles.Select(Path.GetFileName))}");

                if (xmlFiles.Length > 0)
                {
                    System.Diagnostics.Debug.WriteLine($" BACKUP FUNDET: {xmlFiles[0]}");
                    return xmlFiles[0];
                }
            }

            System.Diagnostics.Debug.WriteLine($"✗ INGEN XML FIL FUNDET");
            return null;
        }

        /// <summary>
        /// Auto-foreslået XML fil path (uændret)
        /// </summary>
        public string SuggestedXmlPath(string tableIndexDirectory)
        {
            return FindXmlPath(tableIndexDirectory) ?? "[IKKE FUNDET - Browse manuelt]";
        }

        /// <summary>
        /// OPDATERET: Primary key column med composite support
        /// </summary>
        public string PrimaryKeyColumn
        {
            get
            {
                // Hvis sammensat PK, brug første kolonne til backward compatibility
                string primaryColumnName = PrimaryKeyColumns?.FirstOrDefault() ?? PrimaryKeyName;

                // Find primærnøgle kolonne fra navnet
                return Columns?.FirstOrDefault(c => c.Name == primaryColumnName)?.ColumnID ?? "c1";
            }
        }

        /// <summary>
        /// Hent alle PK column IDs
        /// </summary>
        public List<string> GetPrimaryKeyColumnIDs()
        {
            var columnIDs = new List<string>();

            if (PrimaryKeyColumns == null || PrimaryKeyColumns.Count == 0)
                return columnIDs;

            foreach (var pkColumnName in PrimaryKeyColumns)
            {
                var column = Columns?.FirstOrDefault(c => c.Name == pkColumnName);
                if (column != null)
                {
                    columnIDs.Add(column.ColumnID);
                }
            }

            return columnIDs;
        }

        /// <summary>
        /// Er det en sammensat primary key?
        /// </summary>
        public bool HasCompositePrimaryKey => PrimaryKeyColumns != null && PrimaryKeyColumns.Count > 1;
    }
}