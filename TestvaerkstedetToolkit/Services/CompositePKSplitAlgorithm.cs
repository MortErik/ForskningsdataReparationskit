using System;
using System.Collections.Generic;
using System.Linq;
using TestvaerkstedetToolkit.Models;

namespace TestvaerkstedetToolkit.Services
{
    /// <summary>
    /// Komplet implementation af composite primary key split algoritme
    /// </summary>
    public static class CompositePKSplitAlgorithm
    {
        /// <summary>
        /// Generer split tables med sammensat PK support
        /// </summary>
        public static List<SplitTable> GenerateSplitTables(UIDataContainer uiData)
        {
            if (uiData?.PrimaryKey == null || !uiData.PrimaryKey.IsValid())
                throw new ArgumentException("Invalid primary key configuration");

            if (uiData.AllColumns == null || uiData.AllColumns.Count == 0)
                throw new ArgumentException("No columns available for splitting");

            var pkInfo = uiData.PrimaryKey;

            // 1. IDENTIFICER PK KOLONNER - BRUG KUN AllColumns (indeholder AutoID fra AutoIDManager)
            var pkColumnNames = pkInfo.GetAllPrimaryKeyColumns();
            var pkColumns = new List<TableIndexColumn>();
            var nonPKColumns = new List<TableIndexColumn>();

            // ENKELT foreach - brug KUN AllColumns som source of truth
            foreach (var xmlColumn in uiData.AllColumns.OrderBy(c => c.Position))
            {
                // Convert XMLColumn til TableIndexColumn for backward compatibility
                var tableColumn = new TableIndexColumn
                {
                    Name = xmlColumn.Name,
                    ColumnID = xmlColumn.ColumnID,
                    DataType = xmlColumn.DataType,
                    TypeOriginal = xmlColumn.TypeOriginal,
                    IsNullable = xmlColumn.IsNullable,
                    Description = xmlColumn.Description,
                    Position = xmlColumn.Position
                };

                if (pkColumnNames.Contains(xmlColumn.Name))
                {
                    pkColumns.Add(tableColumn);
                }
                else
                {
                    nonPKColumns.Add(tableColumn);
                }
            }

            // 2. BEREGN SPLIT KAPACITET
            int pkColumnsCount = pkColumns.Count;
            int maxDataColumnsPerSplit = Math.Max(0, 950 - pkColumnsCount);

            if (maxDataColumnsPerSplit <= 0)
                throw new InvalidOperationException($"For mange PK kolonner ({pkColumnsCount}). Maksimalt 999 PK kolonner tilladt.");

            // 3. BEREGN ANTAL SPLITS NØDVENDIGE
            int requiredSplits = (int)Math.Ceiling((double)nonPKColumns.Count / maxDataColumnsPerSplit);
            if (requiredSplits == 0) requiredSplits = 1; // Minimum 1 split

            // 4. GENERER SPLITS
            var splitTables = new List<SplitTable>();

            for (int i = 0; i < requiredSplits; i++)
            {
                var splitTable = GenerateSingleSplit(
                    splitIndex: i + 1,
                    nonPKColumns: nonPKColumns,
                    pkColumns: pkColumns,
                    maxDataColumnsPerSplit: maxDataColumnsPerSplit,
                    currentSplitIndex: i,
                    uiData: uiData
                );

                splitTables.Add(splitTable);
            }

            return splitTables;
        }

        /// <summary>
        /// Generer et enkelt split
        /// </summary>
        private static SplitTable GenerateSingleSplit(
            int splitIndex,
            List<TableIndexColumn> nonPKColumns,
            List<TableIndexColumn> pkColumns,
            int maxDataColumnsPerSplit,
            int currentSplitIndex,
            UIDataContainer uiData)
        {
            var splitTable = new SplitTable
            {
                SplitIndex = splitIndex,
                IsFirstSplit = splitIndex == 1,
                Columns = new List<XMLColumn>()
            };

            // Beregn data kolonne interval for dette split
            int startDataIndex = currentSplitIndex * maxDataColumnsPerSplit;
            int endDataIndex = Math.Min(startDataIndex + maxDataColumnsPerSplit - 1, nonPKColumns.Count - 1);

            // Tilføj data kolonner til dette split
            if (startDataIndex < nonPKColumns.Count)
            {
                for (int j = startDataIndex; j <= endDataIndex && j < nonPKColumns.Count; j++)
                {
                    var dataColumn = ConvertToXMLColumn(nonPKColumns[j]);
                    splitTable.Columns.Add(dataColumn);
                }

                // Beregn original position range for legacy support
                splitTable.StartColumn = nonPKColumns[startDataIndex].Position;
                splitTable.EndColumn = endDataIndex < nonPKColumns.Count ?
                    nonPKColumns[endDataIndex].Position :
                    nonPKColumns.Last().Position;
            }
            else
            {
                // Edge case: kun PK kolonner
                splitTable.StartColumn = 1;
                splitTable.EndColumn = 1;
            }

            // Tilføj ALLE PK kolonner til ALLE splits (duplikering)
            foreach (var pkColumn in pkColumns)
            {
                var xmlPKColumn = ConvertToXMLColumn(pkColumn);
                xmlPKColumn.Description = (xmlPKColumn.Description ?? "") + " (Primary Key - duplikeret til alle splits)";
                splitTable.Columns.Add(xmlPKColumn);
            }

            // Generer tabel navn
            splitTable.TableName = GenerateTableName(uiData.OriginalTableName, splitIndex);

            return splitTable;
        }

