using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForskningsdataReparationskit.Models;

namespace ForskningsdataReparationskit.Utilities
{
    /// <summary>
    /// TableIndex parser til læsning af metadata
    /// </summary>
    public class TableIndexParser
    {
        /// <summary>
        /// Parse tableIndex.xml og udtræk alle tabel entries
        /// </summary>
        public static List<TableIndexEntry> ParseTableIndex(string tableIndexPath)
        {
            var entries = new List<TableIndexEntry>();

            try
            {
                var doc = System.Xml.Linq.XDocument.Load(tableIndexPath);
                var ns = doc.Root.GetDefaultNamespace();

                var tables = doc.Descendants(ns + "table");

                foreach (var table in tables)
                {
                    var entry = new TableIndexEntry
                    {
                        Name = table.Element(ns + "name")?.Value ?? "",
                        Folder = table.Element(ns + "folder")?.Value ?? "",
                        Description = table.Element(ns + "description")?.Value ?? "",
                        TotalRows = int.TryParse(table.Element(ns + "rows")?.Value, out int rows) ? rows : 0,

                        // Initialiser PrimaryKeyColumns liste
                        PrimaryKeyColumns = new List<string>()
                    };

                    // Parse columns (uændret)
                    var columnsElement = table.Element(ns + "columns");
                    if (columnsElement != null)
                    {
                        int position = 1;
                        foreach (var column in columnsElement.Elements(ns + "column"))
                        {
                            var tableColumn = new TableIndexColumn
                            {
                                Name = column.Element(ns + "name")?.Value ?? "",
                                ColumnID = column.Element(ns + "columnID")?.Value ?? "",
                                DataType = column.Element(ns + "type")?.Value ?? "",
                                TypeOriginal = column.Element(ns + "typeOriginal")?.Value ?? "",
                                IsNullable = column.Element(ns + "nullable")?.Value?.ToLower() == "true",
                                Description = column.Element(ns + "description")?.Value ?? "",
                                Position = position++
                            };

                            entry.Columns.Add(tableColumn);
                        }
                    }

                    // Parse ALLE primary key columns
                    var primaryKeyElement = table.Element(ns + "primaryKey");
                    if (primaryKeyElement != null)
                    {
                        // Bevar existing property for backward compatibility
                        entry.PrimaryKeyName = primaryKeyElement.Element(ns + "name")?.Value ?? "";

                        // Læs ALLE <column> elementer i primaryKey
                        foreach (var pkColumnElement in primaryKeyElement.Elements(ns + "column"))
                        {
                            string columnName = pkColumnElement.Value?.Trim();
                            if (!string.IsNullOrEmpty(columnName))
                            {
                                entry.PrimaryKeyColumns.Add(columnName);
                            }
                        }

                        // Fallback: Hvis ingen kolonner fundet, brug den gamle måde (første kolonne)
                        if (entry.PrimaryKeyColumns.Count == 0 && primaryKeyElement.Element(ns + "column") != null)
                        {
                            string firstColumn = primaryKeyElement.Element(ns + "column")?.Value ?? "";
                            if (!string.IsNullOrEmpty(firstColumn))
                            {
                                entry.PrimaryKeyColumns.Add(firstColumn);
                            }
                        }
                    }

                    // Parse foreign keys
                    var foreignKeysElement = table.Element(ns + "foreignKeys");
                    if (foreignKeysElement != null)
                    {
                        foreach (var fk in foreignKeysElement.Elements(ns + "foreignKey"))
                        {
                            var reference = fk.Element(ns + "reference");
                            if (reference != null)
                            {
                                var fkInfo = new ForeignKeyInfo
                                {
                                    Name = fk.Element(ns + "name")?.Value ?? "",
                                    ReferencedTable = fk.Element(ns + "referencedTable")?.Value ?? "",
                                    Column = reference.Element(ns + "column")?.Value ?? "",
                                    Referenced = reference.Element(ns + "referenced")?.Value ?? ""
                                };

                                entry.ForeignKeys.Add(fkInfo);
                            }
                        }
                    }

                    entries.Add(entry);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fejl ved parsing af tableIndex.xml: {ex.Message}", ex);
            }

            return entries;
        }

        /// <summary>
        /// Find tabel entry efter navn
        /// </summary>
        public static TableIndexEntry FindTableByName(List<TableIndexEntry> entries, string tableName)
        {
            return entries.FirstOrDefault(e =>
                string.Equals(e.Name, tableName, StringComparison.OrdinalIgnoreCase));
        }
    }
}