using System;
using System.Collections.Generic;
using System.Linq;
using TestvaerkstedetToolkit.Models;

namespace TestvaerkstedetToolkit.Services
{
    /// <summary>
    /// Service for split configuration and calculation
    /// Håndterer beregning af split punkter, validering og generering af split tabeller
    /// </summary>
    public class SplitConfigurationService
    {
        /// <summary>
        /// Beregn automatiske split punkter baseret på kolonne antal og PK størrelse
        /// </summary>
        public List<int> CalculateAutoSplitPoints(TableIndexEntry tableEntry, List<string> pkColumns, int maxColumnsPerTable = 950)
        {
            var splitPoints = new List<int>();

            if (tableEntry == null || tableEntry.Columns == null || tableEntry.Columns.Count == 0)
                return splitPoints;

            int pkColumnsCount = pkColumns.Count;
            int totalColumns = tableEntry.Columns.Count;

            if (totalColumns + pkColumnsCount <= maxColumnsPerTable)
            {
                // Scenario: Under 950 - halvér for test
                int midPoint = totalColumns / 2;
                if (midPoint > 0 && midPoint < totalColumns)
                {
                    splitPoints.Add(midPoint);
                }
            }
            else
            {
                // Scenario: Over 950 - fyld første tabel med 950 inklusiv PK
                int availableForDataInFirstSplit = maxColumnsPerTable - pkColumnsCount;

                if (availableForDataInFirstSplit > 0)
                {
                    splitPoints.Add(availableForDataInFirstSplit);

                    int remainingColumns = totalColumns - availableForDataInFirstSplit;
                    int currentPosition = availableForDataInFirstSplit + 1;

                    while (remainingColumns > (maxColumnsPerTable - pkColumnsCount))
                    {
                        currentPosition += (maxColumnsPerTable - pkColumnsCount);
                        if (currentPosition < totalColumns)
                        {
                            splitPoints.Add(currentPosition);
                            remainingColumns = totalColumns - currentPosition;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return splitPoints;
        }

        /// <summary>
        /// Parser split points fra string input
        /// </summary>
        public List<int> ParseSplitPoints(string input)
        {
            var points = new List<int>();

            if (string.IsNullOrWhiteSpace(input))
                return points;

            foreach (string part in input.Split(',', ';'))
            {
                string trimmed = part.Trim();
                if (int.TryParse(trimmed, out int point))
                {
                    points.Add(point);
                }
            }

            return points.Distinct().OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Valider split points med TableIndex metadata
        /// </summary>
        public string ValidateTableIndexAwareSplitPoints(List<int> splitPoints, TableIndexEntry tableEntry, List<XMLColumn> allColumns, List<string> pkColumns, int maxColumnsPerTable = 950)
        {
            var errors = new List<string>();

            if (tableEntry == null)
            {
                errors.Add("TableIndex metadata mangler");
                return string.Join("\n", errors);
            }

            int maxColumns = tableEntry.Columns.Count;

            foreach (int point in splitPoints)
            {
                if (point < 1 || point >= maxColumns)
                {
                    errors.Add($"Split punkt {point} er udenfor gyldigt interval (1-{maxColumns - 1})");
                }
            }

            var tableSizes = CalculateTableSizes(splitPoints, allColumns, pkColumns);
            for (int i = 0; i < tableSizes.Count; i++)
            {
                if (tableSizes[i] > maxColumnsPerTable)
                {
                    errors.Add($"Tabel {i + 1} vil få {tableSizes[i]} kolonner (over {maxColumnsPerTable} grænse)");
                }
            }

            return errors.Count > 0 ? string.Join("\n", errors) : null;
        }

        /// <summary>
        /// Beregn tabel størrelser baseret på split points
        /// </summary>
        public List<int> CalculateTableSizes(List<int> splitPoints, List<XMLColumn> allColumns, List<string> pkColumns)
        {
            var sizes = new List<int>();
            int lastPoint = 0;
            int pkCount = pkColumns.Count;

            foreach (int point in splitPoints)
            {
                int dataColumns = point - lastPoint;
                int totalSize = dataColumns + pkCount;
                sizes.Add(totalSize);
                lastPoint = point;
            }

            int finalDataColumns = allColumns.Count - lastPoint;
            int finalSize = finalDataColumns + pkCount;
            sizes.Add(finalSize);

            return sizes;
        }

        /// <summary>
        /// Generer split tables med TableIndex metadata
        /// </summary>
        public List<SplitTable> GenerateTableIndexAwareSplitTables(
            List<int> splitPoints,
            TableIndexEntry tableEntry,
            List<XMLColumn> allColumns,
            PrimaryKeyInfo pkInfo)
        {
            var resultTables = new List<SplitTable>();
            if (tableEntry == null) return resultTables;

            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            var sortedSplitPoints = splitPoints.OrderBy(x => x).ToList();

            int splitIndex = 1;
            int startColumn = 1;

            // Generer splits baseret på split points
            foreach (int splitPoint in sortedSplitPoints)
            {
                var table = new SplitTable
                {
                    TableName = $"{tableEntry.Name}_{splitIndex}",
                    StartColumn = startColumn,
                    EndColumn = splitPoint,
                    SplitIndex = splitIndex,
                    Columns = new List<XMLColumn>()
                };

                // Data kolonner i interval [startColumn, splitPoint]
                var dataColumns = allColumns
                    .Where(c => c.Position >= startColumn && c.Position <= splitPoint &&
                               !pkColumns.Contains(c.Name))
                    .OrderBy(c => c.Position)
                    .ToList();

                foreach (var dataCol in dataColumns)
                {
                    table.Columns.Add(dataCol);
                }

                // Tilføj ALLE PK kolonner
                foreach (var pkColumnName in pkColumns)
                {
                    var pkCol = allColumns.FirstOrDefault(c => c.Name == pkColumnName);
                    if (pkCol != null)
                    {
                        table.Columns.Add(pkCol);
                    }
                }

                resultTables.Add(table);
                startColumn = splitPoint + 1;
                splitIndex++;
            }

            // Sidste tabel får resterende kolonner
            if (startColumn <= allColumns.Count)
            {
                var lastTable = new SplitTable
                {
                    TableName = $"{tableEntry.Name}_{splitIndex}",
                    StartColumn = startColumn,
                    EndColumn = allColumns.Count,
                    SplitIndex = splitIndex,
                    Columns = new List<XMLColumn>()
                };

                var lastDataColumns = allColumns
                    .Where(c => c.Position >= startColumn && !pkColumns.Contains(c.Name))
                    .OrderBy(c => c.Position)
                    .ToList();

                foreach (var dataCol in lastDataColumns)
                {
                    lastTable.Columns.Add(dataCol);
                }

                foreach (var pkColumnName in pkColumns)
                {
                    var pkCol = allColumns.FirstOrDefault(c => c.Name == pkColumnName);
                    if (pkCol != null)
                    {
                        lastTable.Columns.Add(pkCol);
                    }
                }

                resultTables.Add(lastTable);
            }

            return resultTables;
        }
    }
}