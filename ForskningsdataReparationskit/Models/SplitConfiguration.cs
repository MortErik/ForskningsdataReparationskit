using System;
using System.Collections.Generic;
using System.Linq;

namespace ForskningsdataReparationskit.Models
{
    /// <summary>
    /// Konfiguration og validering af split punkter
    /// Indeholder split points, validerings resultater, og metadata
    /// </summary>
    public class SplitConfiguration
    {
        /// <summary>
        /// Liste over split punkter (kolonne positioner hvor tabellen skal splittes)
        /// </summary>
        public List<int> SplitPoints { get; set; } = new List<int>();

        /// <summary>
        /// Maksimalt antal kolonner per tabel (inklusiv PK)
        /// </summary>
        public int MaxColumnsPerTable { get; set; } = 950;

        /// <summary>
        /// Validerings fejl beskeder
        /// </summary>
        public List<string> ValidationErrors { get; set; } = new List<string>();

        /// <summary>
        /// Validerings advarsler
        /// </summary>
        public List<string> ValidationWarnings { get; set; } = new List<string>();

        /// <summary>
        /// Angiver om konfigurationen er valid
        /// </summary>
        public bool IsValid => ValidationErrors.Count == 0;

        /// <summary>
        /// Angiver om konfigurationen har advarsler
        /// </summary>
        public bool HasWarnings => ValidationWarnings.Count > 0;

        /// <summary>
        /// Samlet validerings besked
        /// </summary>
        public string ValidationMessage
        {
            get
            {
                if (IsValid && !HasWarnings)
                    return "Split konfiguration er valid";
                else if (IsValid && HasWarnings)
                    return $"Valid med {ValidationWarnings.Count} advarsel(er)";
                else
                    return $"{ValidationErrors.Count} fejl fundet";
            }
        }

        /// <summary>
        /// Detaljeret validerings summary
        /// </summary>
        public string GetDetailedSummary()
        {
            var summary = new System.Text.StringBuilder();

            if (ValidationErrors.Count > 0)
            {
                summary.AppendLine("FEJL:");
                foreach (var error in ValidationErrors)
                    summary.AppendLine($"  - {error}");
            }

            if (ValidationWarnings.Count > 0)
            {
                if (summary.Length > 0) summary.AppendLine();
                summary.AppendLine("ADVARSLER:");
                foreach (var warning in ValidationWarnings)
                    summary.AppendLine($"  - {warning}");
            }

            return summary.ToString();
        }

        /// <summary>
        /// Forventet antal tabeller efter split
        /// </summary>
        public int ExpectedTableCount => SplitPoints.Count + 1;
    }
}
