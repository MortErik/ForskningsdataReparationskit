using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ForskningsdataReparationskit.Models;
using ForskningsdataReparationskit.Utilities;

namespace ForskningsdataReparationskit.Services
{
    /// <summary>
    /// Interface for progress reporting
    /// </summary>
    public interface IProgressReporter
    {
        void Report(int percentage, string message);
    }

    /// <summary>
    /// Interface for logging operations
    /// </summary>

    public interface ISplitLogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void SaveToFile(string logPath);
        void SaveAllLogs(string outputDirectory, string timestamp, UIDataContainer uiData, TableIndexUpdateResult tableIndexResult);
    }

    /// <summary>
    /// Orchestrator service for XML table splitting operations
    /// Koordinerer split execution workflow mellem services og håndterer file system operationer
    /// </summary>
    public class XMLSplitService
    {
        private readonly TableIndexService tableIndexService;
        private readonly XMLFileGenerationService fileGenerationService;
        private readonly SplitConfigurationService splitConfigService;

        public XMLSplitService()
        {
            tableIndexService = new TableIndexService();
            fileGenerationService = new XMLFileGenerationService();
            splitConfigService = new SplitConfigurationService();
        }

        /// <summary>
        /// Hovedmetode: Udfør split operation med ATOMIC OPERATIONS
        /// Genererer i temp folder først, validerer, derefter flytter til final destination
        /// </summary>
        public SplitResult ExecuteSplit(
            UIDataContainer uiData,
            IProgressReporter progress = null,
            ISplitLogger logger = null,
            string customBasePath = null)
        {
            var result = new SplitResult { StartTime = DateTime.Now };
            string tempDirectory = null;
            string finalOutputDirectory = null;

            try
            {
                // STEP 1: Opret temp directory
                string basePath = customBasePath ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string parentFolder = Path.Combine(basePath, "XML_Table_Splits");

                // Parse original table nummer
                int originalTableNumber = int.Parse(Regex.Match(uiData.OriginalTableEntry.Folder, @"table(\d+)").Groups[1].Value);

                string versionNumber = FileSystemHelper.GetNextVersionNumber(parentFolder, uiData.OriginalTableName);
                string splitFolderName = $"split_{uiData.OriginalTableName}_table{originalTableNumber}_{versionNumber}";

                // TEMP folder for generation
                tempDirectory = Path.Combine(Path.GetTempPath(), $"xml_split_temp_{Guid.NewGuid():N}");
                Directory.CreateDirectory(tempDirectory);

                // FINAL destination
                finalOutputDirectory = Path.Combine(parentFolder, splitFolderName);

                logger?.LogInfo($"Arbejder i temp mappe: {tempDirectory}");
                logger?.LogInfo($"Final destination: {finalOutputDirectory}");

                progress?.Report(5, "Initialiserer opdeling...");

                // LOG OPERATION START
                LogOperationStart(uiData, versionNumber, logger);

                // Parse next table nummer
                int nextTableNumber = GetNextAvailableTableNumber(uiData.TableIndexPath);

                progress?.Report(10, "Genererer split tabeller i temp...");

                // STEP 2: Generer ALLE filer i temp directory
                for (int i = 0; i < uiData.Tables.Count; i++)
                {
                    int currentTableNumber = (i == 0) ? originalTableNumber : nextTableNumber + (i - 1);

                    //int progressPercent = 10 + (int)((i / (double)uiData.Tables.Count) * 65);
                    //progress?.Report(progressPercent, $"Generer tabel {i + 1} af {uiData.Tables.Count}...");

                    fileGenerationService.GenerateTableFiles(uiData.Tables[i], tempDirectory, uiData, currentTableNumber);
                }

                progress?.Report(75, "Opdaterer tableIndex.xml...");

                // STEP 3: Opdater tableIndex.xml
                TableIndexUpdateResult tableIndexResult = null;
                if (!string.IsNullOrEmpty(uiData.TableIndexPath))
                {
                    logger?.LogInfo("Starter opdatering af tableIndex.xml");
                    try
                    {
                        tableIndexResult = tableIndexService.GenerateUpdatedTableIndex(
                            uiData.Tables,
                            tempDirectory,
                            uiData,
                            originalTableNumber,
                            nextTableNumber);

                        logger?.LogInfo("Afsluttet opdatering af tableIndex.xml");
                        logger?.LogInfo("");
                        logger?.LogInfo("=== TABLEINDEX TRANSFORMATION DETAILS ===");
                        logger?.LogInfo(tableIndexResult.TechnicalLog);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError($"Fejl ved opdatering af tableIndex.xml: {ex.Message}");
                        throw; // Re-throw for rollback
                    }
                }

                progress?.Report(85, "Validerer output...");

                // STEP 4: Validér output før commit
                logger?.LogInfo("Starter validering af genererede filer...");
                ValidateSplitOutput(tempDirectory, uiData, logger);
                logger?.LogInfo("Validering bestået - alle filer er korekte");

                progress?.Report(90, "Flytter filer til final destination...");

                // STEP 5: Opret final directory og flyt alt
                Directory.CreateDirectory(Path.GetDirectoryName(finalOutputDirectory));

                // Hvis destination eksisterer, find næste ledige version
                int retryCount = 0;
                while (Directory.Exists(finalOutputDirectory) && retryCount < 10)
                {
                    logger?.LogWarning($"Destination already exists: {finalOutputDirectory}");
                    logger?.LogInfo("Finding next available version number...");

                    // Increment version
                    versionNumber = IncrementVersionNumber(versionNumber);
                    splitFolderName = $"split_{uiData.OriginalTableName}_table{originalTableNumber}_{versionNumber}";
                    finalOutputDirectory = Path.Combine(parentFolder, splitFolderName);

                    logger?.LogInfo($"Trying new version: {versionNumber}");
                    retryCount++;
                }

                if (Directory.Exists(finalOutputDirectory))
                {
                    throw new InvalidOperationException("Could not find available version number after 10 attempts");
                }

                Directory.Move(tempDirectory, finalOutputDirectory);
                tempDirectory = null;

                // STEP 6: Populate result
                result.Success = true;
                result.OutputDirectory = finalOutputDirectory;
                result.TablesGenerated = uiData.Tables.Count;
                result.TotalRows = uiData.TotalRows;
                result.GeneratedFiles = Directory.GetFiles(finalOutputDirectory, "*", SearchOption.AllDirectories).ToList();
                result.EndTime = DateTime.Now;

                // LOG OPERATION END og gem log filer
                progress?.Report(95, "Genererer log filer...");
                LogOperationEnd(finalOutputDirectory, uiData, logger, tableIndexResult);

                progress?.Report(100, "Opdeling fuldført");
                return result;
            }
            catch (Exception ex)
            {
                logger?.LogError($"KRITISK FEJL: {ex.Message}");
                logger?.LogError($"Stack trace: {ex.StackTrace}");

                // Slet temp directory hvis det stadig eksisterer (rollback)
                if (tempDirectory != null && Directory.Exists(tempDirectory))
                {
                    try
                    {
                        Directory.Delete(tempDirectory, true);
                        logger?.LogInfo("Temp directory slettet efter fejl (rollback)");
                    }
                    catch (Exception cleanupEx)
                    {
                        logger?.LogWarning($"Kunne ikke slette temp directory: {cleanupEx.Message}");
                    }
                }

                result.Success = false;
                result.ErrorMessage = ex.Message;
                result.EndTime = DateTime.Now;
                return result;
            }
        }

        private string IncrementVersionNumber(string currentVersion)
        {
            // Parse "v1.0" → 1.0
            var versionStr = currentVersion.Replace("v", "");
            if (double.TryParse(versionStr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double version))
            {
                version += 0.1;
                return $"v{version.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}";
            }
            return "v1.0";
        }

        /// <summary>
        /// Validér at split output er korrekt før commit
        /// </summary>
        private void ValidateSplitOutput(string outputDirectory, UIDataContainer uiData, ISplitLogger logger)
        {
            var errors = new List<string>();

            // 1. Find alle split XML filer (EXCLUDE tableIndex)
            var xmlFiles = Directory.GetFiles(outputDirectory, "*.xml", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f).StartsWith("table") &&
                           !Path.GetFileName(f).Contains("tableIndex"))
                .ToList();

            // 2. Tjek at alle forventede XML filer findes
            if (xmlFiles.Count < uiData.Tables.Count)
            {
                errors.Add($"Forventede {uiData.Tables.Count} split XML filer, fandt kun {xmlFiles.Count}");
            }

            // 3. Tjek at tableIndex findes
            string tableIndexPath = Path.Combine(outputDirectory, "tableIndex_updated.xml");
            if (!File.Exists(tableIndexPath))
            {
                errors.Add("tableIndex_updated.xml mangler");
            }
            else
            {
                // 4. Tjek at tableIndex er valid XML
                try
                {
                    var doc = XDocument.Load(tableIndexPath);

                    // 5. Tjek at alle split tabeller er i tableIndex
                    var ns = doc.Root.GetDefaultNamespace();
                    foreach (var table in uiData.Tables)
                    {
                        var tableElement = doc.Descendants(ns + "table")
                            .FirstOrDefault(t => t.Element(ns + "name")?.Value == table.TableName);

                        if (tableElement == null)
                        {
                            errors.Add($"Split tabel '{table.TableName}' mangler i tableIndex");
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"tableIndex_updated.xml er ikke valid XML: {ex.Message}");
                }
            }

            // 6. Validér row counts og struktur i SPLIT filer
            foreach (var xmlFile in xmlFiles)
            {
                try
                {
                    int rowCount = 0;
                    bool hasTableRoot = false;

                    using (var reader = XmlReader.Create(xmlFile))
                    {
                        // Tjek root element
                        reader.MoveToContent();
                        if (reader.Name == "table")
                        {
                            hasTableRoot = true;
                        }

                        // Count rows
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
                            {
                                rowCount++;
                            }
                        }
                    }

                    // Valider struktur
                    if (!hasTableRoot)
                    {
                        errors.Add($"{Path.GetFileName(xmlFile)}: Invalid root element (expected <table>)");
                    }

                    // Valider row count
                    if (rowCount != uiData.TotalRows)
                    {
                        errors.Add($"{Path.GetFileName(xmlFile)}: Forventede {uiData.TotalRows} rows, fandt {rowCount}");
                    }
                    else if (hasTableRoot)
                    {
                        logger?.LogInfo($"{Path.GetFileName(xmlFile)}: Validation OK ({rowCount} rows, valid structure)");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(xmlFile)}: Kunne ikke validere - {ex.Message}");
                }
            }

            if (errors.Count > 0)
            {
                logger?.LogError("VALIDERING FEJLEDE:");
                foreach (var error in errors)
                {
                    logger?.LogError($"  - {error}");
                }

                throw new InvalidOperationException(
                    $"Split output validering fejlede med {errors.Count} fejl:\n" +
                    string.Join("\n", errors)
                );
            }

            logger?.LogInfo($"Validering OK - {uiData.Tables.Count} tabeller, {uiData.TotalRows} rows per tabel");
        }

        /// <summary>
        /// Log operation start med detaljeret konfiguration
        /// </summary>
        private void LogOperationStart(UIDataContainer uiData, string versionNumber, ISplitLogger logger)
        {
            if (logger == null) return;

            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            logger.LogInfo("==============================================");
            logger.LogInfo("         XML TABLE SPLIT OPERATION");
            logger.LogInfo("==============================================");
            logger.LogInfo($"Start tid: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            logger.LogInfo($"Original tabel: {uiData.OriginalTableName}");
            logger.LogInfo($"Total rækker: {uiData.TotalRows:N0}");
            logger.LogInfo($"Total kolonner: {uiData.AllColumns.Count}");
            logger.LogInfo($"Split konfiguration: {uiData.Tables.Count} tabeller");
            logger.LogInfo($"PK Type: {(pkInfo.IsComposite ? "Composite" : "Single")}");
            logger.LogInfo($"PK Kolonner: {string.Join(", ", pkColumns)} ({pkColumns.Count} total)");
            logger.LogInfo($"Available capacity per split: {uiData.GetAvailableDataColumnsPerSplit()} kolonner");
            logger.LogInfo($"Version: {versionNumber}");

            logger.LogInfo("");
            logger.LogInfo("REUNION VIEW KONFIGURATION:");
            logger.LogInfo($"View navn: RA_Samling_af_{uiData.OriginalTableName}");
            logger.LogInfo($"Split tabeller: {string.Join(", ", uiData.Tables.Select(t => t.TableName))}");
            logger.LogInfo($"JOIN på: {string.Join(", ", pkColumns)}");

            logger.LogInfo("");
            logger.LogInfo("KOLONNE MAPPING:");
            logger.LogInfo("=========================================");

            foreach (var table in uiData.Tables)
            {
                logger.LogInfo($"");
                logger.LogInfo($"Tabel: {table.TableName}");

                var dataColumns = table.Columns.Where(c => !pkColumns.Contains(c.Name)).OrderBy(c => c.Position).ToList();
                var pkColumnsInSplit = table.Columns.Where(c => pkColumns.Contains(c.Name)).OrderBy(c => c.Position).ToList();

                // Beregn korrekt kolonne placering baseret på split index
                if (table.SplitIndex == 1)
                {
                    // Split1: Behold original rækkefølge - sorter ALLE kolonner efter position
                    var allColumns = table.Columns.OrderBy(c => c.Position).ToList();
                    //int xmlColumnCounter = 1;

                    logger.LogInfo($"  PK Kolonner ({pkColumnsInSplit.Count}) - Placering i {table.TableName}:");
                    foreach (var pkCol in pkColumnsInSplit)
                    {
                        int positionInSplit = allColumns.IndexOf(pkCol) + 1;
                        logger.LogInfo($"    - {pkCol.Name} ({pkCol.DataType}) - placeret på c{positionInSplit}");
                    }

                    logger.LogInfo($"  Data Kolonner ({dataColumns.Count}):");
                    logger.LogInfo($"    Fra         Til         Navn                Datatype");
                    logger.LogInfo($"    {new string('-', 60)}");

                    foreach (var col in dataColumns)
                    {
                        int positionInSplit = allColumns.IndexOf(col) + 1;
                        string colName = col.Name.Length > 19 ? col.Name.Substring(0, 16) + "..." : col.Name;
                        logger.LogInfo($"    {col.ColumnID,-11} c{positionInSplit,-10} {colName,-19} {col.DataType}");
                    }
                }
                else
                {
                    // Split2+: Data kolonner først, PK kolonner bagerst
                    logger.LogInfo($"  PK Kolonner ({pkColumnsInSplit.Count}) - Placering i {table.TableName}:");

                    int pkStartPosition = dataColumns.Count + 1;
                    int pkCounter = pkStartPosition;
                    foreach (var pkCol in pkColumnsInSplit)
                    {
                        logger.LogInfo($"    - {pkCol.Name} ({pkCol.DataType}) - placeret på c{pkCounter}");
                        pkCounter++;
                    }

                    logger.LogInfo($"  Data Kolonner ({dataColumns.Count}):");
                    logger.LogInfo($"    Fra         Til         Navn                Datatype");
                    logger.LogInfo($"    {new string('-', 60)}");

                    int toCounter = 1;
                    foreach (var col in dataColumns)
                    {
                        string colName = col.Name.Length > 19 ? col.Name.Substring(0, 16) + "..." : col.Name;
                        logger.LogInfo($"    {col.ColumnID,-11} c{toCounter,-10} {colName,-19} {col.DataType}");
                        toCounter++;
                    }
                }

                logger.LogInfo($"  → Total kolonner: {table.Columns.Count} ({pkColumnsInSplit.Count} PK + {dataColumns.Count} data)");
            }
            logger.LogInfo("");
        }

        /// <summary>
        /// Log operation end og gem alle log filer via logger
        /// </summary>
        private void LogOperationEnd(string outputDirectory, UIDataContainer uiData, ISplitLogger logger, TableIndexUpdateResult tableIndexResult)
        {
            if (logger == null) return;

            // Generer fil oversigt
            var generatedFiles = Directory.GetFiles(outputDirectory, "*", SearchOption.AllDirectories);
            logger.LogInfo("");
            logger.LogInfo($"GENEREREDE FILER ({generatedFiles.Length} total):");
            logger.LogInfo("==========================================");

            var xmlFiles = generatedFiles.Where(f => f.EndsWith(".xml")).OrderBy(f => f).ToList();
            var otherFiles = generatedFiles.Where(f => !f.EndsWith(".xml")).OrderBy(f => f).ToList();

            logger.LogInfo("XML DATA FILER:");
            foreach (var file in xmlFiles)
            {
                var fileInfo = new FileInfo(file);
                logger.LogInfo($"  - {Path.GetFileName(file)} ({fileInfo.Length:N0} bytes)");
            }

            if (otherFiles.Count > 0)
            {
                logger.LogInfo("ANDRE FILER:");
                foreach (var file in otherFiles)
                {
                    var fileInfo = new FileInfo(file);
                    logger.LogInfo($"  - {Path.GetFileName(file)} ({fileInfo.Length:N0} bytes)");
                }
            }

            logger.LogInfo("");
            logger.LogInfo($"Slut tid: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            logger.LogInfo("==============================================");
            logger.LogInfo("            OPERATION FULDFØRT ");
            logger.LogInfo("==============================================");

            // GEM ALLE LOG FILER (delegeret til logger med tableIndex result)
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            logger.SaveAllLogs(outputDirectory, timestamp, uiData, tableIndexResult);
        }

        /// <summary>
        /// Find næste tilgængelige table nummer fra tableIndex.xml
        /// </summary>
        private int GetNextAvailableTableNumber(string tableIndexPath)
        {
            var doc = XDocument.Load(tableIndexPath);
            var ns = doc.Root.GetDefaultNamespace();

            var highestNumber = doc.Descendants(ns + "folder")
                .Select(f => Regex.Match(f.Value, @"table(\d+)$"))
                .Where(m => m.Success)
                .Select(m => int.Parse(m.Groups[1].Value))
                .DefaultIfEmpty(0)
                .Max();

            return highestNumber + 1;
        }
    }
}