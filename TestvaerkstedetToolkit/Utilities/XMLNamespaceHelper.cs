using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace TestvaerkstedetToolkit.Utilities
{
    /// <summary>
    /// Utility klasse til håndtering af XML namespaces
    /// Indeholder metoder til namespace detection og generering for SIARD format
    /// </summary>
    public static class XMLNamespaceHelper
    {
        /// <summary>
        /// Standard SIARD namespace base
        /// </summary>
        private const string SIARD_NAMESPACE_BASE = "http://www.sa.dk/xmlns/siard/1.0/schema0/";

        /// <summary>
        /// Detect original namespace fra XML fil
        /// </summary>
        public static string DetectNamespace(string xmlPath)
        {
            // Default namespace hvis detection fejler
            string defaultNamespace = SIARD_NAMESPACE_BASE;

            if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
                return defaultNamespace;

            try
            {
                using (var reader = XmlReader.Create(xmlPath))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "table")
                        {
                            string xmlns = reader.GetAttribute("xmlns");
                            if (!string.IsNullOrEmpty(xmlns))
                            {
                                return xmlns;
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fejl ved detection af namespace: {ex.Message}");
            }

            return defaultNamespace;
        }

        /// <summary>
        /// Generer namespace for en specifik tabel nummer
        /// </summary>
        public static string GenerateTableNamespace(int tableNumber)
        {
            return $"{SIARD_NAMESPACE_BASE}table{tableNumber}.xsd";
        }

        /// <summary>
        /// Returner standard SIARD namespace som XNamespace
        /// </summary>
        public static XNamespace GetSIARDNamespace()
        {
            return XNamespace.Get(SIARD_NAMESPACE_BASE);
        }
    }
}
