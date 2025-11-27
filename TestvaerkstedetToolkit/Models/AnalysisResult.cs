using System;

namespace TestvaerkstedetToolkit.Models
{
    /// <summary>
    /// Resultat fra primary key analyse
    /// Indeholder statistik om PK unikhed, null værdier, og duplikater
    /// </summary>
    public class AnalysisResult
    {
        /// <summary>
        /// Antal unikke PK kombinationer
        /// </summary>
        public int UniqueCount { get; set; }

        /// <summary>
        /// Total antal rækker analyseret
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Antal rækker med null værdier i PK
        /// </summary>
        public int NullCount { get; set; }

        /// <summary>
        /// Angiver om PK kombinationen er perfekt unik (ingen null, ingen duplikater)
        /// </summary>
        public bool IsUnique => UniqueCount == TotalCount && NullCount == 0;

        /// <summary>
        /// Angiver om der er null værdier i PK
        /// </summary>
        public bool HasNulls => NullCount > 0;

        /// <summary>
        /// Antal duplikat kombinationer (rækker der ikke er unikke og ikke null)
        /// </summary>
        public int DuplicateCount => TotalCount - UniqueCount - NullCount;

        /// <summary>
        /// Angiver om PK er næsten god (unik men med null værdier)
        /// </summary>
        public bool IsNearlyGood => UniqueCount + NullCount == TotalCount && NullCount > 0;

        /// <summary>
        /// Generisk status message
        /// </summary>
        public string StatusMessage
        {
            get
            {
                if (IsUnique)
                    return "PERFEKT: Primærnøglekombinationen er unik og komplet";
                else if (IsNearlyGood)
                    return "NÆSTEN GOD: Kombinationen er unik men har null værdier";
                else
                    return $"IKKE EGNET: Kombinationen er ikke unik ({DuplicateCount:N0} duplikater)";
            }
        }
    }
}
