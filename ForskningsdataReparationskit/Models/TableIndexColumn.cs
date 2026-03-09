using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForskningsdataReparationskit.Models
{
    /// <summary>
    /// TableIndex kolonne med fuld metadata
    /// </summary>
    public class TableIndexColumn
    {
        public string Name { get; set; }
        public string ColumnID { get; set; }
        public string DataType { get; set; }        // XML datatype: VARCHAR, INTEGER, DECIMAL
        public string TypeOriginal { get; set; }    // Original format specifier
        public bool IsNullable { get; set; }
        public string Description { get; set; }
        public int Position { get; set; }

        /// <summary>
        /// Display med alle metadata
        /// </summary>
        public string DisplayText => $"{ColumnID}: {Name} ({DataType}, {(IsNullable ? "nullable" : "not null")})";

        /// <summary>
        /// Get default værdi baseret på datatype
        /// Kun PK og date/time kolonner får værdier, resten står tomt
        /// </summary>
        public string GetDefaultValue()
        {
            switch (DataType?.ToUpper())
            {
                case "DATE":
                    return "9999-12-31";
                case "TIME":
                    return "23:59:59";
                case "TIMESTAMP":
                    return "9999-12-31T23:59:59";
                default:
                    // Alle andre typer (INTEGER, DECIMAL, BOOLEAN, VARCHAR osv) → tom værdi
                    return "";
            }
        }
    }
}
