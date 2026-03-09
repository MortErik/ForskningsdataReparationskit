using System;
using System.Collections.Generic;

namespace ForskningsdataReparationskit.Models
{
    /// <summary>
    /// Resultat fra en split operation
    /// Indeholder information om succes, output lokation, og genererede filer
    /// </summary>
    public class SplitResult
    {
        /// <summary>
        /// Angiver om split operationen lykkedes
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Output directory hvor split tabeller blev genereret
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Fejlbesked hvis operation fejlede
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Antal tabeller der blev genereret
        /// </summary>
        public int TablesGenerated { get; set; }

        /// <summary>
        /// Total antal rækker per tabel
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// Liste over alle genererede filer (XML, tableIndex, logs)
        /// </summary>
        public List<string> GeneratedFiles { get; set; } = new List<string>();

        /// <summary>
        /// Tidspunkt for operation start
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Tidspunkt for operation afslutning
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Total varighed af operationen
        /// </summary>
        public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : TimeSpan.Zero;
    }
}
