using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForskningsdataReparationskit.Models
{
    /// <summary>
    /// XMLColumn med TableIndex metadata
    /// </summary>
    public class XMLColumn
    {
        public string Name { get; set; }           // Fra TableIndex: "f_id"
        public string ColumnID { get; set; }      // Fra TableIndex: "c1"
        public string DataType { get; set; }      // Fra TableIndex: "DECIMAL"
        public string TypeOriginal { get; set; }  // Fra TableIndex: "%9.0g"
        public bool IsNullable { get; set; }      // Fra TableIndex
        public string Description { get; set; }   // Fra TableIndex
        public int Position { get; set; }         // Beregnet position
        public int colNr = 0;
        public string dataType = "";
        public string nillable = "";

        /// <summary>
        /// Extract column number fra ColumnID (c1, c2, etc.)
        /// </summary>
        public int ExtractColumnNumber()
        {
            if (string.IsNullOrEmpty(ColumnID) || !ColumnID.StartsWith("c"))
                return 0;

            if (int.TryParse(ColumnID.Substring(1), out int number))
                return number;

            return 0;
        }

        public bool IsNillable
        {
            get { return nillable?.ToLower() == "true"; }
            set { nillable = value ? "true" : "false"; }
        }
    }
}
