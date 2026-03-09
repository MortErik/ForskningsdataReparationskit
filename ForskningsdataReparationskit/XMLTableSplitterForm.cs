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
using ForskningsdataReparationskit.Controls;
using ForskningsdataReparationskit.Models;
using ForskningsdataReparationskit.Services;
using ForskningsdataReparationskit.Utilities;

namespace ForskningsdataReparationskit
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

        // Custom output path (null = brug Desktop default)
        private string customOutputPath = null;
        private const string OUTPUT_FOLDER_NAME = "XML_Table_Splits";

        private bool splitPointsHasPlaceholder = true;
        private const string PLACEHOLDER_TEXT = "Eksempel: 950, 1800, 2700 (eller lad stå tom for automatisk split)";

        #endregion

        #region Constructor

        public XMLTableSplitterForm()
        {
            InitializeComponent();
            LoadOutputPathPreference();
            // compositePKSelector er allerede initialiseret i Designer

            SetupSplitPointsPlaceholder();
            AddSplitPointsInfoLabel();

            // Tilføj Load event handler
            this.Load += XMLTableSplitterForm_Load;
        }

        /// <summary>
        /// Form Load event - kaldes EFTER WelcomeForm har tilføjet backPanel
        /// </summary>
        private void XMLTableSplitterForm_Load(object sender, EventArgs e)
        {
            AddOutputControlsToBackPanel();
            UpdateOutputPathLabel();
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
                //UpdateSplitPreview();
                btnCalculateSplit.Enabled = true;
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
                totalRows = XMLHelper.CountRowsFromXML(currentXMLPath);

                // Set original namespace
                originalNamespace = XMLNamespaceHelper.DetectNamespace(currentXMLPath);

                UpdateUIAfterStructureAnalysis();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved TableIndex structure analyse: {ex.Message}");
            }
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
                    MessageBox.Show($"Split punkter er over 975 kolonner:\n{validation}");
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
                                   totalColumnCount > 975 ? " - NÆRMER SIG 1000 KOLONNE GRÆNSE" : "";

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

        #region Primary Key Analysis

        /// <summary>
        /// PK Analyse - analysér combined unikhed
        /// Bruger nu PrimaryKeyAnalysisService
        /// </summary>
        private async void btnAnalyzePK_Click(object sender, EventArgs e)
        {
            var pkInfo = compositePKSelector.GetPrimaryKeyInfo();

            // Tjek om brugeren kun har valgt auto-generated AutoID
            if (pkInfo.IsOnlyAutoGenerated)
            {
                MessageBox.Show("AutoID genereres automatisk under split og er garanteret unik.\n\n" +
                               "Analyse er ikke nødvendig - du kan fortsætte direkte til split.",
                               "Auto-genereret Primærnøgle", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Tjek tableIndex først
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
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;
                progressBar.Maximum = 100;

                
                var pkAnalysisService = new PrimaryKeyAnalysisService();
                //var pkInfo = compositePKSelector.GetPrimaryKeyInfo();
                var pkColumns = pkInfo.GetAllPrimaryKeyColumns();
                

                lblPreviewInfo.Text = $"Analyserer {(pkInfo.IsComposite ? "sammensat" : "enkelt")} primærnøgle unikhed...";

                var analysisResult = await Task.Run(() =>
                    pkAnalysisService.AnalyzeCompositePrimaryKeyUniqueness(
                        currentXMLPath, pkColumns, currentTableEntry, pkInfo));

                progressBar.Visible = false;

                string message = pkAnalysisService.BuildPKAnalysisMessage(pkColumns, analysisResult.UniqueCount, analysisResult.TotalCount, analysisResult.NullCount);
                ShowPKAnalysisResult(message, analysisResult.UniqueCount, analysisResult.TotalCount, analysisResult.NullCount);
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
            }
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
                message += "ADVARSEL: Kombinationen er unik men har null værdier\n" +
                         "For arkiveringsversionen anbefales det at finde en anden mulig primærnøgle/primørnøgle kombination.";

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
                         $"Dette vil bryde referentiel integritet i arkiveringsversionen.\n\n";

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

        private async void btnExecuteSplit_Click(object sender, EventArgs e)
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
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;
                progressBar.Maximum = 100;

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
                await ExecuteSplitOperation(uiData);
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
                progressBar.Value = 0;
            }
        }

        /// <summary>
        /// Kør split operation med fuld logging og fil generering
        /// Bruger nu XMLSplitService orchestrator
        /// </summary>
        private async Task ExecuteSplitOperation(UIDataContainer uiData)
        {
            try
            {
                var splitService = new XMLSplitService();
                var logger = new SplitLogger();
                var progress = new FormProgressReporter(progressBar, lblPreviewInfo);

                var result = await Task.Run(() => splitService.ExecuteSplit(uiData, progress, logger, GetOutputBasePath()));

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
                        FileSystemHelper.OpenDirectorySafely(result.OutputDirectory);
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
        #endregion

        #region Output Directory Management

        /// <summary>
        /// Button handler: Vælg custom output directory
        /// </summary>
        private void btnChangeOutput_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Vælg output mappe for XML splits";
                folderDialog.SelectedPath = customOutputPath ??
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    customOutputPath = folderDialog.SelectedPath;
                    UpdateOutputPathLabel();

                    // TILFØJ disse 2 linjer:
                    Properties.Settings.Default.TableSplitterOutputPath = customOutputPath;
                    Properties.Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// Opdater output path label baseret på nuværende valg
        /// </summary>
        private void UpdateOutputPathLabel()
        {
            string displayPath;

            if (customOutputPath != null)
            {
                displayPath = $"{customOutputPath}/{OUTPUT_FOLDER_NAME}";
                lblOutputPath.ForeColor = Color.DarkBlue;
            }
            else
            {
                displayPath = $"Desktop/{OUTPUT_FOLDER_NAME}";
                lblOutputPath.ForeColor = Color.DarkGreen;
            }

            lblOutputPath.Text = $"Output destination: {displayPath}";
        }

        /// <summary>
        /// Tilføj output controls til eksisterende back panel (oprettet af WelcomeForm)
        /// </summary>
        private void AddOutputControlsToBackPanel()
        {
            // Find back button (oprettet af WelcomeForm.AddBackButtonWithDocking)
            var backButton = this.Controls.Find("backButton", true).FirstOrDefault();

            if (backButton != null && backButton.Parent is Panel backPanel)
            {
                // Fjern controls fra form (de blev tilføjet i Designer)
                this.Controls.Remove(lblOutputPath);
                this.Controls.Remove(btnChangeOutput);

                // Tilføj til back panel
                lblOutputPath.Dock = DockStyle.None;
                lblOutputPath.AutoSize = true;
                lblOutputPath.Location = new Point(200, 17);

                btnChangeOutput.Dock = DockStyle.Right;
                btnChangeOutput.Margin = new Padding(0, 7, 15, 7);

                backPanel.Controls.Add(lblOutputPath);
                backPanel.Controls.Add(btnChangeOutput);
            }
        }

        /// <summary>
        /// Hent base path for output (Desktop eller custom)
        /// </summary>
        private string GetOutputBasePath()
        {
            return customOutputPath ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        #endregion

        private void compositePKSelector_Load(object sender, EventArgs e)
        {

        }

        private void LoadOutputPathPreference()
        {
            string savedPath = Properties.Settings.Default.TableSplitterOutputPath;
            if (!string.IsNullOrEmpty(savedPath) && Directory.Exists(savedPath))
                customOutputPath = savedPath;
            UpdateOutputPathLabel();
        }
    }
}