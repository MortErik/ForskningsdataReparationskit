using System;
using System.IO;
using System.Xml;

namespace ForskningsdataReparationskit.Utilities
{
    /// <summary>
    /// Utility klasse til XML operationer
    /// Indeholder hjælpemetoder til XML parsing og analyse
    /// </summary>
    public static class XMLHelper
    {
        /// <summary>
        /// Count rækker fra XML fil
        /// </summary>
        public static int CountRowsFromXML(string xmlPath)
        {
            int totalRows = 0;

            if (string.IsNullOrEmpty(xmlPath) || !File.Exists(xmlPath))
                return totalRows;

            try
            {
                using (var reader = XmlReader.Create(xmlPath))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
                        {
                            totalRows++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fejl ved row counting: {ex.Message}");
                totalRows = 0;
            }

            return totalRows;
        }
    }
}
