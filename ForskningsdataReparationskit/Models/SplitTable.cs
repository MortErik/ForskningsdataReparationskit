using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForskningsdataReparationskit.Models
{
    /// <summary>
    /// SplitTable med TableIndex metadata og naming strategy
    /// </summary>
    public class SplitTable
    {
        public string Name { get; set; }
        public string TableName { get; set; }      // "Forældre" (første), "Forældre_2" (andre)
        public string FolderName { get; set; }     // "table6_forældre_v1", "table6_forældre_2_v1"  
        public string OriginalFolder { get; set; } // "table6"
        public string OriginalName { get; set; }    // "Forældre" (for folder reference)
        public string OriginalFolderName { get; set; }  // "table6"
        public int SplitIndex { get; set; }             // 1, 2, 3
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
        public bool IsFirstSplit { get; set; }  // true for første split (erstatter original)
        public List<XMLColumn> Columns { get; set; } = new List<XMLColumn>();
        public int ColumnCount => EndColumn - StartColumn + 1 + 1; // +1 for primærnøgle
        public string ColumnRange => $"c{StartColumn}-c{EndColumn} + PK";

        /// <summary>
        /// Generate table name baseret på naming strategy (Option A)
        /// </summary>
        public static string GenerateTableName(string originalName, int splitIndex)
        {
            // GAMMEL: return splitIndex == 1 ? originalName : $"{originalName}_{splitIndex}";
            // NY: Altid brug suffix for alle split tabeller
            return $"{originalName}_{splitIndex}";
        }


        public string GeneratedFolderName
        {
            get
            {
                return SplitIndex == 1 ? OriginalFolderName : $"{OriginalFolderName}_{SplitIndex}";
            }
        }
    }
}
