using System;
using System.Text.RegularExpressions;

namespace TestvaerkstedetToolkit.Utilities
{
    public static class DataTypeMapper
    {
        /// <summary>
        /// Get default værdi baseret på bekendtgørelse 128 datatype
        /// </summary>
        public static string GetDefaultValue(string dataType, bool isNullable)
        {
            if (isNullable) return null; // Generate xsi:nil="true"

            switch (dataType?.ToUpper())
            {
                default:
                    return "";
            }
        }

        /// <summary>
        /// Normaliser datatype til standard format
        /// </summary>
        public static string NormalizeDataType(string rawType)
        {
            if (string.IsNullOrEmpty(rawType)) return "VARCHAR";

            // Fjern XML namespace prefixes og parenteser
            string cleanType = rawType.Replace("xs:", "").ToUpper();

            // Extract base type før parenteser (f.eks. "VARCHAR(38)" → "VARCHAR")
            int parenIndex = cleanType.IndexOf('(');
            if (parenIndex > 0)
                cleanType = cleanType.Substring(0, parenIndex);

            switch (cleanType)
            {
                case "STRING":
                    return "VARCHAR";
                case "INT":
                case "INTEGER":
                    return "INTEGER";
                case "DECIMAL":
                    return "DECIMAL";
                case "DATE":
                    return "DATE";
                case "TIME":
                    return "TIME";
                case "DATETIME":
                case "TIMESTAMP":
                    return "TIMESTAMP";
                case "BOOLEAN":
                    return "BOOLEAN";
                default:
                    return "VARCHAR"; // Fallback til varchar for ukendte typer
            }
        }

        /// <summary>
        /// Detect datatype fra typeOriginal pattern (bekendtgørelse 128 formater)
        /// </summary>
        public static string DetectDataTypeFromOriginal(string typeOriginal)
        {
            if (string.IsNullOrEmpty(typeOriginal)) return "VARCHAR";

            string lower = typeOriginal.ToLower();

            // Stata patterns
            if (lower.Contains("%") && lower.Contains("f")) return "DECIMAL";
            if (lower.Contains("%") && lower.Contains("s")) return "VARCHAR";
            if (lower.Contains("td") && lower.Contains("yyyy")) return "DATE";
            if (lower.Contains("tc") && lower.Contains("hh")) return "TIME";
            if (lower.Contains("tc") && lower.Contains("yyyy") && lower.Contains("hh")) return "TIMESTAMP";

            // SAS patterns
            if (lower.Contains("$") && lower.Contains(".")) return "VARCHAR";
            if (lower.Contains("f") && lower.Contains(".") && !lower.Contains("$")) return "DECIMAL";
            if (lower.Contains("yymmdd")) return "DATE";
            if (lower.Contains("time")) return "TIME";
            if (lower.Contains("e8601dt")) return "TIMESTAMP";

            // SPSS patterns
            if (lower.StartsWith("a")) return "VARCHAR";
            if (lower.StartsWith("f")) return "DECIMAL";
            if (lower.Contains("sdate")) return "DATE";
            if (lower.Contains("time8")) return "TIME";
            if (lower.Contains("ymdhms") || lower.Contains("datetime")) return "TIMESTAMP";

            // XML patterns
            if (lower.Contains("string")) return "VARCHAR";
            if (lower.Contains("int")) return "INTEGER";
            if (lower.Contains("decimal")) return "DECIMAL";
            if (lower.Contains("date") && !lower.Contains("time")) return "DATE";
            if (lower.Contains("time") && !lower.Contains("date")) return "TIME";
            if (lower.Contains("datetime")) return "TIMESTAMP";

            return "VARCHAR"; // Safe fallback
        }

        /// <summary>
        /// Format værdi baseret på typeOriginal og datatype
        /// </summary>
        public static string FormatValueByOriginalType(string value, string typeOriginal, string dataType)
        {
            if (string.IsNullOrEmpty(value)) return value;

            try
            {
                if (string.IsNullOrEmpty(dataType)) return value.Trim();

                switch (dataType.ToUpper())
                {
                    case "INTEGER":
                        if (long.TryParse(value, out long intVal))
                            return intVal.ToString();
                        break;

                    case "DECIMAL":
                        if (decimal.TryParse(value, out decimal decVal))
                        {
                            return FormatDecimalByTypeOriginal(decVal, typeOriginal);
                        }
                        break;

                    case "DATE":
                        return FormatDateValue(value);

                    case "TIME":
                        return FormatTimeValue(value);

                    case "TIMESTAMP":
                        return FormatTimestampValue(value);

                    case "VARCHAR":
                    default:
                        return value.Trim();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fejl ved formattering af '{value}': {ex.Message}");
            }

            return value; // Fallback til original
        }

        /// <summary>
        /// Format decimal værdi efter typeOriginal specifikation
        /// </summary>
        private static string FormatDecimalByTypeOriginal(decimal value, string typeOriginal)
        {
            try
            {
                // Parse precision og scale fra forskellige formater
                // f9.2, %9.2f, %9.2g
                var match = Regex.Match(typeOriginal, @"(\d+)\.(\d+)");
                if (match.Success)
                {
                    int scale = int.Parse(match.Groups[2].Value);
                    return Math.Round(value, scale).ToString($"F{scale}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fejl ved parsing af TypeOriginal '{typeOriginal}': {ex.Message}");
            }

            return value.ToString();
        }

        /// <summary>
        /// Format dato værdi til standard format
        /// </summary>
        private static string FormatDateValue(string value)
        {
            if (DateTime.TryParse(value, out DateTime dateVal))
                return dateVal.ToString("yyyy-MM-dd");
            return value;
        }

        /// <summary>
        /// Format tid værdi til standard format
        /// </summary>
        private static string FormatTimeValue(string value)
        {
            if (TimeSpan.TryParse(value, out TimeSpan timeVal))
                return timeVal.ToString(@"hh\:mm\:ss");
            return value;
        }

        /// <summary>
        /// Format timestamp værdi til standard format
        /// </summary>
        private static string FormatTimestampValue(string value)
        {
            if (DateTime.TryParse(value, out DateTime timestampVal))
                return timestampVal.ToString("yyyy-MM-ddTHH:mm:ss");
            return value;
        }

        /// <summary>
        /// Safe integer parsing med fallback
        /// </summary>
        public static int ParseIntSafely(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return int.TryParse(value.Trim(), out int result) ? result : 0;
        }
    }
}