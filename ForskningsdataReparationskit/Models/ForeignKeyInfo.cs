using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForskningsdataReparationskit.Models
{
    /// <summary>
    /// Foreign Key information fra TableIndex
    /// </summary>
    public class ForeignKeyInfo
    {
        public string Name { get; set; }
        public string ReferencedTable { get; set; }
        public string Column { get; set; }
        public string ColumnName { get; set; }        // Kolonne der refererer
        public string Referenced { get; set; }
        public string ReferencedColumn { get; set; }  // Kolonne der refereres til
    }
}
