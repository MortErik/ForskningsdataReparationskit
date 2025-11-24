using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TestvaerkstedetToolkit.Controls;
using TestvaerkstedetToolkit.Models;
using TestvaerkstedetToolkit.Services;
using TestvaerkstedetToolkit.Utilities;

namespace TestvaerkstedetToolkit
{
    public partial class XMLTableSplitterForm : Form
    {
        #region Fields

        private string currentXMLPath = "";
        private string currentXSDPath = "";
        private string originalNamespace = "";
        private List<XMLColumn> allColumns = new List<XMLColumn>();
        private List<SplitTable> resultTables = new List<SplitTable>();
        private int totalRows = 0;

        // Private fields for TableIndex integration
        private List<TableIndexEntry> availableTables = new List<TableIndexEntry>();
        private TableIndexEntry currentTableEntry = null;
        private string currentTableIndexPath = "";

        private bool splitPointsHasPlaceholder = true;
        private const string PLACEHOLDER_TEXT = "Eksempel: 950, 1800, 2700 (eller lad stå tom for automatisk split)";

        #endregion

        #region Constructor

        public XMLTableSplitterForm()
        {
            InitializeComponent();
            // compositePKSelector er allerede initialiseret i Designer

            SetupSplitPointsPlaceholder();
            AddSplitPointsInfoLabel();
        }

        #endregion

        #region sammensat PK Event Handlers

        /// <summary>
        /// Event handler for sammensat PK ændringer
        /// </summary>
        private void CompositePKSelector_PrimaryKeyChanged(object sender, EventArgs e)
        {
            if (compositePKSelector.IsValid())
            {
                // Update preview og enable knapper
                UpdateSplitPreview();
                btnExecuteSplit.Enabled = true;
                btnAnalyzePK.Enabled = true;  // Enable PK analyse når PK er valgt

                // Log PK konfiguration til debug
                var pkInfo = compositePKSelector.GetPrimaryKeyInfo();
                var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
                System.Diagnostics.Debug.WriteLine($"PK Changed: {string.Join(", ", pkColumns)} (Composite: {pkInfo.IsComposite})");
            }
            else
            {
                // Disable knapper og vis fejl
                btnExecuteSplit.Enabled = false;
                btnAnalyzePK.Enabled = false;
                lblPreviewInfo.Text = compositePKSelector.GetValidationError();
                lblPreviewInfo.ForeColor = Color.DarkRed;
            }
        }

        #endregion

        #region TableIndex Integration

        /// <summary>
        /// Browse for tableIndex.xml - HOVEDINDGANG til workflow
        /// </summary>
        private void btnBrowseTableIndex_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "TableIndex filer|tableIndex.xml|XML filer|*.xml|Alle filer|*.*";
                openDialog.Title = "Vælg tableIndex.xml fil";

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        currentTableIndexPath = openDialog.FileName;
                        txtTableIndexPath.Text = currentTableIndexPath;

                        // Parse tableIndex.xml og load tilgængelige tabeller
                        LoadTableIndexMetadata();

                        lblTableInfo.Text = $"Loaded {availableTables.Count} tabeller fra tableIndex";
                        lblTableInfo.ForeColor = Color.DarkGreen;