        /// <summary>
        /// Convert TableIndexColumn til XMLColumn
        /// </summary>
        private static XMLColumn ConvertToXMLColumn(TableIndexColumn tableColumn)
        {
            return new XMLColumn
            {
                Name = tableColumn.Name,
                ColumnID = tableColumn.ColumnID,
                DataType = tableColumn.DataType,
                TypeOriginal = tableColumn.TypeOriginal,
                IsNullable = tableColumn.IsNullable,
                Description = tableColumn.Description,
                Position = tableColumn.Position
            };
        }

        /// <summary>
        /// Generer tabel navn med konsistent naming convention
        /// </summary>
        private static string GenerateTableName(string originalTableName, int splitIndex)
        {
            return splitIndex == 1 ? originalTableName : $"{originalTableName}_{splitIndex}";
        }

        /// <summary>
        /// Validér split konfiguration
        /// </summary>
        public static bool ValidateSplitConfiguration(List<SplitTable> splits, UIDataContainer uiData)
        {
            if (splits == null || splits.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("Validation failed: No splits generated");
                return false;
            }

            if (uiData?.PrimaryKey == null || !uiData.PrimaryKey.IsValid())
            {
                System.Diagnostics.Debug.WriteLine("Validation failed: Invalid PK configuration");
                return false;
            }

            // Validér at alle splits har PK kolonner
            var expectedPKColumns = uiData.PrimaryKey.GetAllPrimaryKeyColumns();

            foreach (var split in splits)
            {
                if (split.Columns == null || split.Columns.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Validation failed: Split {split.SplitIndex} has no columns");
                    return false;
                }

                // Tjek at alle PK kolonner er til stede
                foreach (var expectedPK in expectedPKColumns)
                {
                    if (!split.Columns.Any(c => c.Name == expectedPK))
                    {
                        System.Diagnostics.Debug.WriteLine($"Validation failed: Split {split.SplitIndex} missing PK column {expectedPK}");
                        return false;
                    }
                }

                // Tjek kolonneantal under grænse
                if (split.Columns.Count > 1000)
                {
                    System.Diagnostics.Debug.WriteLine($"Validation failed: Split {split.SplitIndex} has {split.Columns.Count} columns (over 1000 limit)");
                    return false;
                }
            }

            System.Diagnostics.Debug.WriteLine($"Validation successful: {splits.Count} splits generated with sammensat PK support");
            return true;
        }

        /// <summary>
        /// Debug information for split generation
        /// </summary>
        public static void LogSplitSummary(List<SplitTable> splits, UIDataContainer uiData)
        {
            var pkColumns = uiData.PrimaryKey.GetAllPrimaryKeyColumns();

            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("=== sammensat PK SPLIT SUMMARY ===");
            System.Diagnostics.Debug.WriteLine($"Original table: {uiData.OriginalTableName}");
            System.Diagnostics.Debug.WriteLine($"PK columns: {string.Join(", ", pkColumns)} (Count: {pkColumns.Count})");
            System.Diagnostics.Debug.WriteLine($"Total original columns: {uiData.OriginalTableEntry.Columns.Count}");
            System.Diagnostics.Debug.WriteLine($"Generated splits: {splits.Count}");

            foreach (var split in splits)
            {
                var dataColumns = split.Columns.Where(c => !pkColumns.Contains(c.Name)).Count();
                var pkColumnsInSplit = split.Columns.Where(c => pkColumns.Contains(c.Name)).Count();

                System.Diagnostics.Debug.WriteLine($"  {split.TableName}: {dataColumns} data + {pkColumnsInSplit} PK = {split.Columns.Count} total columns");
            }
            System.Diagnostics.Debug.WriteLine("===================================");
        }
    }
}