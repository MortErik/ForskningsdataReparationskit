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
    }
}
