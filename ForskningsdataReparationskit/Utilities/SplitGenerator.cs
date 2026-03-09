using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForskningsdataReparationskit.Models;

namespace ForskningsdataReparationskit.Utilities
{
    /// <summary>
    /// Split configuration generator med TableIndex metadata
    /// </summary>
    public static class SplitGenerator
    {
        /// <summary>
        /// Generate split tabeller med TableIndex metadata og naming strategy
        /// </summary>
        public static List<SplitTable> GenerateSplitTables(
            TableIndexEntry originalEntry,
            List<int> splitPoints,
            string primaryKeyName)
        {
            var splitTables = new List<SplitTable>();
            int startColumn = 1;
            int splitIndex = 1;

            foreach (int splitPoint in splitPoints)
            {
                var splitTable = new SplitTable
                {
                    TableName = GenerateTableName(originalEntry.Name, splitIndex),
                    OriginalName = originalEntry.Name,
                    StartColumn = startColumn,
                    EndColumn = splitPoint,
                    IsFirstSplit = splitIndex == 1
                };

                // Tilføj relevante kolonner fra TableIndex
                foreach (var column in originalEntry.Columns)
                {
                    int columnNumber = column.Position;
                    if (columnNumber >= startColumn && columnNumber <= splitPoint)
                    {
                        splitTable.Columns.Add(ConvertToColumn(column));
                    }
                }

                splitTables.Add(splitTable);
                startColumn = splitPoint + 1;
                splitIndex++;
            }

            // Sidste split (alt efter sidste split punkt)
            if (startColumn <= originalEntry.Columns.Count)
            {
                var finalSplit = new SplitTable
                {
                    TableName = GenerateTableName(originalEntry.Name, splitIndex),
                    OriginalName = originalEntry.Name,
                    StartColumn = startColumn,
                    EndColumn = originalEntry.Columns.Count,
                    IsFirstSplit = splitIndex == 1 && splitTables.Count == 0
                };

                foreach (var column in originalEntry.Columns)
                {
                    int columnNumber = column.Position;
                    if (columnNumber >= startColumn)
                    {
                        finalSplit.Columns.Add(ConvertToColumn(column));
                    }
                }

                splitTables.Add(finalSplit);
            }

            return splitTables;
        }

        /// <summary>
        /// Generate table name
        /// </summary>
        private static string GenerateTableName(string originalName, int splitIndex)
        {
            if (splitIndex == 1)
                return originalName;
            else
                return originalName + "_" + splitIndex.ToString();
        }

        /// <summary>
        /// Convert TableIndexColumn til XMLColumn
        /// </summary>
        private static XMLColumn ConvertToColumn(TableIndexColumn tableColumn)
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
    }
}