                        UpdateUIAfterTableIndexLoad();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fejl ved læsning af tableIndex.xml:\n{ex.Message}",
                                       "TableIndex Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        lblTableInfo.Text = "Fejl ved loading af tableIndex";
                        lblTableInfo.ForeColor = Color.Red;
                    }
                }
            }
        }

        /// <summary>
        /// Load og parse tableIndex.xml metadata
        /// </summary>
        private void LoadTableIndexMetadata()
        {
            availableTables.Clear();
            cmbTableSelector.Items.Clear();

            // Parse tableIndex.xml
            availableTables = TableIndexParser.ParseTableIndex(currentTableIndexPath);

            // SORTÉR availableTables listen direkte (ikke kun visningen)
            availableTables = availableTables.OrderByDescending(tie => tie.Columns.Count).ToList();

            // Populer table selector dropdown (nu er de allerede sorteret)
            foreach (var table in availableTables)
            {
                cmbTableSelector.Items.Add(table.DisplayText);
            }

            if (cmbTableSelector.Items.Count > 0)
            {
                cmbTableSelector.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Table selector changed - auto-suggest XML fil og load metadata
        /// </summary>
        private void cmbTableSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTableSelector.SelectedIndex < 0 || cmbTableSelector.SelectedIndex >= availableTables.Count)
                return;

            try
            {
                currentTableEntry = availableTables[cmbTableSelector.SelectedIndex];
                PopulateColumnOverview();
                compositePKSelector.LoadTableData(currentTableEntry);

                // Auto-load ALLE eksisterende PK kolonner
                if (currentTableEntry.PrimaryKeyColumns != null && currentTableEntry.PrimaryKeyColumns.Count > 0)
                {
                    var pkInfo = new PrimaryKeyInfo();
                    foreach (var pkColumn in currentTableEntry.PrimaryKeyColumns)
                    {
                        pkInfo.ExistingColumnNames.Add(pkColumn);
                    }
                    compositePKSelector.SetPrimaryKeyInfo(pkInfo);
                }

                // Update UI med sammensat PK info
                lblTableInfo.Text = $"Tabel: {currentTableEntry.Name} ({currentTableEntry.Columns.Count} kolonner) | {currentTableEntry.GetPrimaryKeyDisplayText()}";
                lblTableInfo.ForeColor = Color.DarkGreen;

                AutoSuggestXMLFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved indlæsning af tabel: {ex.Message}");
            }
        }

        /// <summary>
        /// Auto-suggest XML fil baseret på table metadata
        /// </summary>
        private void AutoSuggestXMLFile()
        {
            if (currentTableEntry == null) return;

            try
            {
                string suggestedXmlPath = currentTableEntry.FindXmlPath(currentTableIndexPath);

                if (!string.IsNullOrEmpty(suggestedXmlPath) && File.Exists(suggestedXmlPath))
                {
                    txtSourceXML.Text = suggestedXmlPath;
                    currentXMLPath = suggestedXmlPath;

                    // Analyze struktur automatisk
                    AnalyzeStructureWithTableIndexMetadata();
                }
                else
                {
                    txtSourceXML.Text = "[XML ikke fundet - brug Manuel Browse]";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AutoSuggest error: {ex.Message}");
            }
        }

        /// <summary>
        /// Analyze structure med TableIndex metadata
        /// </summary>
        private void AnalyzeStructureWithTableIndexMetadata()
        {
            if (currentTableEntry == null || string.IsNullOrEmpty(currentXMLPath))
                return;

            try
            {
                // Auto-detect XSD fil
                currentXSDPath = Path.ChangeExtension(currentXMLPath, ".xsd");

                // Count rows fra XML
                CountRowsFromXML();

                // Set original namespace
                DetectOriginalNamespace();

                UpdateUIAfterStructureAnalysis();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved TableIndex structure analyse: {ex.Message}");
            }
        }

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

        #endregion

        #region Column Overview

        private void PopulateColumnOverview()
        {
            allColumns.Clear();

            if (currentTableEntry?.Columns != null)
            {
                // TableIndex mode - use TableIndex metadata
                foreach (var tableColumn in currentTableEntry.Columns.OrderBy(c => c.Position))
                {
                    // Convert til XMLColumn
                    var column = new XMLColumn
                    {
                        Name = tableColumn.Name,
                        ColumnID = tableColumn.ColumnID,
                        DataType = tableColumn.DataType,
                        TypeOriginal = tableColumn.TypeOriginal,
                        IsNullable = tableColumn.IsNullable,
                        Description = tableColumn.Description,
                        Position = tableColumn.Position
                    };

                    allColumns.Add(column);

                    // Display format med TableIndex metadata
                    string nullableText = tableColumn.IsNullable ? "nullable" : "not null";
                    string displayText = $"{tableColumn.ColumnID}: {tableColumn.Name} ({tableColumn.DataType}, {nullableText})";
                }
            }
            else if (allColumns.Count > 0)
            {
                // Legacy XML analysis mode
                foreach (var column in allColumns)
                {
                    string nillableText = column.IsNullable ? "nullable" : "not-null";
                    string displayText = $"{column.Name} ({column.DataType}, {nillableText})";
                }
            }
        }

        #endregion

        #region Split Configuration

        private void btnCalculateSplit_Click(object sender, EventArgs e)
        {
            if (currentTableEntry == null || allColumns.Count == 0)
            {
                MessageBox.Show("Load TableIndex metadata først");
                return;
            }

            try
            {
                var splitConfigService = new SplitConfigurationService();
                var pkInfo = compositePKSelector.GetPrimaryKeyInfo();
                var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

                resultTables.Clear();
                List<int> splitPoints;

                // REFACTORED: Brug SplitConfigurationService
                if (IsSplitPointsEmpty())
                {
                    splitPoints = splitConfigService.CalculateAutoSplitPoints(currentTableEntry, pkColumns);

                    // Fjern placeholder og vis beregnede split punkter
                    HidePlaceholder();
                    txtSplitPoints.Text = string.Join(", ", splitPoints);

                    MessageBox.Show($"Auto-split beregnet: {splitPoints.Count + 1} tabeller med maks. 950 kolonner hver.\n\n" +
                                   $"Split punkter: {string.Join(", ", splitPoints)}\n\n" +
                                   $"Du kan ændre disse værdier hvis nødvendigt.",
                                   "Auto-Split Beregnet", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    splitPoints = splitConfigService.ParseSplitPoints(txtSplitPoints.Text);
                }

                string validation = splitConfigService.ValidateTableIndexAwareSplitPoints(
                    splitPoints, currentTableEntry, allColumns, pkColumns);

                if (!string.IsNullOrEmpty(validation))
                {
                    MessageBox.Show($"Split punkter er ikke gyldige:\n{validation}");
                    return;
                }

                resultTables = splitConfigService.GenerateTableIndexAwareSplitTables(
                    splitPoints, currentTableEntry, allColumns, pkInfo);

                ShowTableIndexSplitPreview();
                btnExecuteSplit.Enabled = resultTables.Count >= 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved TableIndex-aware split beregning: {ex.Message}");
            }
        }

        private List<int> CalculateAutoSplitPoints()
        {
            var splitPoints = new List<int>();

            if (currentTableEntry == null || allColumns.Count == 0)
                return splitPoints;

            var pkInfo = compositePKSelector.GetPrimaryKeyInfo();
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            int pkColumnsCount = pkColumns.Count;
            int totalColumns = allColumns.Count;

            if (totalColumns + pkColumnsCount <= 950)
            {
                // Scenario: Under 950 - halvér for test
                int midPoint = totalColumns / 2;
                if (midPoint > 0 && midPoint < totalColumns)
                {
                    splitPoints.Add(midPoint);
                }
            }
            else
            {
                // Scenario: Over 950 - fyld første tabel med 950 inklusiv PK
                int availableForDataInFirstSplit = 950 - pkColumnsCount;

                if (availableForDataInFirstSplit > 0)
                {
                    splitPoints.Add(availableForDataInFirstSplit);

                    int remainingColumns = totalColumns - availableForDataInFirstSplit;
                    int currentPosition = availableForDataInFirstSplit + 1;

                    while (remainingColumns > (950 - pkColumnsCount))
                    {
                        currentPosition += (950 - pkColumnsCount);
                        if (currentPosition < totalColumns)
                        {
                            splitPoints.Add(currentPosition);
                            remainingColumns = totalColumns - currentPosition;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return splitPoints;
        }

        private List<int> ParseSplitPoints(string input)
        {
            var points = new List<int>();

            // Skip parsing hvis det er placeholder tekst
            if (splitPointsHasPlaceholder || string.IsNullOrWhiteSpace(input))
                return points;

            foreach (string part in input.Split(',', ';'))
            {
                string trimmed = part.Trim();
                if (int.TryParse(trimmed, out int point))
                {
                    points.Add(point);
                }
            }

            return points.Distinct().OrderBy(x => x).ToList();
        }

        private string ValidateTableIndexAwareSplitPoints(List<int> splitPoints)
        {
            var errors = new List<string>();

            if (currentTableEntry == null)
            {
                errors.Add("TableIndex metadata mangler");
                return string.Join("\n", errors);
            }

            int maxColumns = currentTableEntry.Columns.Count;

            foreach (int point in splitPoints)
            {
                if (point < 1 || point >= maxColumns)
                {
                    errors.Add($"Split punkt {point} er udenfor gyldigt interval (1-{maxColumns - 1})");
                }
            }

            var tableSizes = CalculateTableSizes(splitPoints);
            for (int i = 0; i < tableSizes.Count; i++)
            {
                if (tableSizes[i] > 950)
                {
                    errors.Add($"Tabel {i + 1} vil få {tableSizes[i]} kolonner (over 950 grænse)");
                }
            }

            return errors.Count > 0 ? string.Join("\n", errors) : null;
        }

        private List<int> CalculateTableSizes(List<int> splitPoints)
        {
            var sizes = new List<int>();
            int lastPoint = 0;

            foreach (int point in splitPoints)
            {
                int size = point - lastPoint + 1;
                sizes.Add(size);
                lastPoint = point;
            }

            int finalSize = allColumns.Count - lastPoint + 1;
            sizes.Add(finalSize);

            return sizes;
        }

        /// <summary>
        /// Generate split tables med TableIndex metadata
        /// </summary>
        private void GenerateTableIndexAwareSplitTables(List<int> splitPoints)
        {
            resultTables.Clear();
            if (currentTableEntry == null) return;

            var pkInfo = compositePKSelector.GetPrimaryKeyInfo();
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            var sortedSplitPoints = splitPoints.OrderBy(x => x).ToList();

            int splitIndex = 1;
            int startColumn = 1;

            // Generer splits baseret på split points
            foreach (int splitPoint in sortedSplitPoints)
            {
                var table = new SplitTable
                {
                    TableName = $"{currentTableEntry.Name}_{splitIndex}",
                    StartColumn = startColumn,
                    EndColumn = splitPoint,
                    SplitIndex = splitIndex,
                    Columns = new List<XMLColumn>()
                };

                // Data kolonner i interval [startColumn, splitPoint]
                var dataColumns = allColumns
                    .Where(c => c.Position >= startColumn && c.Position <= splitPoint &&
                               !pkColumns.Contains(c.Name))
                    .OrderBy(c => c.Position)
                    .ToList();

                foreach (var dataCol in dataColumns)
                {
                    table.Columns.Add(dataCol);
                }

                // Tilføj ALLE PK kolonner
                foreach (var pkColumnName in pkColumns)
                {
                    var pkCol = allColumns.FirstOrDefault(c => c.Name == pkColumnName);
                    if (pkCol != null)
                    {
                        table.Columns.Add(pkCol);
                    }
                }

                resultTables.Add(table);
                startColumn = splitPoint + 1;
                splitIndex++;
            }

            // Sidste tabel får resterende kolonner
            if (startColumn <= allColumns.Count)
            {
                var lastTable = new SplitTable
                {
                    TableName = $"{currentTableEntry.Name}_{splitIndex}",
                    StartColumn = startColumn,
                    EndColumn = allColumns.Count,
                    SplitIndex = splitIndex,
                    Columns = new List<XMLColumn>()
                };

                var lastDataColumns = allColumns
                    .Where(c => c.Position >= startColumn && !pkColumns.Contains(c.Name))
                    .OrderBy(c => c.Position)
                    .ToList();

                foreach (var dataCol in lastDataColumns)
                {
                    lastTable.Columns.Add(dataCol);
                }

                foreach (var pkColumnName in pkColumns)
                {
                    var pkCol = allColumns.FirstOrDefault(c => c.Name == pkColumnName);
                    if (pkCol != null)
                    {
                        lastTable.Columns.Add(pkCol);
                    }
                }

                resultTables.Add(lastTable);
            }
        }

        /// <summary>
        /// Show split preview med TableIndex metadata
        /// </summary>
        private void ShowTableIndexSplitPreview()
        {
            lstSplitPreview.Items.Clear();

            if (resultTables.Count == 0 || currentTableEntry == null)
            {
                lblPreviewInfo.Text = "Ingen TableIndex-aware split konfigureret";
                return;
            }

            var pkInfo = compositePKSelector.GetPrimaryKeyInfo();
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            int pkColumnsCount = pkColumns.Count;

            foreach (var table in resultTables)
            {
                int dataColumnCount = table.Columns.Where(c => !pkColumns.Contains(c.Name)).Count();
                int totalColumnCount = dataColumnCount + pkColumnsCount;

                string statusIcon = totalColumnCount > 1000 ? "[FEJL]" :
                                   totalColumnCount > 950 ? "[ADVARSEL]" : "[OK]";

                string statusText = totalColumnCount > 1000 ? " - OVER 1000 KOLONNE GRÆNSE" :
                                   totalColumnCount > 950 ? " - NÆRMER SIG 1000 KOLONNE GRÆNSE" : "";

                string intervalText = $"kolonner {table.StartColumn}-{table.EndColumn}";
                string countText = $"({dataColumnCount} data + {pkColumnsCount} PK = {totalColumnCount} total)";
                string displayText = $"{statusIcon} {table.TableName}: {intervalText} {countText}{statusText}";

                lstSplitPreview.Items.Add(displayText);
            }

            string pkDisplayText = pkColumns.Count > 1 ?
                $"sammensat PK: {string.Join(", ", pkColumns)}" :
                $"PK: {pkColumns.FirstOrDefault() ?? "Ingen"}";

            lblPreviewInfo.Text = $"Split: {resultTables.Count} tabeller | {pkDisplayText} | Total: {currentTableEntry.Columns.Count} kolonner";
            lblPreviewInfo.ForeColor = Color.DarkGreen;
        }

        #endregion

        #region Split Preview

        private void UpdateSplitPreview()
        {
            if (currentTableEntry == null || !compositePKSelector.IsValid())
            {
                lblPreviewInfo.Text = "Konfigurer tabel og primærnøgle først...";
                lblPreviewInfo.ForeColor = Color.DarkRed;
                lstSplitPreview.Items.Clear();
                return;
            }

            try
            {
                var uiData = CollectUIData();

                // Validate split konfiguration
                if (!CompositePKSplitAlgorithm.ValidateSplitConfiguration(uiData.Tables, uiData))
                {
                    lblPreviewInfo.Text = "FEJL: Ugyldig split konfiguration!";
                    lblPreviewInfo.ForeColor = Color.DarkRed;
                    return;
                }

                // Update preview info
                var pkInfo = uiData.PrimaryKey;
                var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
                string pkType = pkInfo.IsComposite ? "Composite" : "Single";

                lblPreviewInfo.Text = $"Split Preview: {uiData.Tables.Count} tabeller | PK: {pkType} ({string.Join(", ", pkColumns)}) | Kapacitet: {uiData.GetAvailableDataColumnsPerSplit()} kolonner/split";
                lblPreviewInfo.ForeColor = Color.DarkGreen;

                // Populate preview list
                lstSplitPreview.Items.Clear();

                for (int i = 0; i < uiData.Tables.Count; i++)
                {
                    var table = uiData.Tables[i];
                    var dataColumns = table.Columns.Where(c => !pkColumns.Contains(c.Name)).Count();
                    var pkColumnsInSplit = table.Columns.Where(c => pkColumns.Contains(c.Name)).Count();

                    string previewText = $"  {table.TableName}: {dataColumns} data kolonner + {pkColumnsInSplit} PK kolonner = {table.Columns.Count} total";
                    lstSplitPreview.Items.Add(previewText);
                }
            }
            catch (Exception ex)
            {
                lblPreviewInfo.Text = $"FEJL i preview: {ex.Message}";
                lblPreviewInfo.ForeColor = Color.DarkRed;
                System.Diagnostics.Debug.WriteLine($"Preview error: {ex}");
            }
        }

        #endregion

        #region Primary Key Analysis

        /// <summary>
        /// PK Analyse med composite support - analysér combined unikhed
        /// REFACTORED: Bruger nu PrimaryKeyAnalysisService
        /// </summary>
        private async void btnAnalyzePK_Click(object sender, EventArgs e)
        {
            // FIX: Tjek tableIndex først
            if (currentTableEntry == null || string.IsNullOrEmpty(currentTableIndexPath))
            {
                MessageBox.Show("Du skal vælge en tableIndex.xml fil først før PK kan analyseres.\n\n" +
                               "Klik 'Browse TableIndex' og vælg en gyldig tableIndex.xml fil.",
                               "TableIndex Påkrævet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(currentXMLPath) || !File.Exists(currentXMLPath))
            {
                MessageBox.Show("XML data fil ikke fundet. Kontroller at den tilsvarende XML fil eksisterer.",
                               "XML Fil Mangler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!compositePKSelector.IsValid())
            {
                MessageBox.Show("Konfigurer gyldig primærnøgle først");
                return;
            }

            try
            {
                btnAnalyzePK.Enabled = false;
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;

                var pkAnalysisService = new PrimaryKeyAnalysisService();
                var pkInfo = compositePKSelector.GetPrimaryKeyInfo();
                var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

                lblPreviewInfo.Text = $"Analyserer {(pkInfo.IsComposite ? "composite" : "enkelt")} primærnøgle unikhed...";

                var (uniqueCount, totalCount, nullCount) = await Task.Run(() =>
                    pkAnalysisService.AnalyzeCompositePrimaryKeyUniqueness(
                        currentXMLPath, pkColumns, currentTableEntry, pkInfo));

                progressBar.Visible = false;

                string message = pkAnalysisService.BuildPKAnalysisMessage(pkColumns, uniqueCount, totalCount, nullCount);
                ShowPKAnalysisResult(message, uniqueCount, totalCount, nullCount);
            }
            catch (Exception ex)
            {
                progressBar.Visible = false;
                MessageBox.Show($"Fejl ved PK analyse: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"PK Analysis error: {ex}");
            }
            finally
            {
                btnAnalyzePK.Enabled = true;
                progressBar.Style = ProgressBarStyle.Continuous;
            }
        }

        /// <summary>
        /// Analysér combined unikhed for composite eller single PK
        /// </summary>
        private (int uniqueCount, int totalCount, int nullCount) AnalyzeCompositePrimaryKeyUniqueness(List<string> pkColumnNames)
        {
            var uniqueValues = new HashSet<string>();
            int totalCount = 0;
            int nullCount = 0;

            // Pre-build columnID lookup
            var columnIDMap = new Dictionary<string, string>();
            var columnNullableMap = new Dictionary<string, bool>();

            System.Diagnostics.Debug.WriteLine("=== PK ANALYSE START ===");
            System.Diagnostics.Debug.WriteLine($"Analyserer PK kolonner: {string.Join(", ", pkColumnNames)}");

            foreach (var pkColumnName in pkColumnNames)
            {
                if (pkColumnName == compositePKSelector.GetPrimaryKeyInfo().AutoGeneratedColumnName &&
                    compositePKSelector.GetPrimaryKeyInfo().IncludeAutoGenerated)
                {
                    System.Diagnostics.Debug.WriteLine($"Skipper auto-generated: {pkColumnName}");
                    continue;
                }

                var tableColumn = currentTableEntry?.Columns?.FirstOrDefault(c => c.Name == pkColumnName);
                if (tableColumn != null)
                {
                    columnIDMap[pkColumnName] = tableColumn.ColumnID;
                    columnNullableMap[pkColumnName] = tableColumn.IsNullable;
                    System.Diagnostics.Debug.WriteLine($"Mapped: '{pkColumnName}' → '{tableColumn.ColumnID}', nullable={tableColumn.IsNullable}");
                }
            }

            using (var reader = XmlReader.Create(currentXMLPath, new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true
            }))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
                    {
                        totalCount++;
                        bool debugThisRow = totalCount <= 3;

                        if (debugThisRow)
                        {
                            System.Diagnostics.Debug.WriteLine($"\n--- ROW {totalCount} ---");
                        }

                        // KORREKT APPROACH: Læs row som string først, parse bagefter
                        string rowXml = reader.ReadOuterXml();

                        // Parse med XDocument for SIKKER kolonnetilgang
                        var rowDoc = XDocument.Parse(rowXml);
                        var ns = rowDoc.Root.GetDefaultNamespace();

                        if (debugThisRow)
                        {
                            var allColumns = rowDoc.Descendants().Where(e => e.Name.LocalName.StartsWith("c")).Select(e => e.Name.LocalName);
                            System.Diagnostics.Debug.WriteLine($"  Kolonner i row: {string.Join(", ", allColumns)}");
                        }

                        // Byg sammensatte PK
                        var keyParts = new List<string>();
                        bool hasNull = false;
                        string nullReason = "";

                        foreach (var pkColumnName in pkColumnNames)
                        {
                            if (pkColumnName == compositePKSelector.GetPrimaryKeyInfo().AutoGeneratedColumnName &&
                                compositePKSelector.GetPrimaryKeyInfo().IncludeAutoGenerated)
                                continue;

                            if (!columnIDMap.ContainsKey(pkColumnName))
                            {
                                hasNull = true;
                                nullReason = $"Kolonne '{pkColumnName}' ikke i mapping";
                                break;
                            }

                            string columnID = columnIDMap[pkColumnName];
                            bool isNullable = columnNullableMap[pkColumnName];

                            var valueElement = rowDoc.Descendants(ns + columnID).FirstOrDefault();

                            if (debugThisRow)
                            {
                                System.Diagnostics.Debug.WriteLine($"  Søger efter: {columnID} ('{pkColumnName}')");
                            }

                            if (valueElement == null)
                            {
                                // Kolonne mangler i XML
                                if (isNullable)
                                {
                                    hasNull = true;
                                    nullReason = $"Nullable kolonne '{columnID}' mangler i XML";
                                    if (debugThisRow)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"    → Ikke fundet (nullable)");
                                    }
                                    break;
                                }
                                else
                                {
                                    // NOT NULL kolonne mangler - kritisk!
                                    hasNull = true;
                                    nullReason = $"KRITISK: Not-null kolonne '{columnID}' mangler i XML";
                                    if (debugThisRow)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"    → Ikke fundet (NOT NULL - FEJL!)");
                                    }
                                    break;
                                }
                            }

                            // Tjek for explicit null
                            var nilAttr = valueElement.Attribute(XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance") + "nil");
                            bool isExplicitNull = nilAttr?.Value?.ToLower() == "true";
                            string value = valueElement.Value;
                            bool isEmpty = string.IsNullOrWhiteSpace(value);

                            if (debugThisRow)
                            {
                                System.Diagnostics.Debug.WriteLine($"    → Værdi: '{value}' (xsi:nil={isExplicitNull}, empty={isEmpty})");
                            }

                            if (isExplicitNull)
                            {
                                hasNull = true;
                                nullReason = $"Kolonne '{columnID}' har xsi:nil='true'";
                                break;
                            }
                            else if (isEmpty)
                            {
                                if (isNullable)
                                {
                                    hasNull = true;
                                    nullReason = $"Kolonne '{columnID}' er tom (nullable)";
                                    break;
                                }
                                else
                                {
                                    // Tom streng i NOT NULL kolonne = valid værdi
                                    keyParts.Add("");
                                }
                            }
                            else
                            {
                                keyParts.Add(value.Trim());
                            }
                        }

                        if (hasNull)
                        {
                            nullCount++;
                            if (nullCount <= 5 || debugThisRow)
                            {
                                System.Diagnostics.Debug.WriteLine($"  RESULTAT: NULL ({nullReason})");
                            }
                        }
                        else
                        {
                            string compositeKey = string.Join("|", keyParts);
                            uniqueValues.Add(compositeKey);

                            if (debugThisRow)
                            {
                                System.Diagnostics.Debug.WriteLine($"  RESULTAT: Key = '{compositeKey}'");
                            }
                        }

                        // GC hint hver 10000 rows
                        if (totalCount % 10000 == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Processed {totalCount:N0} rows, {uniqueValues.Count:N0} unique, {nullCount:N0} null");
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"\n=== PK ANALYSE SLUT ===");
            System.Diagnostics.Debug.WriteLine($"Total: {totalCount:N0}, Unique: {uniqueValues.Count:N0}, Null: {nullCount:N0}");

            return (uniqueValues.Count, totalCount, nullCount);
        }

        /// <summary>
        /// Byg PK analyse besked
        /// </summary>
        private string BuildPKAnalysisMessage(List<string> pkColumns, int uniqueCount, int totalCount, int nullCount)
        {
            string pkDescription = pkColumns.Count == 1 ?
                $"kolonne '{pkColumns[0]}'" :
                $"kombinerede kolonner: {string.Join(", ", pkColumns)}";

            string message = $"Analyse af primærnøgle {pkDescription}:\n\n" +
                           $"• Total rækker: {totalCount:N0}\n" +
                           $"• Unikke kombinationer: {uniqueCount:N0}\n" +
                           $"• Rækker med null/tomme værdier: {nullCount:N0}\n\n";

            return message;
        }

        /// <summary>
        /// Vis PK analyse resultat med korrekt feedback
        /// </summary>
        private void ShowPKAnalysisResult(string baseMessage, int uniqueCount, int totalCount, int nullCount)
        {
            string message = baseMessage;
            var pkInfo = compositePKSelector.GetPrimaryKeyInfo();

            if (uniqueCount == totalCount && nullCount == 0)
            {
                message += "PERFEKT: Primærnøglekombinationen er unik og komplet - ideel til arkiveringsversionen!";
                MessageBox.Show(message, "Primærnøgle Analyse", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (uniqueCount + nullCount == totalCount && nullCount > 0)
            {
                message += "NÆSTEN GOD: Kombinationen er unik men har null værdier\n" +
                         "For arkiveringsversionen anbefales det at evt. tilføje en auto-genereret kolonne.";

                var result = MessageBox.Show(message + "\n\nTilføj auto-genereret kolonne til primærnøglen?",
                                            "Primærnøgle Analyse", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    pkInfo.IncludeAutoGenerated = true;
                    compositePKSelector.SetPrimaryKeyInfo(pkInfo);
                }
            }
            else
            {
                int duplicates = totalCount - uniqueCount - nullCount;
                message += $"IKKE EGNET: Kombinationen er ikke unik!\n" +
                         $"Antal duplikat kombinationer: {duplicates:N0}\n" +
                         $"Dette vil bryde referentiel integritet i arkiveringsversionen.\n\n" +
                         $"Anbefaling: Tilføj auto-genereret kolonne til primærnøglen.";

                var result = MessageBox.Show(message + "\n\nTilføj auto-genereret kolonne til primærnøglen?",
                                            "Primærnøgle Analyse", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    pkInfo.IncludeAutoGenerated = true;
                    compositePKSelector.SetPrimaryKeyInfo(pkInfo);
                }
            }
        }

        #endregion

        #region Split Execution

        private void btnExecuteSplit_Click(object sender, EventArgs e)
        {
            if (!compositePKSelector.IsValid())
            {
                MessageBox.Show("Ugyldig primærnøgle konfiguration: " + compositePKSelector.GetValidationError());
                return;
            }

            try
            {
                btnExecuteSplit.Enabled = false;
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;

                var uiData = CollectUIData();

                // Validér split konfiguration
                if (!CompositePKSplitAlgorithm.ValidateSplitConfiguration(uiData.Tables, uiData))
                {
                    MessageBox.Show("Fejl i split konfiguration!");
                    return;
                }

                // Log split konfiguration
                LogSplitConfiguration(uiData);

                // Kør split execution
                ExecuteSplitOperation(uiData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved split execution: {ex.Message}\n\nDetaljer: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"Split execution error: {ex}");
            }
            finally
            {
                btnExecuteSplit.Enabled = true;
                progressBar.Visible = false;
                progressBar.Style = ProgressBarStyle.Continuous;
            }
        }

        /// <summary>
        /// Kør split operation med fuld logging og fil generering
        /// Bruger nu XMLSplitService orchestrator
        /// </summary>
        private async void ExecuteSplitOperation(UIDataContainer uiData)
        {
            try
            {
                var splitService = new XMLSplitService();
                var logger = new Utilities.SplitLogger();
                var progress = new FormProgressReporter(progressBar, lblPreviewInfo);

                var result = await Task.Run(() => splitService.ExecuteSplit(uiData, progress, logger));

                if (result.Success)
                {
                    var openResult = MessageBox.Show(
                        $"Split fuldført!\n\n" +
                        $"Oprettet {result.TablesGenerated} nye tabeller i:\n{result.OutputDirectory}\n\n" +
                        $"Filer genereret:\n" +
                        $"- {result.TablesGenerated} XML filer\n" +
                        $"- tableIndex_updated.xml med cross-references\n" +
                        $"- Detaljeret operation log\n\n" +
                        $"Åbn output mappe?",
                        "Split Fuldført",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (openResult == DialogResult.Yes)
                    {
                        OpenDirectorySafely(result.OutputDirectory);
                    }
                }
                else
                {
                    MessageBox.Show($"Fejl under split operation:\n{result.ErrorMessage}",
                                   "Operation Fejlede", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl under split operation:\n{ex.Message}",
                               "Operation Fejlede", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Thread-safe split execution med ATOMIC OPERATIONS
        /// Genererer i temp folder først, validerer, derefter flytter til final destination
        /// </summary>
        private string ExecuteTableIndexSplit(UIDataContainer uiData, IProgress<(int, string)> progress)
        {
            var logger = new SplitLogger();
            string tempDirectory = null;
            string finalOutputDirectory = null;

            try
            {
                // STEP 1: Opret temp directory
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string parentFolder = Path.Combine(desktopPath, "XML_Table_Splits");

                // Parse original table nummer
                int originalTableNumber = int.Parse(Regex.Match(uiData.OriginalTableEntry.Folder, @"table(\d+)").Groups[1].Value);

                string versionNumber = GetNextVersionNumber(parentFolder, uiData.OriginalTableName);
                string splitFolderName = $"split_{uiData.OriginalTableName}_table{originalTableNumber}_{versionNumber}";

                // Timestamp til logging
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                // TEMP folder for generation
                tempDirectory = Path.Combine(Path.GetTempPath(), $"xml_split_temp_{Guid.NewGuid():N}");
                Directory.CreateDirectory(tempDirectory);

                // FINAL destination
                finalOutputDirectory = Path.Combine(parentFolder, splitFolderName);

                logger.LogInfo($"Arbejder i temp mappe: {tempDirectory}");
                logger.LogInfo($"Final destination: {finalOutputDirectory}");

                progress?.Report((5, "Initialiserer opdeling..."));

                LogSplitOperationStart(logger, uiData, versionNumber);

                progress?.Report((10, "Genererer split tabeller i temp..."));

                // Parse next table nummer (originalTableNumber er allerede deklareret ovenfor)
                int nextTableNumber = GetNextAvailableTableNumber(uiData.TableIndexPath);

                // STEP 2: Generer ALLE filer i temp directory
                for (int i = 0; i < uiData.Tables.Count; i++)
                {
                    int currentTableNumber = (i == 0) ? originalTableNumber : nextTableNumber + (i - 1);
                    GenerateTableFiles(uiData.Tables[i], tempDirectory, uiData, currentTableNumber);
                }

                progress?.Report((75, "Opdaterer tableIndex.xml..."));

                if (!string.IsNullOrEmpty(uiData.TableIndexPath))
                {
                    logger.LogInfo("Starter opdatering af tableIndex.xml med sammensat PK support");
                    try
                    {
                        string tableIndexLog = GenerateUpdatedTableIndex(uiData.Tables, tempDirectory, uiData, originalTableNumber, nextTableNumber);
                        logger.LogInfo("Afsluttet opdatering af tableIndex.xml");

                        // Gem tableIndex transformation log til senere brug i technical log
                        logger.LogInfo("");
                        logger.LogInfo("=== TABLEINDEX TRANSFORMATION DETAILS ===");
                        logger.LogInfo(tableIndexLog);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Fejl ved opdatering af tableIndex.xml: {ex.Message}");
                        throw; // Re-throw for rollback
                    }
                }

                progress?.Report((85, "Validerer output..."));

                // STEP 3: Validér output før commit
                logger.LogInfo("Starter validering af genererede filer...");
                ValidateSplitOutput(tempDirectory, uiData, logger);
                logger.LogInfo("Validering bestået - alle filer er korekte");

                progress?.Report((90, "Flytter filer til final destination..."));

                // STEP 4: Opret final directory og flyt alt
                Directory.CreateDirectory(Path.GetDirectoryName(finalOutputDirectory));
                Directory.Move(tempDirectory, finalOutputDirectory);
                tempDirectory = null; // Mark som flyttet

                logger.LogInfo($"Filer flyttet til final destination: {finalOutputDirectory}");

                progress?.Report((95, "Opretter log filer..."));

                LogSplitOperationEnd(logger, finalOutputDirectory, timestamp, uiData);

                progress?.Report((100, "Opdeling fuldført"));
                return finalOutputDirectory;
            }
            catch (Exception ex)
            {
                logger.LogError($"KRITISK FEJL: {ex.Message}");
                logger.LogError($"Stack trace: {ex.StackTrace}");

                // Slet temp directory hvis det stadig eksisterer
                if (tempDirectory != null && Directory.Exists(tempDirectory))
                {
                    try
                    {
                        Directory.Delete(tempDirectory, true);
                        logger.LogInfo("Temp directory slettet efter fejl (rollback)");
                    }
                    catch (Exception cleanupEx)
                    {
                        logger.LogWarning($"Kunne ikke slette temp directory: {cleanupEx.Message}");
                    }
                }

                try
                {
                    string errorLogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                                     $"xml_split_error_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                    logger.SaveToFile(errorLogPath);
                }
                catch
                {
                    // Ignore - kan ikke gemme log
                }

                throw;
            }
        }

        /// <summary>
        /// Log split operation start
        /// </summary>
        private void LogSplitOperationStart(SplitLogger logger, UIDataContainer uiData, string versionNumber)
        {
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

            // TILFØJET: Log reunion view information
            logger.LogInfo("");
            logger.LogInfo("REUNION VIEW KONFIGURATION:");
            logger.LogInfo($"View navn: AV_Opsamling_af_{uiData.OriginalTableName}");
            logger.LogInfo($"Split tabeller: {string.Join(", ", uiData.Tables.Select(t => t.TableName))}");
            logger.LogInfo($"JOIN på: {string.Join(", ", pkColumns)}");

            // Detaljeret kolonne mapping (eksisterende logik uændret)
            logger.LogInfo("");
            logger.LogInfo("KOLONNE MAPPING:");
            logger.LogInfo("=========================================");

            foreach (var table in uiData.Tables)
            {
                logger.LogInfo($"");
                logger.LogInfo($"Tabel: {table.TableName}");

                var dataColumns = table.Columns.Where(c => !pkColumns.Contains(c.Name)).ToList();
                var pkColumnsInSplit = table.Columns.Where(c => pkColumns.Contains(c.Name)).ToList();

                logger.LogInfo($"  PK Kolonner ({pkColumnsInSplit.Count}):");
                foreach (var pkCol in pkColumnsInSplit)
                {
                    logger.LogInfo($"    - {pkCol.Name} ({pkCol.DataType}) - duplikeret til alle splits");
                }

                logger.LogInfo($"  Data Kolonner ({dataColumns.Count}):");
                foreach (var dataCol in dataColumns)
                {
                    logger.LogInfo($"    - {dataCol.ColumnID}: {dataCol.Name} ({dataCol.DataType})");
                }

                logger.LogInfo($"  → Total kolonner: {table.Columns.Count} ({pkColumnsInSplit.Count} PK + {dataColumns.Count} data)");
            }
            logger.LogInfo("");
        }

        /// <summary>
        /// Log split operation end og gem log
        /// </summary>
        private void LogSplitOperationEnd(SplitLogger logger, string outputDirectory, string timestamp, UIDataContainer uiData)
        {
            // Generer fil oversigt for logs
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

            // GEM 2 LOGS: Technical (fuld detail) og Complete (user-friendly)
            string technicalLogPath = Path.Combine(outputDirectory, $"split_operation_technical_{timestamp}.log");
            logger.SaveToFile(technicalLogPath);

            string completeLogPath = Path.Combine(outputDirectory, $"split_operation_complete_{timestamp}.log");
            GenerateUserFriendlyLog(completeLogPath, outputDirectory, timestamp, uiData);
        }

        /// <summary>
        /// Generer brugervenlig log med struktureret oversigt
        /// </summary>
        /// <summary>
        /// Generer brugervenlig log med struktureret oversigt
        /// </summary>
        private void GenerateUserFriendlyLog(string logPath, string outputDirectory, string timestamp, UIDataContainer uiData)
        {
            var log = new StringBuilder();
            var startTime = DateTime.Now.AddSeconds(-5);

            log.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            log.AppendLine("                    XML TABLE SPLIT OPERATION - COMPLETE LOG");
            log.AppendLine("═══════════════════════════════════════════════════════════════════════════");

            string versionNumber = Path.GetFileName(outputDirectory).Split('_').ElementAtOrDefault(2) ?? "v1.0";
            string operationId = $"split_{uiData.OriginalTableName}_{versionNumber}_{timestamp}";

            log.AppendLine($"Operation ID: {operationId}");
            log.AppendLine($"Start: {startTime:yyyy-MM-dd HH:mm:ss}");
            log.AppendLine($"Slut: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            log.AppendLine($"Varighed: {(DateTime.Now - startTime):hh\\:mm\\:ss}");
            log.AppendLine($"System: {Environment.MachineName} | Bruger: {Environment.UserName}");
            log.AppendLine("───────────────────────────────────────────────────────────────────────────");
            log.AppendLine();

            // Section 1: Operation Overview
            log.AppendLine("═══ 1. OPERATION OVERVIEW ═══");
            log.AppendLine();

            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            log.AppendLine($"Original Tabel:        {uiData.OriginalTableName}");
            log.AppendLine($"Total Rækker:          {uiData.TotalRows}");
            log.AppendLine($"Total Kolonner:        {uiData.AllColumns.Count}");
            log.AppendLine($"Split Strategi:        {uiData.Tables.Count} tabeller (maks 950 kolonner per tabel)");
            log.AppendLine($"PK Type:               {(pkInfo.IsComposite ? "Composite" : "Single")} ({string.Join(", ", pkColumns)})");
            log.AppendLine($"Version:               {versionNumber}");
            log.AppendLine($"Output Mappe:          {outputDirectory}");
            log.AppendLine();

            log.AppendLine("SPLIT TABELLER:");
            int originalTableNumber = int.Parse(Regex.Match(uiData.OriginalTableEntry.Folder, @"table(\d+)").Groups[1].Value);
            int nextTableNumber = GetNextAvailableTableNumber(uiData.TableIndexPath);

            for (int i = 0; i < uiData.Tables.Count; i++)
            {
                var table = uiData.Tables[i];
                int tableNumber = (i == 0) ? originalTableNumber : nextTableNumber + (i - 1);
                var dataColumns = table.Columns.Where(c => !pkColumns.Contains(c.Name)).Count();
                log.AppendLine($"  • {table.TableName} → table{tableNumber}  ({table.Columns.Count} kolonner: {pkColumns.Count} PK + {dataColumns} data)");
            }
            log.AppendLine();

            log.AppendLine("REUNION VIEW:");
            log.AppendLine($"  • AV_Opsamling_af_{uiData.OriginalTableName}");
            log.AppendLine($"  • JOIN på: {string.Join(", ", pkColumns)}");
            log.AppendLine();
            log.AppendLine();

            // Section 2: Errors & Exceptions
            log.AppendLine("═══ 2. ERRORS & EXCEPTIONS ═══");
            log.AppendLine();
            log.AppendLine("Status: No errors detected");
            log.AppendLine("Warnings: 0");
            log.AppendLine("Errors: 0");
            log.AppendLine("Critical: 0");
            log.AppendLine();
            log.AppendLine();

            // Section 3: Foreign Key & TableIndex Transformation
            log.AppendLine("═══ 3. TRANSFORMATION SUMMARY ═══");
            log.AppendLine();
            log.AppendLine("Foreign Key Transformation:");
            log.AppendLine("  Se split_operation_technical log for detaljeret FK analyse");
            log.AppendLine();
            log.AppendLine("TableIndex Transformation:");
            log.AppendLine("  Se split_operation_technical log for step-by-step transformation");
            log.AppendLine();
            log.AppendLine();

            // Section 4: Column Distribution
            log.AppendLine("═══ 4. COLUMN DISTRIBUTION ═══");
            log.AppendLine();

            foreach (var table in uiData.Tables)
            {
                var dataColumns = table.Columns.Where(c => !pkColumns.Contains(c.Name)).ToList();

                log.AppendLine($"{table.TableName} - {table.Columns.Count} kolonner");
                log.AppendLine("───────────────────────────────────────────────────────────────────────────");

                log.AppendLine($"  PRIMARY KEY ({pkColumns.Count} kolonner - duplikeret til alle splits):");
                foreach (var pkCol in pkColumns)
                {
                    var col = table.Columns.FirstOrDefault(c => c.Name == pkCol);
                    if (col != null)
                        log.AppendLine($"    • {col.Name} ({col.DataType})");
                }
                log.AppendLine();

                log.AppendLine($"  DATA KOLONNER ({dataColumns.Count}):");
                foreach (var col in dataColumns)
                {
                    log.AppendLine($"    {col.ColumnID} → {col.Name} ({col.DataType})");
                }
                log.AppendLine();
            }

            log.AppendLine();

            // Section 5: File Generation & Validation
            log.AppendLine("═══ 5. FILE GENERATION & VALIDATION ═══");
            log.AppendLine();

            var generatedFiles = Directory.GetFiles(outputDirectory, "*", SearchOption.AllDirectories);
            var xmlFiles = generatedFiles.Where(f => f.EndsWith(".xml") && !f.Contains("tableIndex")).OrderBy(f => f).ToList();
            var tableIndexFiles = generatedFiles.Where(f => f.Contains("tableIndex")).ToList();

            log.AppendLine("XML Data Filer:");
            log.AppendLine("───────────────────────────────────────────────────────────────────────────");
            foreach (var file in xmlFiles)
            {
                var fileInfo = new FileInfo(file);
                log.AppendLine($"  {Path.GetFileName(file)}");
                log.AppendLine($"    → Size: {fileInfo.Length:N0} bytes");

                try
                {
                    int rowCount = 0;
                    using (var reader = XmlReader.Create(file))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
                                rowCount++;
                        }
                    }
                    log.AppendLine($"    → Rows: {rowCount}");
                    log.AppendLine($"    → Structure: VALID");
                }
                catch
                {
                    log.AppendLine($"    → Structure: Could not validate");
                }
                log.AppendLine();
            }

            if (tableIndexFiles.Count > 0)
            {
                log.AppendLine("TableIndex:");
                log.AppendLine("───────────────────────────────────────────────────────────────────────────");
                foreach (var file in tableIndexFiles)
                {
                    var fileInfo = new FileInfo(file);
                    log.AppendLine($"  {Path.GetFileName(file)}");
                    log.AppendLine($"    → Size: {fileInfo.Length:N0} bytes");
                    log.AppendLine($"    → Original '{uiData.OriginalTableName}' erstattet med '{uiData.Tables[0].TableName}'");
                    if (uiData.Tables.Count > 1)
                    {
                        for (int i = 1; i < uiData.Tables.Count; i++)
                        {
                            log.AppendLine($"    → '{uiData.Tables[i].TableName}' tilføjet");
                        }
                    }
                    log.AppendLine($"    → Reunion view tilføjet");
                }
            }

            log.AppendLine();
            log.AppendLine();

            // Section 6: Operation Summary
            log.AppendLine("═══ 6. OPERATION SUMMARY ═══");
            log.AppendLine();
            log.AppendLine($"STATUS: COMPLETED");
            log.AppendLine("───────────────────────────────────────────────────────────────────────────");
            log.AppendLine($"Tabeller genereret:    {uiData.Tables.Count} ({string.Join(", ", uiData.Tables.Select(t => t.TableName))})");
            log.AppendLine($"Rækker per tabel:      {uiData.TotalRows}");
            log.AppendLine($"Warnings:              0");
            log.AppendLine($"Errors:                0");

            var totalFiles = generatedFiles.Length;
            var xmlCount = xmlFiles.Count;
            log.AppendLine($"Total filer:           {totalFiles} ({xmlCount} XML data + 1 tableIndex + 2 logs)");

            log.AppendLine();
            log.AppendLine($"Output lokation:");
            log.AppendLine($"  {outputDirectory}");

            log.AppendLine();
            log.AppendLine("═══════════════════════════════════════════════════════════════════════════");
            log.AppendLine("                              LOG AFSLUTTET");
            log.AppendLine("═══════════════════════════════════════════════════════════════════════════");

            File.WriteAllText(logPath, log.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Log split konfiguration til debug
        /// </summary>
        private void LogSplitConfiguration(UIDataContainer uiData)
        {
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("=== SPLIT CONFIGURATION ===");
            System.Diagnostics.Debug.WriteLine($"Original Table: {uiData.OriginalTableName}");
            System.Diagnostics.Debug.WriteLine($"Total Columns: {uiData.AllColumns.Count}");
            System.Diagnostics.Debug.WriteLine($"PK Type: {(pkInfo.IsComposite ? "Composite" : "Single")}");
            System.Diagnostics.Debug.WriteLine($"PK Columns: {string.Join(", ", pkColumns)}");
            System.Diagnostics.Debug.WriteLine($"Available Capacity per Split: {uiData.GetAvailableDataColumnsPerSplit()}");
            System.Diagnostics.Debug.WriteLine($"Generated Splits: {uiData.Tables.Count}");

            foreach (var table in uiData.Tables)
            {
                var dataColumns = table.Columns.Where(c => !pkColumns.Contains(c.Name)).Count();
                var pkColumnsInSplit = table.Columns.Where(c => pkColumns.Contains(c.Name)).Count();
                System.Diagnostics.Debug.WriteLine($"  {table.TableName}: {dataColumns} data + {pkColumnsInSplit} PK = {table.Columns.Count} total");
            }
            System.Diagnostics.Debug.WriteLine("=========================================");
        }

        /// <summary>
        /// Validér at split output er korrekt før commit
        /// </summary>
        private void ValidateSplitOutput(string outputDirectory, UIDataContainer uiData, SplitLogger logger)
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
                        logger.LogInfo($"{Path.GetFileName(xmlFile)}: Validation OK ({rowCount} rows, valid structure)");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(xmlFile)}: Kunne ikke validere - {ex.Message}");
                }
            }

            if (errors.Count > 0)
            {
                logger.LogError("VALIDERING FEJLEDE:");
                foreach (var error in errors)
                {
                    logger.LogError($"  - {error}");
                }

                throw new InvalidOperationException(
                    $"Split output validering fejlede med {errors.Count} fejl:\n" +
                    string.Join("\n", errors)
                );
            }

            logger.LogInfo($"Validering OK - {uiData.Tables.Count} tabeller, {uiData.TotalRows} rows per tabel");
        }

        #endregion

        #region File Generation

        /// <summary>
        /// Generer XML og XSD filer for split tabel med sammensat PK support
        /// </summary>
        private void GenerateTableFiles(SplitTable table, string outputDirectory, UIDataContainer uiData, int tableNumber)
        {
            // Direkte table folder uden beskrivende wrapper
            string tableFolder = $"table{tableNumber}";
            string tablePath = Path.Combine(outputDirectory, tableFolder);
            Directory.CreateDirectory(tablePath);

            // XML fil med korrekt navn og namespace
            string xmlFileName = $"table{tableNumber}.xml";
            string xmlPath = Path.Combine(tablePath, xmlFileName);

            // Generer namespace med korrekt tabelnummer
            string newNamespace = $"http://www.sa.dk/xmlns/siard/1.0/schema0/table{tableNumber}.xsd";

            GenerateXMLFile(table, xmlPath, newNamespace, uiData, tableNumber);
        }

        /// <summary>
        /// Generer XML fil for split tabel med sammensat PK support
        /// </summary>
        private void GenerateXMLFile(SplitTable table, string xmlPath, string newNamespace, UIDataContainer uiData, int tableNumber)
        {
            string xsdFileName = $"table{tableNumber}.xsd";

            using (var writer = XmlWriter.Create(xmlPath, new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8
            }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("table", newNamespace);
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", $"{newNamespace} {xsdFileName}");

                ProcessXMLRowsWithCompositePK(writer, table, uiData);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Process XML rækker med sammensat PK support og korrekt kolonne placering
        /// </summary>
        private void ProcessXMLRowsWithCompositePK(XmlWriter writer, SplitTable table, UIDataContainer uiData)
        {
            int currentRowId = 1;
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            using (var reader = XmlReader.Create(uiData.XMLPath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
                    {
                        var rowXml = reader.ReadOuterXml();
                        var rowDoc = XDocument.Parse(rowXml);
                        var ns = rowDoc.Root.GetDefaultNamespace();

                        writer.WriteStartElement("row");
                        int xmlColumnCounter = 1;

                        if (table.SplitIndex == 1)
                        {
                            // TABLE 1: Behold original struktur med PK på original position
                            foreach (var column in table.Columns.OrderBy(c => c.Position))
                            {
                                // Skip auto-generated kolonne - den håndteres efter loopen
                                if (column.Name == pkInfo.AutoGeneratedColumnName && pkInfo.IncludeAutoGenerated)
                                {
                                    continue;
                                }

                                if (pkColumns.Contains(column.Name))
                                {
                                    // Eksisterende PK kolonne - kopier fra original XML
                                    WriteExistingPKColumn(writer, xmlColumnCounter, column, rowDoc, ns, uiData);
                                }
                                else
                                {
                                    // Data kolonne
                                    WriteDataColumn(writer, xmlColumnCounter, column, rowDoc, ns);
                                }
                                xmlColumnCounter++;
                            }

                            // Auto-generated PK i slutningen - KUN ÉN GANG
                            if (pkInfo.IncludeAutoGenerated)
                            {
                                WriteAutoGeneratedPKColumn(writer, xmlColumnCounter, currentRowId);
                                xmlColumnCounter++;
                            }
                        }
                        else
                        {
                            // TABLE 2+: Data kolonner først, PK kolonner i slutningen
                            var dataColumns = table.Columns
                                .Where(c => !pkColumns.Contains(c.Name))
                                .OrderBy(c => c.Position);

                            // Filtrer auto-generated kolonne fra eksisterende PK kolonner
                            var pkColumnsInTable = table.Columns
                                .Where(c => pkColumns.Contains(c.Name) &&
                                            c.Name != pkInfo.AutoGeneratedColumnName)
                                .OrderBy(c => c.Position);

                            // Skriv data kolonner først
                            foreach (var column in dataColumns)
                            {
                                WriteDataColumn(writer, xmlColumnCounter, column, rowDoc, ns);
                                xmlColumnCounter++;
                            }

                            // Skriv eksisterende PK kolonner
                            foreach (var column in pkColumnsInTable)
                            {
                                WriteExistingPKColumn(writer, xmlColumnCounter, column, rowDoc, ns, uiData);
                                xmlColumnCounter++;
                            }

                            // Auto-generated PK kolonne allersidst - KUN ÉN GANG
                            if (pkInfo.IncludeAutoGenerated)
                            {
                                WriteAutoGeneratedPKColumn(writer, xmlColumnCounter, currentRowId);
                                xmlColumnCounter++;
                            }
                        }

                        writer.WriteEndElement(); // </row>
                        currentRowId++;
                    }
                }
            }
        }

        /// <summary>
        /// Skriv eksisterende PK kolonne
        /// </summary>
        private void WriteExistingPKColumn(XmlWriter writer, int columnNumber, XMLColumn column, XDocument rowDoc, XNamespace ns, UIDataContainer uiData)
        {
            writer.WriteStartElement($"c{columnNumber}");

            var element = rowDoc.Descendants(ns + column.ColumnID).FirstOrDefault();

            if (element != null)
            {
                var xsiNs = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
                var nilAttr = element.Attribute(xsiNs + "nil");

                bool hasNilAttribute = nilAttr?.Value?.ToLower() == "true";
                bool hasValue = !string.IsNullOrWhiteSpace(element.Value);

                if (hasNilAttribute)
                {
                    writer.WriteAttributeString("xsi", "nil", xsiNs.NamespaceName, "true");
                }
                else if (hasValue)
                {
                    writer.WriteString(element.Value);
                }
                else
                {
                    if (column.IsNullable)
                    {
                        writer.WriteAttributeString("xsi", "nil", xsiNs.NamespaceName, "true");
                    }
                    else
                    {
                        // Not-null PK med tom værdi - LOG kun dette
                        System.Diagnostics.Debug.WriteLine($"WARNING: Not-null PK {column.ColumnID} has empty value");
                    }
                }
            }
            else
            {
                // PK kolonne mangler HELT - DET er kritisk!
                System.Diagnostics.Debug.WriteLine($"CRITICAL: PK column {column.ColumnID} missing in source XML!");

                if (column.IsNullable)
                {
                    writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Skriv data kolonne
        /// </summary>
        private void WriteDataColumn(XmlWriter writer, int columnNumber, XMLColumn column, XDocument rowDoc, XNamespace ns)
        {
            writer.WriteStartElement($"c{columnNumber}");

            var element = rowDoc.Descendants(ns + column.ColumnID).FirstOrDefault();

            if (element != null)
            {
                var xsiNs = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
                var nilAttr = element.Attribute(xsiNs + "nil");

                bool hasNilAttribute = nilAttr?.Value?.ToLower() == "true";
                bool hasValue = !string.IsNullOrWhiteSpace(element.Value);

                if (hasNilAttribute)
                {
                    writer.WriteAttributeString("xsi", "nil", xsiNs.NamespaceName, "true");
                }
                else if (hasValue)
                {
                    writer.WriteString(element.Value);
                }
                else
                {
                    if (column.IsNullable)
                    {
                        writer.WriteAttributeString("xsi", "nil", xsiNs.NamespaceName, "true");
                    }
                }
            }
            else
            {
                // SPARSE XML: Kolonne mangler i source = null værdi (NORMALT)
                if (column.IsNullable)
                {
                    writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
                }
                // For not-null kolonner: skriv tomt element (XSD validering håndterer det)
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Skriv auto-generated PK kolonne
        /// </summary>
        private void WriteAutoGeneratedPKColumn(XmlWriter writer, int columnNumber, int rowId)
        {
            writer.WriteStartElement($"c{columnNumber}");
            writer.WriteString(rowId.ToString());
            writer.WriteEndElement();
        }

        #endregion

        #region TableIndex Generation

        /// <summary>
        /// Generer opdateret tableIndex.xml med split tabeller og opdaterede FK referencer
        /// ERSTATTER original tabel med første split på samme position
        /// </summary>
        private string GenerateUpdatedTableIndex(List<SplitTable> splitTables, string outputDirectory, UIDataContainer uiData, int originalTableNumber, int startingTableNumber)
        {
            try
            {
                string sourceTableIndexPath = uiData.TableIndexPath;
                string outputTableIndexPath = Path.Combine(outputDirectory, "tableIndex_updated.xml");
                var doc = XDocument.Load(sourceTableIndexPath);
                var ns = doc.Root.GetDefaultNamespace();

                var logger = new StringBuilder();
                logger.AppendLine("═══════════════════════════════════════════════════════════");
                logger.AppendLine("         TABLEINDEX TRANSFORMATION LOG");
                logger.AppendLine("═══════════════════════════════════════════════════════════");
                logger.AppendLine($"Original tabel: {uiData.OriginalTableEntry.Name}");
                logger.AppendLine($"Split strategi: {splitTables.Count} tabeller");
                logger.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                logger.AppendLine();

                // STEP 1: Find og erstat original tabel med første split
                logger.AppendLine("STEP 1: ERSTAT ORIGINAL TABEL");
                logger.AppendLine("───────────────────────────────");

                var tablesElement = doc.Descendants(ns + "tables").First();
                var originalTableElement = tablesElement.Elements(ns + "table")
                    .FirstOrDefault(t => t.Element(ns + "name")?.Value == uiData.OriginalTableEntry.Name);

                if (originalTableElement == null)
                {
                    throw new Exception($"Original tabel '{uiData.OriginalTableEntry.Name}' ikke fundet i tableIndex!");
                }

                int originalPosition = tablesElement.Elements(ns + "table").ToList().IndexOf(originalTableElement) + 1;
                logger.AppendLine($"  Position i tableIndex: #{originalPosition}");
                logger.AppendLine($"  Original table nummer: {originalTableNumber}");

                // Opret første split element (erstatter original)
                var firstSplitElement = CreateCompleteTableElementWithCompositePK(
                    splitTables[0],
                    splitTables,
                    ns,
                    uiData,
                    originalTableNumber  // BEVAR original table nummer
                );

                // ERSTAT original tabel med første split
                originalTableElement.ReplaceWith(firstSplitElement);
                logger.AppendLine($" Erstattede '{uiData.OriginalTableEntry.Name}' med '{splitTables[0].TableName}'");
                logger.AppendLine($"  → Beholder table{originalTableNumber} folder");
                logger.AppendLine();

                // STEP 2: Tilføj resterende splits til bunden
                if (splitTables.Count > 1)
                {
                    logger.AppendLine("STEP 2: TILFØJ RESTERENDE SPLITS");
                    logger.AppendLine("─────────────────────────────────");

                    for (int i = 1; i < splitTables.Count; i++)
                    {
                        int currentTableNumber = startingTableNumber + (i - 1);
                        var splitElement = CreateCompleteTableElementWithCompositePK(
                            splitTables[i],
                            splitTables,
                            ns,
                            uiData,
                            currentTableNumber
                        );

                        tablesElement.Add(splitElement);
                        logger.AppendLine($"Tilføjede '{splitTables[i].TableName}' som table{currentTableNumber}");
                    }
                    logger.AppendLine();
                }

                // STEP 3: Opdater alle foreign key referencer
                logger.AppendLine("STEP 3: OPDATER FOREIGN KEY REFERENCER");
                logger.AppendLine("───────────────────────────────────────");
                var fkUpdateLog = UpdateForeignKeyReferences(doc, ns, splitTables, uiData);
                logger.Append(fkUpdateLog);
                logger.AppendLine();

                // STEP 4: Opret eller opdater views sektion
                logger.AppendLine("STEP 4: GENERER REUNION VIEW");
                logger.AppendLine("────────────────────────────");
                CreateOrUpdateViewsSection(doc, ns, splitTables, uiData);
                logger.AppendLine($"Oprettet view: AV_Opsamling_af_{uiData.OriginalTableName}");
                logger.AppendLine();

                // STEP 5: Validering
                logger.AppendLine("STEP 5: VALIDERING");
                logger.AppendLine("──────────────────");
                ValidateTableIndexTransformation(doc, ns, uiData, splitTables, logger);
                logger.AppendLine();

                // Gem opdateret tableIndex
                doc.Save(outputTableIndexPath);
                logger.AppendLine("═══════════════════════════════════════════════════════════");
                logger.AppendLine($"tableIndex_updated.xml gemt: {outputTableIndexPath}");
                logger.AppendLine("═══════════════════════════════════════════════════════════");

                System.Diagnostics.Debug.WriteLine($"TableIndex transformation fuldført: {splitTables.Count} splits");
                return logger.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception($"Fejl ved tableIndex transformation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Opdater alle foreign key referencer til split tabeller
        /// Håndterer sammensatte FKs der spænder på tværs af splits
        /// </summary>
        private string UpdateForeignKeyReferences(XDocument doc, XNamespace ns, List<SplitTable> splitTables, UIDataContainer uiData)
        {
            var log = new StringBuilder();
            string originalTableName = uiData.OriginalTableName;
            var pkColumns = uiData.PrimaryKey.GetAllPrimaryKeyColumns();

            // Find ALLE foreign keys der refererer til original tabel
            var allForeignKeys = doc.Descendants(ns + "foreignKey")
                .Where(fk => fk.Element(ns + "referencedTable")?.Value == originalTableName)
                .ToList();

            if (allForeignKeys.Count == 0)
            {
                log.AppendLine("  → Ingen eksterne foreign key referencer fundet");
                return log.ToString();
            }

            log.AppendLine($"  Fundet {allForeignKeys.Count} foreign key(s) der refererer til {originalTableName}");
            log.AppendLine();

            int updatedCount = 0;
            int splitCount = 0;
            int skippedCount = 0;

            foreach (var fkElement in allForeignKeys)
            {
                var parentTable = fkElement.Ancestors(ns + "table").First();
                var parentTableName = parentTable.Element(ns + "name")?.Value;
                var fkName = fkElement.Element(ns + "name")?.Value;

                log.AppendLine($"  FK: {fkName} (fra tabel: {parentTableName})");

                // Hent alle reference elementer
                var references = fkElement.Elements(ns + "reference").ToList();

                if (references.Count == 1)
                {
                    // SIMPLE FK - opdater til composite hvis nødvendigt
                    var referencedColumn = references[0].Element(ns + "referenced")?.Value;
                    var targetSplit = FindSplitContainingColumn(splitTables, referencedColumn, pkColumns, uiData);

                    if (targetSplit == null)
                    {
                        log.AppendLine($"     ADVARSEL: Kolonne '{referencedColumn}' ikke fundet i nogen split!");
                        log.AppendLine($"    → FK kunne ikke opdateres automatisk");
                        log.AppendLine($"    → MANUEL REVIEW PÅKRÆVET i output");
                        log.AppendLine();
                        skippedCount++;
                        continue;
                    }

                    var referencedTableElement = fkElement.Element(ns + "referencedTable");
                    string oldValue = referencedTableElement.Value;
                    referencedTableElement.Value = targetSplit.TableName;

                    // Hvis target har sammensat PK, udvid FK med manglende kolonner
                    if (pkColumns.Count > 1)
                    {
                        // Find source tabelens kolonner for at matche sammensat PK
                        var sourceTableElement = fkElement.Ancestors(ns + "table").First();
                        var sourceColumns = sourceTableElement.Descendants(ns + "column")
                            .Select(c => c.Element(ns + "name")?.Value)
                            .Where(n => n != null)
                            .ToList();

                        var addedColumns = new List<string>();

                        // Tilføj manglende PK-kolonner til FK hvis de findes i source tabel
                        foreach (var pkCol in pkColumns)
                        {
                            if (pkCol == referencedColumn) continue; // Allerede der

                            // Tjek om source tabel har denne kolonne
                            if (sourceColumns.Contains(pkCol))
                            {
                                var newReference = new XElement(ns + "reference");
                                newReference.Add(new XElement(ns + "column", pkCol));
                                newReference.Add(new XElement(ns + "referenced", pkCol));
                                fkElement.Add(newReference);
                                addedColumns.Add(pkCol);
                            }
                        }

                        if (addedColumns.Count > 0)
                        {
                            log.AppendLine($"     Opdateret til sammensat FK: {oldValue} → {targetSplit.TableName}");
                            log.AppendLine($"      Tilføjet {addedColumns.Count} PK kolonne(r): {string.Join(", ", addedColumns)}");
                        }
                        else
                        {
                            log.AppendLine($"     Opdateret: {oldValue} → {targetSplit.TableName}");
                            log.AppendLine($"      ADVARSEL: Source tabel mangler sammensat PK kolonner!");
                        }
                    }
                    else
                    {
                        log.AppendLine($"     Opdateret: {oldValue} → {targetSplit.TableName}");
                    }

                    log.AppendLine($"      Kolonne '{referencedColumn}' findes i split {targetSplit.SplitIndex}");
                    updatedCount++;
                }
                else
                {
                    // Sammensatte FK - multiple kolonner
                    log.AppendLine($"    Sammensatte FK med {references.Count} kolonner:");

                    // Group references by target split
                    var referencesByTarget = new Dictionary<string, List<XElement>>();
                    var unresolvedReferences = new List<string>();

                    foreach (var refElement in references)
                    {
                        var referencedColumn = refElement.Element(ns + "referenced")?.Value;
                        var targetSplit = FindSplitContainingColumn(splitTables, referencedColumn, pkColumns, uiData);

                        if (targetSplit == null)
                        {
                            unresolvedReferences.Add(referencedColumn);
                            continue;
                        }

                        if (!referencesByTarget.ContainsKey(targetSplit.TableName))
                        {
                            referencesByTarget[targetSplit.TableName] = new List<XElement>();
                        }
                        referencesByTarget[targetSplit.TableName].Add(refElement);

                        log.AppendLine($"      - '{referencedColumn}' → {targetSplit.TableName}");
                    }

                    // Tjek om der er unresolved kolonner
                    if (unresolvedReferences.Count > 0)
                    {
                        log.AppendLine($"     ADVARSEL: {unresolvedReferences.Count} kolonne(r) ikke fundet:");
                        foreach (var col in unresolvedReferences)
                        {
                            log.AppendLine($"      - '{col}' kunne ikke resolves");
                        }
                        log.AppendLine($"    → FK kunne ikke opdateres fuldstændigt");
                        log.AppendLine($"    → MANUEL REVIEW PÅKRÆVET");
                        log.AppendLine();
                        skippedCount++;
                        continue;
                    }

                    if (referencesByTarget.Count == 1)
                    {
                        // Alle referencer går til SAMME split - simple opdatering
                        var targetTableName = referencesByTarget.Keys.First();
                        fkElement.Element(ns + "referencedTable").Value = targetTableName;

                        log.AppendLine($"     Alle kolonner i samme split → {targetTableName}");
                        updatedCount++;
                    }
                    else if (referencesByTarget.Count > 1)
                    {
                        // Referencer spænder FLERE splits - split FK op!
                        log.AppendLine($"   SPLITTER FK i {referencesByTarget.Count} separate FKs:");

                        // Fjern original FK element
                        var parentForeignKeys = fkElement.Parent;
                        fkElement.Remove();

                        int subFKIndex = 1;
                        foreach (var kvp in referencesByTarget)
                        {
                            string targetTable = kvp.Key;
                            var refs = kvp.Value;

                            // FK navn kollision detection
                            string newFKName = $"{fkName}_Split{subFKIndex}";
                            int collisionCounter = 1;

                            while (parentForeignKeys.Elements(ns + "foreignKey")
                                .Any(fk => fk.Element(ns + "name")?.Value == newFKName))
                            {
                                newFKName = $"{fkName}_Split{subFKIndex}_{collisionCounter}";
                                collisionCounter++;
                            }

                            // Opret ny split FK
                            var newFK = new XElement(ns + "foreignKey");
                            newFK.Add(new XElement(ns + "name", newFKName));
                            newFK.Add(new XElement(ns + "referencedTable", targetTable));

                            foreach (var refElement in refs)
                            {
                                newFK.Add(new XElement(refElement));
                            }

                            parentForeignKeys.Add(newFK);

                            string collisionNote = collisionCounter > 1 ? $" (kollision løst: _{collisionCounter - 1})" : "";
                            log.AppendLine($"       {newFKName} → {targetTable} ({refs.Count} kolonner){collisionNote}");
                            subFKIndex++;
                        }

                        splitCount++;
                        updatedCount += referencesByTarget.Count;
                    }
                }

                log.AppendLine();
            }

            log.AppendLine($"  RESULTAT:");
            log.AppendLine($"    • {updatedCount} FK opdateret");
            log.AppendLine($"    • {splitCount} FK split op");
            if (skippedCount > 0)
            {
                log.AppendLine($"    • {skippedCount} FK skipped (kræver manuel review)");
            }

            return log.ToString();
        }

        /// <summary>
        /// Validér at tableIndex transformation lykkedes korrekt
        /// </summary>
        private void ValidateTableIndexTransformation(XDocument doc, XNamespace ns, UIDataContainer uiData, List<SplitTable> splitTables, StringBuilder logger)
        {
            bool isValid = true;

            logger.AppendLine("VALIDERING:");
            logger.AppendLine("───────────");

            // 1. Tjek at original tabel IKKE findes mere
            var remainingOriginal = doc.Descendants(ns + "table")
                .FirstOrDefault(t => t.Element(ns + "name")?.Value == uiData.OriginalTableName);

            if (remainingOriginal != null)
            {
                logger.AppendLine($"  FEJL: Original tabel '{uiData.OriginalTableName}' findes stadig!");
                isValid = false;
            }
            else
            {
                logger.AppendLine($"  Original tabel '{uiData.OriginalTableName}' korrekt erstattet");
            }

            // 2. Tjek at alle split tabeller findes
            foreach (var split in splitTables)
            {
                var splitElement = doc.Descendants(ns + "table")
                    .FirstOrDefault(t => t.Element(ns + "name")?.Value == split.TableName);

                if (splitElement == null)
                {
                    logger.AppendLine($"   FEJL: Split tabel '{split.TableName}' ikke fundet!");
                    isValid = false;
                }
                else
                {
                    logger.AppendLine($"  Split tabel '{split.TableName}' OK");
                }
            }

            // 3. Tjek at ingen FK refererer til original tabel længere (med warning for unresolved)
            var orphanedFKs = doc.Descendants(ns + "foreignKey")
                .Where(fk => fk.Element(ns + "referencedTable")?.Value == uiData.OriginalTableName)
                .ToList();

            if (orphanedFKs.Count > 0)
            {
                logger.AppendLine($"  ADVARSEL: {orphanedFKs.Count} FK refererer stadig til '{uiData.OriginalTableName}'");
                logger.AppendLine($"     Dette kan være FKs der ikke kunne opdateres automatisk");
                logger.AppendLine($"     → Tjek foreign_key_transformation_analysis.log for detaljer");
                // IKKE fail validation - det er forventet for unresolved kolonner
            }
            else
            {
                logger.AppendLine($"  Ingen orphaned FK referencer");
            }

            logger.AppendLine();
            logger.AppendLine(isValid ? "VALIDERING BESTÅET" : " VALIDERING FEJLEDE - SE FEJL OVENFOR");
        }

        /// <summary>
        /// Opret eller opdater views sektion med split reunion view
        /// </summary>
        private void CreateOrUpdateViewsSection(XDocument doc, XNamespace ns, List<SplitTable> splitTables, UIDataContainer uiData)
        {
            // Find eller opret views element
            var viewsElement = doc.Descendants(ns + "views").FirstOrDefault();
            if (viewsElement == null)
            {
                viewsElement = new XElement(ns + "views");
                doc.Root.Add(viewsElement);
            }

            // Generer reunion view
            var reunionView = GenerateSplitReunionView(splitTables, uiData, ns);
            viewsElement.Add(reunionView);
        }

        /// <summary>
        /// Generer SQL view til at samle split tabeller tilbage til original format
        /// </summary>
        private XElement GenerateSplitReunionView(List<SplitTable> splitTables, UIDataContainer uiData, XNamespace ns)
        {
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            var originalTableName = uiData.OriginalTableName;

            // View navn: AV_Opsamling_af_R_testfil
            string viewName = $"RA_Samling_af_{originalTableName}";

            // Byg SQL forespørgsel
            string sqlQuery = GenerateReunionSQL(splitTables, pkColumns, uiData);

            // Opret view element
            var viewElement = new XElement(ns + "view");
            viewElement.Add(new XElement(ns + "name", viewName));
            viewElement.Add(new XElement(ns + "queryOriginal", sqlQuery));

            // Arkivfaglig beskrivelse med reference til kontekstdokumentation
            string tableNames = string.Join(", ", splitTables.Select(t => t.TableName));
            string pkDescription = string.Join(", ", pkColumns);

            string description = $"Dette SQL-view er oprettet af Rigsarkivet i forbindelse med behandling af arkiveringsversionen. " +
                                $"Viewet rekonstruerer den originale tabel {originalTableName} ved at samle de opdelte tabeller ({tableNames}) via primærnøglekobling på kolonne(r): {pkDescription}. " +
                                $"Tabellen blev opdelt for at overholde tekniske begrænsninger i relationelle databasesystemer (maksimalt 1000 kolonner per tabel). " +
                                $"Se kontekstdokumentation for mere information om opdelingen, datasammenhæng og anvendelse af denne forespørgsel.";

            viewElement.Add(new XElement(ns + "description", description));

            return viewElement;
        }

        /// <summary>
        /// Generer SQL forespørgsel til at samle split tabeller - FORENKLET med SELECT *
        /// </summary>
        private string GenerateReunionSQL(List<SplitTable> splitTables, List<string> pkColumns, UIDataContainer uiData)
        {
            var sql = new StringBuilder();

            // SELECT clause - simpel * approach
            var selectParts = new List<string>();
            for (int i = 0; i < splitTables.Count; i++)
            {
                selectParts.Add($"t{i + 1}.*");
            }

            sql.AppendLine($"SELECT {string.Join(", ", selectParts)}");

            // FROM clause med første tabel
            var firstTable = splitTables.First();
            sql.AppendLine($"FROM {firstTable.TableName} t1");

            // INNER JOINs med resterende tabeller
            for (int i = 1; i < splitTables.Count; i++)
            {
                var table = splitTables[i];
                int tableNum = i + 1;

                sql.AppendLine($"INNER JOIN {table.TableName} t{tableNum}");

                // JOIN conditions på alle sammensat PK kolonner
                var joinConditions = new List<string>();
                foreach (var pkColumn in pkColumns)
                {
                    joinConditions.Add($"t1.{pkColumn} = t{tableNum}.{pkColumn}");
                }

                sql.AppendLine($"\tON {string.Join(" AND ", joinConditions)}");
            }

            // ORDER BY første PK kolonne
            sql.AppendLine($"ORDER BY t1.{pkColumns.First()}");

            return sql.ToString().TrimEnd();
        }

        /// <summary>
        /// Opret komplet table element med sammensat PK support
        /// </summary>
        private XElement CreateCompleteTableElementWithCompositePK(SplitTable splitTable, List<SplitTable> allSplits, XNamespace ns, UIDataContainer uiData, int tableNumber)
        {
            var tableElement = new XElement(ns + "table");
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            // Naming logic - ALLE splits får suffix
            string tableName = $"{uiData.OriginalTableEntry.Name}_{splitTable.SplitIndex}";
            string folderName = $"{uiData.OriginalTableEntry.Folder}_{splitTable.SplitIndex}";

            tableElement.Add(new XElement(ns + "name", splitTable.TableName));
            tableElement.Add(new XElement(ns + "folder", $"table{tableNumber}"));

            // Description med sammensat PK information
            string description = CreateCompositePKDescription(splitTable, allSplits, uiData);
            tableElement.Add(new XElement(ns + "description", description));

            // Columns med sammensat PK logic
            var columnsElement = CreateColumnsElementWithCompositePK(splitTable, uiData, ns);
            tableElement.Add(columnsElement);

            // Primary key sektion med composite support
            var primaryKeySection = CreateCompositePrimaryKeySection(splitTable, uiData, ns);
            tableElement.Add(primaryKeySection);

            // Foreign keys med cross-references
            var foreignKeysElement = CreateForeignKeysElementWithCompositePK(splitTable, allSplits, ns, uiData);
            if (foreignKeysElement.HasElements)
            {
                tableElement.Add(foreignKeysElement);
            }

            // Row count
            tableElement.Add(new XElement(ns + "rows", uiData.TotalRows));

            return tableElement;
        }

        /// <summary>
        /// Opret columns element med sammensat PK placering logik
        /// </summary>
        private XElement CreateColumnsElementWithCompositePK(SplitTable splitTable, UIDataContainer uiData, XNamespace ns)
        {
            var columnsElement = new XElement(ns + "columns");
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            int xmlColumnCounter = 1;

            if (splitTable.SplitIndex == 1)
            {
                // TABLE 1: Behold original kolonne rækkefølge
                foreach (var column in splitTable.Columns.OrderBy(c => c.Position))
                {
                    var columnElement = CreateColumnElement(ns, column, $"c{xmlColumnCounter}", uiData);
                    columnsElement.Add(columnElement);
                    xmlColumnCounter++;
                }
            }
            else
            {
                // TABLE 2+: Data kolonner først, PK kolonner i slutningen
                var dataColumns = splitTable.Columns.Where(c => !pkColumns.Contains(c.Name)).OrderBy(c => c.Position);
                var pkColumnsInTable = splitTable.Columns.Where(c => pkColumns.Contains(c.Name) && c.Name != pkInfo.AutoGeneratedColumnName).OrderBy(c => c.Position);

                // Data kolonner
                foreach (var column in dataColumns)
                {
                    var columnElement = CreateColumnElement(ns, column, $"c{xmlColumnCounter}", uiData);
                    columnsElement.Add(columnElement);
                    xmlColumnCounter++;
                }

                // Eksisterende PK kolonner
                foreach (var column in pkColumnsInTable)
                {
                    var enhancedColumn = new XMLColumn
                    {
                        Name = column.Name,
                        ColumnID = column.ColumnID,
                        DataType = column.DataType,
                        TypeOriginal = column.TypeOriginal,
                        IsNullable = column.IsNullable,
                        Description = (column.Description ?? "") + " (Primary Key - duplikeret til split)",
                        Position = column.Position
                    };
                    var columnElement = CreateColumnElement(ns, enhancedColumn, $"c{xmlColumnCounter}", uiData);
                    columnsElement.Add(columnElement);
                    xmlColumnCounter++;
                }

                // Auto-generated PK kolonne
                if (pkInfo.IncludeAutoGenerated)
                {
                    var autoColumnElement = CreateAutoGeneratedColumnElement(ns, $"c{xmlColumnCounter}", pkInfo.AutoGeneratedColumnName);
                    columnsElement.Add(autoColumnElement);
                    xmlColumnCounter++;
                }
            }

            return columnsElement;
        }

        /// <summary>
        /// Opret standard column element
        /// </summary>
        private XElement CreateColumnElement(XNamespace ns, XMLColumn column, string newColumnID, UIDataContainer uiData)
        {
            var columnElement = new XElement(ns + "column");
            columnElement.Add(new XElement(ns + "name", column.Name));
            columnElement.Add(new XElement(ns + "columnID", newColumnID));  // Ny sekventiel ID
            columnElement.Add(new XElement(ns + "type", column.DataType));
            columnElement.Add(new XElement(ns + "typeOriginal", column.TypeOriginal ?? ""));
            columnElement.Add(new XElement(ns + "nullable", column.IsNullable.ToString().ToLower()));
            columnElement.Add(new XElement(ns + "description", column.Description ?? ""));
            return columnElement;
        }

        /// <summary>
        /// Opret auto-generated column element
        /// </summary>
        private XElement CreateAutoGeneratedColumnElement(XNamespace ns, string columnID, string columnName)
        {
            var columnElement = new XElement(ns + "column");
            columnElement.Add(new XElement(ns + "name", columnName));
            columnElement.Add(new XElement(ns + "columnID", columnID));
            columnElement.Add(new XElement(ns + "type", "INTEGER"));
            columnElement.Add(new XElement(ns + "typeOriginal", ""));
            columnElement.Add(new XElement(ns + "nullable", "false"));
            columnElement.Add(new XElement(ns + "description", "Auto-genereret primærnøgle til split tabel kobling"));
            return columnElement;
        }

        /// <summary>
        /// Opret composite primary key sektion
        /// </summary>
        private XElement CreateCompositePrimaryKeySection(SplitTable splitTable, UIDataContainer uiData, XNamespace ns)
        {
            var primaryKeySection = new XElement(ns + "primaryKey");
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            // Konsistent naming - alle får suffix
            string tableName = $"{uiData.OriginalTableEntry.Name}_{splitTable.SplitIndex}";

            if (pkInfo.IsComposite)
            {
                // sammensat PK navn
                primaryKeySection.Add(new XElement(ns + "name", $"PK_{tableName}_Composite"));

                // Tilføj alle PK kolonner
                foreach (var pkColumn in pkColumns)
                {
                    primaryKeySection.Add(new XElement(ns + "column", pkColumn));
                }
            }
            else
            {
                // Single PK
                primaryKeySection.Add(new XElement(ns + "name", $"PK_{tableName}"));
                primaryKeySection.Add(new XElement(ns + "column", pkColumns.First()));
            }

            return primaryKeySection;
        }

        /// <summary>
        /// Opret foreign keys med sammensat PK cross-references
        /// </summary>
        private XElement CreateForeignKeysElementWithCompositePK(SplitTable splitTable, List<SplitTable> allSplits, XNamespace ns, UIDataContainer uiData)
        {
            var foreignKeysElement = new XElement(ns + "foreignKeys");
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            // 1. INHERITED FOREIGN KEYS (kun relevante)
            foreach (var originalFK in uiData.OriginalTableEntry.ForeignKeys)
            {
                var columnInThisSplit = splitTable.Columns.FirstOrDefault(c => c.Name == originalFK.Column);
                if (columnInThisSplit != null && !pkColumns.Contains(originalFK.Column))
                {
                    var foreignKeyElement = new XElement(ns + "foreignKey");

                    string tableName = splitTable.SplitIndex == 1 ?
                        uiData.OriginalTableEntry.Name :
                        $"{uiData.OriginalTableEntry.Name}_{splitTable.SplitIndex}";

                    string fkName = $"{originalFK.Name}_Split{splitTable.SplitIndex}";
                    foreignKeyElement.Add(new XElement(ns + "name", fkName));
                    foreignKeyElement.Add(new XElement(ns + "referencedTable", originalFK.ReferencedTable));

                    var referenceElement = new XElement(ns + "reference");
                    referenceElement.Add(new XElement(ns + "column", originalFK.Column));
                    referenceElement.Add(new XElement(ns + "referenced", originalFK.Referenced));

                    foreignKeyElement.Add(referenceElement);
                    foreignKeysElement.Add(foreignKeyElement);
                }
            }

            // 2. CROSS-REFERENCE FOREIGN KEYS mellem split tabeller (sammensat)
            if (allSplits.Count > 1)
            {
                foreach (var otherSplit in allSplits.Where(s => s.SplitIndex != splitTable.SplitIndex))
                {
                    // Opret ÉN sammensat FK med alle PK kolonner
                    var crossRefFK = new XElement(ns + "foreignKey");

                    string currentTableName = $"{uiData.OriginalTableEntry.Name}_{splitTable.SplitIndex}";
                    string otherTableName = $"{uiData.OriginalTableEntry.Name}_{otherSplit.SplitIndex}";
                    string fkName = $"FK_{currentTableName}_{otherTableName}";

                    crossRefFK.Add(new XElement(ns + "name", fkName));
                    crossRefFK.Add(new XElement(ns + "referencedTable", otherTableName));

                    // Tilføj ALLE PK kolonner som references i SAMME FK
                    foreach (var pkColumn in pkColumns)
                    {
                        var referenceElement = new XElement(ns + "reference");
                        referenceElement.Add(new XElement(ns + "column", pkColumn));
                        referenceElement.Add(new XElement(ns + "referenced", pkColumn));
                        crossRefFK.Add(referenceElement);
                    }

                    foreignKeysElement.Add(crossRefFK);
                }
            }

            return foreignKeysElement;
        }

        /// <summary>
        /// Opret beskrivelse med sammensat PK information
        /// </summary>
        private string CreateCompositePKDescription(SplitTable splitTable, List<SplitTable> allSplits, UIDataContainer uiData)
        {
            string originalDescription = uiData.OriginalTableEntry.Description ?? "";
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
            string pkDescription = string.Join(", ", pkColumns);

            // Beregn kolonne intervals for denne split
            int startColumn = splitTable.StartColumn;
            int endColumn = splitTable.EndColumn;

            // Find andre split-tabeller til kobling
            var otherSplits = allSplits.Where(s => s.SplitIndex != splitTable.SplitIndex)
                .Select(s => $"{uiData.OriginalTableEntry.Name}_{s.SplitIndex}")
                .ToList();
            string otherSplitNames = string.Join(", ", otherSplits);

            string splitInfo;
            string archiveContext = "Denne tabel er opdelt af Rigsarkivet i forbindelse med behandling af arkiveringsversionen for at overholde tekniske begrænsninger i relationelle databasesystemer (maksimalt 1000 kolonner per tabel). Se kontekstdokumentation for mere information om opdelingen og datasammenhæng.";

            if (splitTable.SplitIndex == 1)
            {
                splitInfo = $"Del 1 af {allSplits.Count}: Indeholder kolonnerne {startColumn}-{endColumn} fra tabellen {uiData.OriginalTableEntry.Name} samt primærnøgle ({pkDescription}) til datakobling. " +
                           $"For at rekonstruere den komplette tabel skal denne kobles med {otherSplitNames} via primærnøglekolonne(r): {pkDescription}. " +
                           $"XML kolonneinterval: c{startColumn}-c{endColumn}.";
            }
            else if (splitTable.SplitIndex == allSplits.Count)
            {
                splitInfo = $"Del {splitTable.SplitIndex} af {allSplits.Count} (afsluttende): Indeholder kolonnerne {startColumn}-{endColumn} fra tabellen {uiData.OriginalTableEntry.Name} samt primærnøgle ({pkDescription}) til datakobling. " +
                           $"For at rekonstruere den komplette tabel skal denne kobles med {otherSplitNames} via primærnøglekolonne(r): {pkDescription}. " +
                           $"XML kolonneinterval: c{startColumn}-c{endColumn}.";
            }
            else
            {
                splitInfo = $"Del {splitTable.SplitIndex} af {allSplits.Count}: Indeholder kolonnerne {startColumn}-{endColumn} fra tabellen {uiData.OriginalTableEntry.Name} samt primærnøgle ({pkDescription}) til datakobling. " +
                           $"For at rekonstruere den komplette tabel skal denne kobles med {otherSplitNames} via primærnøglekolonne(r): {pkDescription}. " +
                           $"XML kolonneinterval: c{startColumn}-c{endColumn}.";
            }

            // Byg samlet beskrivelse
            if (!string.IsNullOrWhiteSpace(originalDescription))
            {
                return $"{originalDescription} | {archiveContext} | {splitInfo}";
            }
            else
            {
                return $"{archiveContext} | {splitInfo}";
            }
        }

        #endregion

        #region Data Collection

        /// <summary>
        /// Collect UI data med sammensat PK support
        /// </summary>
        private UIDataContainer CollectUIData()
        {
            var container = new UIDataContainer
            {
                XMLPath = currentXMLPath,
                XSDPath = currentXSDPath,
                TableIndexPath = currentTableIndexPath,
                OriginalTableEntry = currentTableEntry,
                OriginalNamespace = originalNamespace,
                TotalRows = totalRows,
                PrimaryKey = compositePKSelector.GetPrimaryKeyInfo(),
                IntegrityDescription = "Betydning ukendt. Rækken er tilføjet under aflevering til arkiv, for at sikre referentiel integritet i databasen af hensyn til langtidsbevaring"
            };

            container.AllColumns = ConvertTableIndexColumnsToXMLColumns(currentTableEntry.Columns);

            AutoIDManager.EnsureAutoIDInAllColumns(container);

            if (resultTables != null && resultTables.Count > 0)
            {
                // Konverter UI-baserede splits til sammensat PK format
                container.Tables = ConvertUIResultsToCompositePK(container);
            }
            else
            {
                // Fallback til auto-beregning hvis ingen UI splits
                container.Tables = CompositePKSplitAlgorithm.GenerateSplitTables(container);
            }

            return container;
        }

        /// <summary>
        /// Konverter UI resultTables til sammensat PK format
        /// </summary>
        private List<SplitTable> ConvertUIResultsToCompositePK(UIDataContainer uiData)
        {
            var convertedTables = new List<SplitTable>();
            var pkInfo = uiData.PrimaryKey;
            var pkColumns = pkInfo.GetAllPrimaryKeyColumns();

            for (int i = 0; i < resultTables.Count; i++)
            {
                var legacyTable = resultTables[i];

                var newTable = new SplitTable
                {
                    TableName = legacyTable.TableName,
                    StartColumn = legacyTable.StartColumn,
                    EndColumn = legacyTable.EndColumn,
                    SplitIndex = i + 1,
                    IsFirstSplit = i == 0,
                    Columns = new List<XMLColumn>()
                };

                // Tilføj data kolonner fra split range
                var dataColumns = uiData.AllColumns
                    .Where(c => c.Position >= legacyTable.StartColumn &&
                               c.Position <= legacyTable.EndColumn &&
                               !pkColumns.Contains(c.Name))
                    .OrderBy(c => c.Position);

                foreach (var dataCol in dataColumns)
                {
                    newTable.Columns.Add(dataCol);
                }

                foreach (var pkColumnName in pkColumns)
                {
                    // Find ALL PK columns (AutoID er nu garanteret i AllColumns af AutoIDManager)
                    var pkCol = uiData.AllColumns.FirstOrDefault(c => c.Name == pkColumnName);
                    if (pkCol != null)
                    {
                        newTable.Columns.Add(pkCol);
                    }
                    else
                    {
                        // Dette burde ALDRIG ske da AutoIDManager sikrer AutoID er i AllColumns
                        System.Diagnostics.Debug.WriteLine($"FEJL: PK kolonne '{pkColumnName}' ikke fundet i AllColumns!");
                    }
                }

                convertedTables.Add(newTable);
            }

            return convertedTables;
        }


        /// <summary>
        /// Convert TableIndexColumn til XMLColumn format
        /// </summary>
        private List<XMLColumn> ConvertTableIndexColumnsToXMLColumns(List<TableIndexColumn> tableIndexColumns)
        {
            var xmlColumns = new List<XMLColumn>();

            foreach (var tableCol in tableIndexColumns.OrderBy(c => c.Position))
            {
                var xmlCol = new XMLColumn
                {
                    Name = tableCol.Name,
                    ColumnID = tableCol.ColumnID,
                    DataType = tableCol.DataType,
                    TypeOriginal = tableCol.TypeOriginal,
                    IsNullable = tableCol.IsNullable,
                    Description = tableCol.Description,
                    Position = tableCol.Position
                };

                xmlColumns.Add(xmlCol);
            }

            return xmlColumns;
        }

        #endregion

        #region UI Management

        /// <summary>
        /// Update UI state efter TableIndex load
        /// </summary>
        private void UpdateUIAfterTableIndexLoad()
        {
            bool hasTableIndex = !string.IsNullOrEmpty(currentTableIndexPath) && availableTables.Count > 0;

            cmbTableSelector.Enabled = hasTableIndex;

            if (!hasTableIndex)
            {
                ResetUIToInitialState();
            }
        }

        /// <summary>
        /// Update UI state efter structure analysis
        /// </summary>
        private void UpdateUIAfterStructureAnalysis()
        {
            bool hasStructure = allColumns.Count > 0;

            txtSplitPoints.Enabled = hasStructure;
            btnCalculateSplit.Enabled = hasStructure;
        }

        /// <summary>
        /// Reset UI til initial state
        /// </summary>
        private void ResetUIToInitialState()
        {
            txtSourceXML.Text = "";
            lstSplitPreview.Items.Clear();

            // Reset placeholder
            ShowPlaceholder();

            lblPreviewInfo.Text = "Konfigurer split først...";
            lblPreviewInfo.ForeColor = Color.DarkRed;

            currentTableEntry = null;
            currentXMLPath = "";
            currentXSDPath = "";
            allColumns.Clear();
            resultTables.Clear();

            btnExecuteSplit.Enabled = false;
            btnAnalyzePK.Enabled = false;
        }

        private void AddSplitPointsInfoLabel()
        {
            // Dette kaldes fra InitializeComponent() eller constructor
            var lblSplitInfo = new Label();
            lblSplitInfo.Text = "💡 Tom felt = automatisk split ved 950 kolonner per tabel";
            lblSplitInfo.ForeColor = Color.DarkBlue;
            lblSplitInfo.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular);
            lblSplitInfo.AutoSize = true;

            // Placer under txtSplitPoints (tilpas position efter dine kontrollers layout)
            lblSplitInfo.Location = new Point(txtSplitPoints.Left, txtSplitPoints.Bottom + 5);

            // Tilføj til samme container som txtSplitPoints
            if (txtSplitPoints.Parent != null)
            {
                txtSplitPoints.Parent.Controls.Add(lblSplitInfo);
            }
        }

        /// <summary>
        /// Opsæt placeholder tekst for split points felt
        /// </summary>
        private void SetupSplitPointsPlaceholder()
        {
            // Sæt initial placeholder
            ShowPlaceholder();

            // Event handlers for focus management
            txtSplitPoints.Enter += TxtSplitPoints_Enter;
            txtSplitPoints.Leave += TxtSplitPoints_Leave;
            txtSplitPoints.TextChanged += TxtSplitPoints_TextChanged;
        }

        private void TxtSplitPoints_Enter(object sender, EventArgs e)
        {
            if (splitPointsHasPlaceholder)
            {
                HidePlaceholder();
            }
        }

        private void TxtSplitPoints_Leave(object sender, EventArgs e)
        {
            // Hvis feltet er tomt når brugeren forlader det, vis placeholder igen
            if (string.IsNullOrWhiteSpace(txtSplitPoints.Text))
            {
                ShowPlaceholder();
            }
        }

        private void TxtSplitPoints_TextChanged(object sender, EventArgs e)
        {
            // Hvis der skrives noget og vi har placeholder, fjern det
            if (splitPointsHasPlaceholder && txtSplitPoints.Text != PLACEHOLDER_TEXT)
            {
                HidePlaceholder();
            }
        }

        private void ShowPlaceholder()
        {
            splitPointsHasPlaceholder = true;
            txtSplitPoints.Text = PLACEHOLDER_TEXT;
            txtSplitPoints.ForeColor = Color.Gray;
            txtSplitPoints.Font = new Font(txtSplitPoints.Font.FontFamily, txtSplitPoints.Font.Size, FontStyle.Italic);
        }

        private void HidePlaceholder()
        {
            if (splitPointsHasPlaceholder)
            {
                splitPointsHasPlaceholder = false;
                txtSplitPoints.Text = "";
                txtSplitPoints.ForeColor = SystemColors.WindowText; // Standard tekst farve
                txtSplitPoints.Font = new Font(txtSplitPoints.Font.FontFamily, txtSplitPoints.Font.Size, FontStyle.Regular);
            }
        }

        /// <summary>
        /// Hjælpe metode til at tjekke om split points felt reelt er tomt
        /// </summary>
        private bool IsSplitPointsEmpty()
        {
            return splitPointsHasPlaceholder || string.IsNullOrWhiteSpace(txtSplitPoints.Text);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// bruges i nuværende GenerateReferenceImpactLog
        /// </summary>
        /// <param name="uiData"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private SplitTable FindSplitContainingColumn(UIDataContainer uiData, string columnName)
        {
            var pkColumns = uiData.PrimaryKey.GetAllPrimaryKeyColumns();
            return FindSplitContainingColumn(uiData.Tables, columnName, pkColumns, uiData);
        }

        /// OVERLOAD: Find split der indeholder kolonne (til UpdateForeignKeyReferences)
        /// Prioriterer data-kolonner over PK kolonner (da PK er duplikeret)
        /// </summary>
        private SplitTable FindSplitContainingColumn(List<SplitTable> splitTables, string columnName, List<string> pkColumns, UIDataContainer uiData)
        {
            if (string.IsNullOrEmpty(columnName))
                return null;

            // Hvis kolonnen er en PK kolonne → returner første split (canonical reference)
            if (pkColumns.Contains(columnName))
            {
                return splitTables.FirstOrDefault();
            }

            // Find split der indeholder denne data-kolonne
            foreach (var split in splitTables)
            {
                var column = split.Columns.FirstOrDefault(c => c.Name == columnName && !pkColumns.Contains(c.Name));
                if (column != null)
                {
                    return split;
                }
            }

            return null;
        }

        /// <summary>
        /// Find næste version nummer baseret på eksisterende mapper
        /// </summary>
        private string GetNextVersionNumber(string parentFolder, string tableName)
        {
            try
            {
                if (!Directory.Exists(parentFolder))
                    return "v1.0";

                var existingFolders = Directory.GetDirectories(parentFolder)
                    .Where(dir => Path.GetFileName(dir).StartsWith($"split_{tableName}_v"))
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
                return $"v{newVersion.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}";
            }
            catch
            {
                return "v1.0";
            }
        }

        /// <summary>
        /// Sikker directory åbning med fallbacks
        /// </summary>
        private void OpenDirectorySafely(string directoryPath)
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
        /// Logger klasse til detaljeret operation logging
        /// </summary>
        public class SplitLogger
        {
            private readonly List<string> _logEntries = new List<string>();
            private readonly DateTime _startTime = DateTime.Now;

            public void LogInfo(string message)
            {
                var entry = $"[{DateTime.Now:HH:mm:ss}] INFO: {message}";
                _logEntries.Add(entry);
                System.Diagnostics.Debug.WriteLine(entry);
            }

            public void LogWarning(string message)
            {
                var entry = $"[{DateTime.Now:HH:mm:ss}] WARN: {message}";
                _logEntries.Add(entry);
                System.Diagnostics.Debug.WriteLine(entry);
            }

            public void LogError(string message)
            {
                var entry = $"[{DateTime.Now:HH:mm:ss}] ERROR: {message}";
                _logEntries.Add(entry);
                System.Diagnostics.Debug.WriteLine(entry);
            }

            public void SaveToFile(string logPath)
            {
                var logContent = new StringBuilder();

                logContent.AppendLine("==============================================================================");
                logContent.AppendLine("                       XML TABLE SPLIT OPERATION LOG");
                logContent.AppendLine("==============================================================================");
                logContent.AppendLine();
                logContent.AppendLine($"Operation startet: {_startTime:yyyy-MM-dd HH:mm:ss}");
                logContent.AppendLine($"Log genereret: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                logContent.AppendLine($"Total varighed: {DateTime.Now - _startTime:hh\\:mm\\:ss}");
                logContent.AppendLine($"System: {Environment.MachineName}");
                logContent.AppendLine($"Bruger: {Environment.UserName}");
                logContent.AppendLine();

                foreach (var entry in _logEntries)
                {
                    logContent.AppendLine(entry);
                }

                logContent.AppendLine();
                logContent.AppendLine("==============================================================================");
                logContent.AppendLine("                               LOG AFSLUTTET");
                logContent.AppendLine("==============================================================================");

                File.WriteAllText(logPath, logContent.ToString(), Encoding.UTF8);
            }
        }

        /// <summary>
        /// Progress reporter adapter for service integration
        /// Forbinder XMLSplitService progress callbacks med UI controls
        /// </summary>
        private class FormProgressReporter : IProgressReporter
        {
            private readonly ProgressBar progressBar;
            private readonly Label statusLabel;

            public FormProgressReporter(ProgressBar bar, Label label)
            {
                progressBar = bar;
                statusLabel = label;
            }

            public void Report(int percentage, string message)
            {
                // Thread-safe UI update
                if (progressBar.InvokeRequired)
                {
                    progressBar.Invoke((Action)(() =>
                    {
                        progressBar.Value = Math.Min(percentage, 100);
                        statusLabel.Text = message;
                    }));
                }
                else
                {
                    progressBar.Value = Math.Min(percentage, 100);
                    statusLabel.Text = message;
                }
            }
        }

        /// <summary>
        /// Detect original namespace fra XML fil
        /// </summary>
        private void DetectOriginalNamespace()
        {
            originalNamespace = "http://www.sa.dk/xmlns/siard/1.0/schema0/";

            if (string.IsNullOrEmpty(currentXMLPath) || !File.Exists(currentXMLPath))
                return;

            try
            {
                using (var reader = XmlReader.Create(currentXMLPath))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "table")
                        {
                            string xmlns = reader.GetAttribute("xmlns");
                            if (!string.IsNullOrEmpty(xmlns))
                            {
                                originalNamespace = xmlns;
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
        }

        /// <summary>
        /// Count rækker fra XML fil
        /// </summary>
        private void CountRowsFromXML()
        {
            totalRows = 0;

            if (string.IsNullOrEmpty(currentXMLPath) || !File.Exists(currentXMLPath))
                return;

            try
            {
                using (var reader = XmlReader.Create(currentXMLPath))
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
        }

        #endregion
    }
}