using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ForskningsdataReparationskit.Utilities
{
    /// <summary>
    /// Utility klasse til file system operationer
    /// Indeholder sikre metoder til directory håndtering og version management
    /// </summary>
    public static class FileSystemHelper
    {
        /// <summary>
        /// Sikker directory åbning med fallbacks
        /// </summary>
        public static void OpenDirectorySafely(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    MessageBox.Show($"Mappe eksisterer ikke: {directoryPath}");
                    return;
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = directoryPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception)
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"\"{directoryPath}\"");
                }
                catch (Exception)
                {
                    try
                    {
                        Clipboard.SetText(directoryPath);
                        MessageBox.Show($"Kunne ikke åbne mappe automatisk.\n\n" +
                                      $"Sti kopieret til clipboard:\n{directoryPath}\n\n" +
                                      $"Indsæt i File Explorer adressefelt.",
                                      "Mappe Åbning Fejlede", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        MessageBox.Show($"Kunne ikke åbne mappe:\n{directoryPath}\n\n" +
                                      $"Åbn manuelt i File Explorer.",
                                      "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        /// <summary>
        /// Find næste version nummer baseret på eksisterende mapper
        /// </summary>
        public static string GetNextVersionNumber(string parentFolder, string tableName)
        {
            try
            {
                if (!Directory.Exists(parentFolder))
                    return "v1.0";

                var existingFolders = Directory.GetDirectories(parentFolder)
                    .Where(dir =>
                    {
                        var name = Path.GetFileName(dir);
                        // Match: split_{tableName}_table{nummer}_v{version}
                        return Regex.IsMatch(name, $@"^split_{Regex.Escape(tableName)}_table\d+_v[\d\.]+$");
                    })
                    .ToList();

                if (existingFolders.Count == 0)
                    return "v1.0";

                // Find højeste version nummer
                double maxVersion = 0;
                foreach (var folder in existingFolders)
                {
                    var folderName = Path.GetFileName(folder);
                    var versionStart = folderName.IndexOf("_v") + 2;
                    var versionEnd = folderName.IndexOf("_", versionStart);

                    if (versionStart > 1 && versionEnd > versionStart)
                    {
                        var versionStr = folderName.Substring(versionStart, versionEnd - versionStart);
                        if (double.TryParse(versionStr, out double version))
                        {
                            maxVersion = Math.Max(maxVersion, version);
                        }
                    }
                }

                double newVersion = maxVersion + 0.1;
                return $"v{newVersion.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}";  // Bruger punktum
            }
            catch
            {
                return "v1.0";
            }
        }

        /// <summary>
        /// Sikrer at directory eksisterer, opretter hvis nødvendigt
        /// Wrapper for Directory.CreateDirectory med implicit error handling
        /// </summary>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
