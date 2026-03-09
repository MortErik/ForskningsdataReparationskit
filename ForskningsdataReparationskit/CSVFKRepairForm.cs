using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForskningsdataReparationskit
{
    public partial class CSVFKRepairForm : Form
    {
        #region Private Classes

        /// <summary>
        /// Repræsenterer et kolonne par til sammensatte PK support
        /// </summary>
        private class ColumnPair
        {
            public ComboBox ParentComboBox { get; set; }
            public ComboBox ChildComboBox { get; set; }
            public Label ParentLabel { get; set; }
            public Label ChildLabel { get; set; }
            public int PairNumber { get; set; }
        }

        #endregion

        #region Fields

        // FK repair fields
        private List<ColumnPair> columnPairs = new List<ColumnPair>();
        private int nextPairNumber = 2;
        private string currentMissingValuesFile = null; // Temp fil for missing values

        // Dialogs
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;

        #endregion

        #region Constructor

        public CSVFKRepairForm()
        {
            InitializeComponent();
            SetupCsvFKRepairEventHandlers();
            SetupListBoxContextMenu();
            InitializeDefaultColumnPair();

            this.FormClosing += (s, e) => CleanupTempFiles();
            this.FormClosed += (s, e) => CleanupTempFiles();
        }

        /// <summary>
        /// Ryd op i temp filer
        /// </summary>
        private void CleanupTempFiles()
        {
            if (!string.IsNullOrEmpty(currentMissingValuesFile) && File.Exists(currentMissingValuesFile))
            {
                try { File.Delete(currentMissingValuesFile); } catch { }
            }
        }

        #endregion

        #region Event Handler Setup

        /// <summary>
        /// Setup event handlers for CSV FK Repair controls
        /// </summary>
        private void SetupCsvFKRepairEventHandlers()
        {
            try
            {
                // Browse buttons
                if (btnBrowseParent != null)
                    btnBrowseParent.Click += btnBrowseParent_Click;

                if (btnBrowseChild != null)
                    btnBrowseChild.Click += btnBrowseChild_Click;

                // sammensatte PK buttons
                if (btnAddPrimaryKey != null)
                    btnAddPrimaryKey.Click += btnAddPrimaryKey_Click;

                if (btnRemovePrimaryKey != null)
                    btnRemovePrimaryKey.Click += btnRemovePrimaryKey_Click;

                // Analysis button
                if (btnAnalyzeFK != null)
                    btnAnalyzeFK.Click += btnAnalyzeFK_Click;

                // Generate/Export buttons
                if (btnGenerateDummies != null)
                    btnGenerateDummies.Click += btnGenerateDummies_Click;

                if (btnCopySelected != null)
                    btnCopySelected.Click += btnCopySelected_Click;

                if (btnExportMissing != null)
                    btnExportMissing.Click += btnExportMissing_Click;

                // Keyboard shortcut for listbox
                if (lstMissingValues != null)
                    lstMissingValues.KeyDown += (s, e) =>
                    {
                        if (e.Control && e.KeyCode == Keys.C)
                        {
                            btnCopySelected_Click(s, new EventArgs());
                            e.Handled = true;
                        }
                    };

                System.Diagnostics.Debug.WriteLine("[Setup] CSV FK Repair event handlers wired");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetupCsvFKRepairEventHandlers error: {ex.Message}");
            }
        }

        #endregion

        #region CSV FK Repair - File Selection

        /// <summary>
        /// Browse for parent CSV file
        /// </summary>
        private void btnBrowseParent_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV filer|*.csv|Alle filer|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtParentCSV.Text = openFileDialog1.FileName;
                AutoDetectColumns(openFileDialog1.FileName, true);
            }
        }

        /// <summary>
        /// Browse for child CSV file
        /// </summary>
        private void btnBrowseChild_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV filer|*.csv|Alle filer|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtChildCSV.Text = openFileDialog1.FileName;
                AutoDetectColumns(openFileDialog1.FileName, false);
            }
        }

        /// <summary>
        /// Auto-detect og populer column dropdowns med kolonnenavne
        /// </summary>
        private void AutoDetectColumns(string filePath, bool isParent)
        {
            try
            {
                using (var reader = new StreamReader(filePath, Encoding.UTF8, true))
                {
                    string headerLine = reader.ReadLine();
                    if (string.IsNullOrEmpty(headerLine)) return;

                    var headers = ParseCSVLine(headerLine);
                    var columnList = new List<string>();

                    for (int i = 0; i < headers.Length; i++)
                    {
                        string displayText = $"{headers[i].Trim()} (kolonne {i + 1})";
                        columnList.Add(displayText);
                    }

                    // Populer alle relevante dropdowns
                    if (isParent)
                    {
                        PopulateComboBox(cmbParentColumn, columnList);
                        foreach (var pair in columnPairs)
                        {
                            PopulateComboBox(pair.ParentComboBox, columnList);
                        }
                    }
                    else
                    {
                        PopulateComboBox(cmbChildColumn, columnList);
                        foreach (var pair in columnPairs)
                        {
                            PopulateComboBox(pair.ChildComboBox, columnList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved læsning af kolonner: {ex.Message}");
            }
        }

        /// <summary>
        /// PopulateComboBox med null checks
        /// </summary>
        private void PopulateComboBox(ComboBox comboBox, List<string> columnList)
        {
            if (comboBox == null || columnList == null) return;

            try
            {
                comboBox.Items.Clear();
                foreach (string column in columnList)
                {
                    comboBox.Items.Add(column);
                }
                if (comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PopulateComboBox error: {ex.Message}");
            }
        }

        /// <summary>
        /// GetSelectedColumnIndex med null checks
        /// </summary>
        private int GetSelectedColumnIndex(ComboBox comboBox)
        {
            if (comboBox?.SelectedItem == null) return -1;

            try
            {
                string selectedText = comboBox.SelectedItem.ToString();
                // Extract "(kolonne X)" delen
                int start = selectedText.LastIndexOf("(kolonne ") + 9;
                int end = selectedText.LastIndexOf(")");

                if (start > 8 && end > start)
                {
                    string columnNumberText = selectedText.Substring(start, end - start);
                    if (int.TryParse(columnNumberText, out int columnNumber))
                    {
                        return columnNumber - 1; // Convert til 0-based index
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetSelectedColumnIndex error: {ex.Message}");
            }

            return -1;
        }

        #endregion

        #region CSV FK Repair - sammensatte PK Support

        /// <summary>
        /// Initialiser default column pair (den første)
        /// </summary>
        private void InitializeDefaultColumnPair()
        {
            // Default pair er allerede i Designer (cmbParentColumn + cmbChildColumn)
            // Vi tracker dem som pair nummer 1
        }

        /// <summary>
        /// Tilføj nyt column pair til sammensatte PK
        /// </summary>
        private void btnAddPrimaryKey_Click(object sender, EventArgs e)
        {
            AddNewColumnPair();
        }

        /// <summary>
        /// Fjern sidste column pair
        /// </summary>
        private void btnRemovePrimaryKey_Click(object sender, EventArgs e)
        {
            RemoveLastColumnPair();
        }

        /// <summary>
        /// Skab et nyt column pair programmatisk
        /// </summary>
        private void AddNewColumnPair()
        {
            var pair = new ColumnPair { PairNumber = nextPairNumber };

            // Skab labels
            pair.ParentLabel = new Label
            {
                Text = $"Parent {nextPairNumber}:",
                Location = new Point(20, 15 + (nextPairNumber - 2) * 30),
                Size = new Size(70, 13),
                AutoSize = true
            };

            pair.ChildLabel = new Label
            {
                Text = $"Child {nextPairNumber}:",
                Location = new Point(380, 15 + (nextPairNumber - 2) * 30),
                Size = new Size(60, 13),
                AutoSize = true
            };

            // Skab ComboBoxes
            pair.ParentComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(105, 12 + (nextPairNumber - 2) * 30),
                Size = new Size(250, 21),
                Name = $"cmbParent{nextPairNumber}"
            };

            pair.ChildComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(460, 12 + (nextPairNumber - 2) * 30),
                Size = new Size(250, 21),
                Name = $"cmbChild{nextPairNumber}"
            };

            // Tilføj til panel
            pnlDynamicColumns.Controls.Add(pair.ParentLabel);
            pnlDynamicColumns.Controls.Add(pair.ChildLabel);
            pnlDynamicColumns.Controls.Add(pair.ParentComboBox);
            pnlDynamicColumns.Controls.Add(pair.ChildComboBox);

            // Auto-populer hvis filer allerede er valgt
            if (!string.IsNullOrEmpty(txtParentCSV.Text))
                AutoDetectColumns(txtParentCSV.Text, true);
            if (!string.IsNullOrEmpty(txtChildCSV.Text))
                AutoDetectColumns(txtChildCSV.Text, false);

            columnPairs.Add(pair);
            nextPairNumber++;

            UpdateRemoveButtonState();
        }

        /// <summary>
        /// Fjern sidste column pair
        /// </summary>
        private void RemoveLastColumnPair()
        {
            if (columnPairs.Count == 0) return;

            var lastPair = columnPairs.Last();

            // Fjern controls fra panel
            pnlDynamicColumns.Controls.Remove(lastPair.ParentLabel);
            pnlDynamicColumns.Controls.Remove(lastPair.ChildLabel);
            pnlDynamicColumns.Controls.Remove(lastPair.ParentComboBox);
            pnlDynamicColumns.Controls.Remove(lastPair.ChildComboBox);

            columnPairs.Remove(lastPair);
            nextPairNumber--;

            UpdateRemoveButtonState();
        }

        /// <summary>
        /// Opdater remove button enabled state
        /// </summary>
        private void UpdateRemoveButtonState()
        {
            btnRemovePrimaryKey.Enabled = columnPairs.Count > 0;
        }

        #endregion

        #region CSV FK Repair - Analysis

        /// <summary>
        /// Hovedmetode til at analysere FK relationships
        /// </summary>
        private async void btnAnalyzeFK_Click(object sender, EventArgs e)
        {
            // Cleanup tidligere temp filer
            CleanupTempFiles();

            // Validering
            if (string.IsNullOrEmpty(txtParentCSV.Text) || string.IsNullOrEmpty(txtChildCSV.Text))
            {
                MessageBox.Show("Vælg både parent og child CSV filer");
                return;
            }

            if (cmbParentColumn.SelectedItem == null || cmbChildColumn.SelectedItem == null)
            {
                MessageBox.Show("Vælg kolonner i begge filer");
                return;
            }

            // Tjek at alle ekstra kolonner er valgt
            foreach (var pair in columnPairs)
            {
                if (pair.ParentComboBox.SelectedItem == null || pair.ChildComboBox.SelectedItem == null)
                {
                    MessageBox.Show($"Vælg kolonner for primærnøgle {pair.PairNumber}");
                    return;
                }
            }

            try
            {
                progressBar1.Visible = true;
                progressBar1.Value = 0;

                // Byg column index arrays
                var parentColumnIndexes = new List<int> { GetSelectedColumnIndex(cmbParentColumn) };
                var childColumnIndexes = new List<int> { GetSelectedColumnIndex(cmbChildColumn) };

                foreach (var pair in columnPairs)
                {
                    parentColumnIndexes.Add(GetSelectedColumnIndex(pair.ParentComboBox));
                    childColumnIndexes.Add(GetSelectedColumnIndex(pair.ChildComboBox));
                }

                progressBar1.Value = 25;

                // Find missing values med sammensatte PK support
                var missingCount = await FindMissingFKValuesAsync(
                    txtParentCSV.Text, parentColumnIndexes.ToArray(),
                    txtChildCSV.Text, childColumnIndexes.ToArray()
                );

                progressBar1.Value = 100;

                // Vis resultater
                DisplayFKResults(missingCount, parentColumnIndexes.Count > 1);

                progressBar1.Visible = false;
            }
            catch (Exception ex)
            {
                progressBar1.Visible = false;
                MessageBox.Show($"Fejl ved FK analyse: {ex.Message}");
            }
        }

        /// <summary>
        /// Find missing FK values med support for sammensatte PKs - MEMORY OPTIMIZED
        /// </summary>
        private async Task<int> FindMissingFKValuesAsync(string parentPath, int[] parentColumns,
                                                         string childPath, int[] childColumns)
        {
            // Cleanup forrige temp fil
            CleanupTempFiles();

            // Opret ny temp fil
            currentMissingValuesFile = Path.GetTempFileName();

            var parentKeys = new HashSet<string>();
            var processedChildKeys = new HashSet<string>();
            int missingCount = 0;

            // Læs parent keys
            using (var reader = new StreamReader(parentPath, Encoding.UTF8, true))
            {
                string line = await reader.ReadLineAsync(); // Skip header

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var fields = ParseCSVLine(line);
                    var key = BuildCompositeKey(fields, parentColumns);

                    if (!string.IsNullOrEmpty(key))
                        parentKeys.Add(key);
                }
            }

            // Læs child keys og skriv missing direkte til temp fil
            using (var writer = new StreamWriter(currentMissingValuesFile, false, Encoding.UTF8))
            {
                using (var reader = new StreamReader(childPath, Encoding.UTF8, true))
                {
                    string line = await reader.ReadLineAsync(); // Skip header

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var fields = ParseCSVLine(line);
                        var key = BuildCompositeKey(fields, childColumns);

                        if (!string.IsNullOrEmpty(key) &&
                            !parentKeys.Contains(key) &&
                            !processedChildKeys.Contains(key))
                        {
                            await writer.WriteLineAsync(key);
                            processedChildKeys.Add(key);
                            missingCount++;
                        }
                    }
                }
            }

            return missingCount;
        }

        /// <summary>
        /// Byg sammensatte PK fra felter og column indexes
        /// </summary>
        private string BuildCompositeKey(string[] fields, int[] columnIndexes)
        {
            var keyParts = new List<string>();

            foreach (int index in columnIndexes)
            {
                if (index >= 0 && index < fields.Length)
                {
                    keyParts.Add(fields[index].Trim());
                }
            }

            return keyParts.Count == columnIndexes.Length ? string.Join("|", keyParts) : null;
        }

        /// <summary>
        /// Robust CSV line parser med quote og escape support
        /// </summary>
        private string[] ParseCSVLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ';' && !inQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            fields.Add(currentField.ToString());
            return fields.ToArray();
        }

        /// <summary>
        /// Vis FK analysis resultater i UI - MEMORY OPTIMIZED
        /// </summary>
        private async void DisplayFKResults(int totalMissing, bool isCompositeKey)
        {
            lstMissingValues.Items.Clear();
            string keyType = isCompositeKey ? "sammensatte nøgler" : "nøgler";

            if (totalMissing == 0)
            {
                lblFKStats.Text = $"RESULTAT: Ingen manglende {keyType} fundet";
                btnGenerateDummies.Enabled = false;
                btnExportMissing.Enabled = false;
                btnCopySelected.Enabled = false;
                return;
            }

            // Læs første 500 linjer fra temp fil til UI display
            if (!string.IsNullOrEmpty(currentMissingValuesFile) && File.Exists(currentMissingValuesFile))
            {
                using (var reader = new StreamReader(currentMissingValuesFile, Encoding.UTF8))
                {
                    string line;
                    int displayCount = 0;

                    while ((line = await reader.ReadLineAsync()) != null && displayCount < 500)
                    {
                        lstMissingValues.Items.Add(line);
                        displayCount++;
                    }
                }
            }

            // Opdater statistik
            lblFKStats.Text = $"Fundet {totalMissing} manglende {keyType}";
            if (totalMissing > 500)
                lblFKStats.Text += $"\n(viser første 500 af {totalMissing})";

            btnGenerateDummies.Enabled = true;
            btnExportMissing.Enabled = true;
            btnCopySelected.Enabled = true;
        }

        #endregion

        #region CSV FK Repair - Dummy Generation

        /// <summary>
        /// Generer dummy records for missing FK values
        /// </summary>
        private async void btnGenerateDummies_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentMissingValuesFile) || !File.Exists(currentMissingValuesFile))
            {
                MessageBox.Show("Ingen manglende værdier at generere rææker for");
                return;
            }

            // Count linjer i temp fil
            int missingCount = File.ReadAllLines(currentMissingValuesFile).Length;
            string parentTableName = Path.GetFileNameWithoutExtension(txtParentCSV.Text);
            string parentFileName = Path.GetFileNameWithoutExtension(txtParentCSV.Text);

            saveFileDialog1.Filter = "CSV filer|*.csv";
            saveFileDialog1.FileName = $"{parentFileName}_updated.csv";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    progressBar1.Visible = true;
                    progressBar1.Value = 0;

                    await GenerateDummyRecordsAsync(saveFileDialog1.FileName);

                    progressBar1.Visible = false;
                    MessageBox.Show($"Opdateret {parentTableName} med {missingCount} nye rækker:\n\n{saveFileDialog1.FileName}",
                        "Fuldført", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    progressBar1.Visible = false;
                    MessageBox.Show($"Fejl ved generering af rækker: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Generer dummy CSV records med auto-detection af dato/tid kolonner
        /// </summary>
        private async Task GenerateDummyRecordsAsync(string outputPath)
        {
            string dummyDescription = txtDummyText.Text.Trim();
            if (string.IsNullOrEmpty(dummyDescription))
                dummyDescription = "Generated row record";

            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                // Læs original header fra parent file
                string originalHeader;
                using (var reader = new StreamReader(txtParentCSV.Text, Encoding.UTF8, true))
                {
                    originalHeader = await reader.ReadLineAsync();
                }

                // Auto-detect kolonnetyper baseret på eksisterende data
                var columnTypes = await DetectColumnTypesAsync(txtParentCSV.Text);

                // Tilføj ekstra kolonne til header
                string Header = originalHeader + ";row_description";
                await writer.WriteLineAsync(Header);

                // Kopier eksisterende data UDEN dummy beskrivelse
                using (var reader = new StreamReader(txtParentCSV.Text, Encoding.UTF8, true))
                {
                    string line = await reader.ReadLineAsync(); // Skip header - allerede skrevet

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string Line = line + ";"; // Tom dummy kolonne for eksisterende data
                            await writer.WriteLineAsync(Line);
                        }
                    }
                }

                // Parse original header for at kende struktur
                var originalFields = ParseCSVLine(originalHeader);
                int originalColumnCount = originalFields.Length;

                // Definer key columns
                var keyColumns = new List<int> { GetSelectedColumnIndex(cmbParentColumn) };
                foreach (var pair in columnPairs)
                {
                    keyColumns.Add(GetSelectedColumnIndex(pair.ParentComboBox));
                }

                // Generer dummy records fra temp fil i chunks
                if (!string.IsNullOrEmpty(currentMissingValuesFile) && File.Exists(currentMissingValuesFile))
                {
                    using (var missingReader = new StreamReader(currentMissingValuesFile, Encoding.UTF8))
                    {
                        string missingKey;
                        int processed = 0;
                        int totalMissing = File.ReadAllLines(currentMissingValuesFile).Length;

                        while ((missingKey = await missingReader.ReadLineAsync()) != null)
                        {
                            // Parse sammensatte PK
                            var keyParts = missingKey.Split('|');

                            // Byg dummy record
                            var dummyRecord = new string[originalColumnCount + 1]; // +1 for ekstra kolonne

                            // Udfyld alle felter
                            for (int i = 0; i < originalColumnCount; i++)
                            {
                                // Tjek om dette er en af de PK kolonner
                                int keyPartIndex = keyColumns.IndexOf(i);
                                if (keyPartIndex >= 0 && keyPartIndex < keyParts.Length)
                                {
                                    // Primary key kolonne - brug faktisk værdi
                                    dummyRecord[i] = keyParts[keyPartIndex];
                                }
                                else
                                {
                                    // Ikke-PK kolonne - brug datatype-specific default værdi
                                    string columnType = (i < columnTypes.Length) ? columnTypes[i] : "string";

                                    switch (columnType)
                                    {
                                        case "date":
                                            dummyRecord[i] = "9999-12-31";
                                            break;
                                        case "timestamp":
                                            dummyRecord[i] = "9999-12-31T23:59:59";
                                            break;
                                        case "time":
                                            dummyRecord[i] = "23:59:59";
                                            break;
                                        default:
                                            dummyRecord[i] = ""; // Tom streng for string og andre typer
                                            break;
                                    }
                                }
                            }

                            // Ekstra kolonne med beskrivelse - KUN for dummy records
                            dummyRecord[originalColumnCount] = dummyDescription;

                            // Skriv record
                            await writer.WriteLineAsync(string.Join(";", dummyRecord));

                            processed++;
                            if (processed % 100 == 0)
                            {
                                progressBar1.Value = (int)((processed * 100.0) / totalMissing);
                                await Task.Delay(1); // UI responsiveness
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region CSV FK Repair - Column Type Detection

        /// <summary>
        /// Analyser parent CSV og detekter datatyper for hver kolonne
        /// </summary>
        private async Task<string[]> DetectColumnTypesAsync(string parentPath)
        {
            var columnSamples = new List<List<string>>();
            int columnCount = 0;

            using (var reader = new StreamReader(parentPath, Encoding.UTF8, true))
            {
                // Skip header
                string headerLine = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(headerLine)) return new string[0];

                var headers = ParseCSVLine(headerLine);
                columnCount = headers.Length;

                // Initialiser sample lister
                for (int i = 0; i < columnCount; i++)
                {
                    columnSamples.Add(new List<string>());
                }

                // Saml samples fra første 100 rækker
                string line;
                int sampleCount = 0;
                while ((line = await reader.ReadLineAsync()) != null && sampleCount < 100)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var fields = ParseCSVLine(line);
                    for (int i = 0; i < Math.Min(fields.Length, columnCount); i++)
                    {
                        if (!string.IsNullOrWhiteSpace(fields[i]))
                        {
                            columnSamples[i].Add(fields[i].Trim());
                        }
                    }
                    sampleCount++;
                }
            }

            // Analyser hver kolonne
            var columnTypes = new string[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                columnTypes[i] = DetermineColumnType(columnSamples[i]);
            }

            return columnTypes;
        }

        /// <summary>
        /// Bestem kolonnetype baseret på samples
        /// </summary>
        private string DetermineColumnType(List<string> samples)
        {
            if (samples.Count == 0) return "string";

            int dateCount = 0;
            int timestampCount = 0;
            int timeCount = 0;

            foreach (string sample in samples)
            {
                if (IsDateValue(sample)) dateCount++;
                else if (IsTimestampValue(sample)) timestampCount++;
                else if (IsTimeValue(sample)) timeCount++;
            }

            double totalSamples = samples.Count;

            // Hvis 80%+ matcher et dato/tid format, klassificer som sådan
            if (timestampCount / totalSamples >= 0.8) return "timestamp";
            if (dateCount / totalSamples >= 0.8) return "date";
            if (timeCount / totalSamples >= 0.8) return "time";

            return "string";
        }

        /// <summary>
        /// Tjek om værdi matcher dato format (YYYY-MM-DD)
        /// </summary>
        private bool IsDateValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            // Check for YYYY-MM-DD format
            if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{4}-\d{2}-\d{2}$"))
            {
                return DateTime.TryParseExact(value, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _);
            }

            return false;
        }

        /// <summary>
        /// Tjek om værdi matcher timestamp format (YYYY-MM-DD HH:MM:SS eller YYYY-MM-DDTHH:MM:SS)
        /// </summary>
        private bool IsTimestampValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            // Check for various timestamp formats
            var timestampPatterns = new[]
            {
                @"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$",           // YYYY-MM-DD HH:MM:SS
                @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$",           // YYYY-MM-DDTHH:MM:SS
                @"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d+$",      // With milliseconds
                @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d+$"       // ISO with milliseconds
            };

            foreach (var pattern in timestampPatterns)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(value, pattern))
                {
                    return DateTime.TryParse(value, out _);
                }
            }

            return false;
        }

        /// <summary>
        /// Tjek om værdi matcher tid format (HH:MM:SS)
        /// </summary>
        private bool IsTimeValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            // Check for HH:MM:SS format
            if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{2}:\d{2}:\d{2}$"))
            {
                return TimeSpan.TryParseExact(value, @"hh\:mm\:ss", null, out _);
            }

            return false;
        }

        #endregion

        #region CSV FK Repair - Export/Copy Functionality

        /// <summary>
        /// Setup context menu til ListBox
        /// </summary>
        private void SetupListBoxContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            var copyItem = new ToolStripMenuItem("Kopiér markerede");
            copyItem.Click += (s, e) => btnCopySelected_Click(s, new EventArgs());
            contextMenu.Items.Add(copyItem);

            var selectAllItem = new ToolStripMenuItem("Markér alle");
            selectAllItem.Click += (s, e) =>
            {
                for (int i = 0; i < lstMissingValues.Items.Count; i++)
                    lstMissingValues.SetSelected(i, true);
            };
            contextMenu.Items.Add(selectAllItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var exportItem = new ToolStripMenuItem("Eksportér alle til fil");
            exportItem.Click += (s, e) => btnExportMissing_Click(s, new EventArgs());
            contextMenu.Items.Add(exportItem);

            lstMissingValues.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// Kopiér markerede items til clipboard
        /// </summary>
        private void btnCopySelected_Click(object sender, EventArgs e)
        {
            if (lstMissingValues.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vælg items at kopiere");
                return;
            }

            var selectedText = string.Join("\r\n", lstMissingValues.SelectedItems.Cast<string>());
            Clipboard.SetText(selectedText);

            MessageBox.Show($"Kopieret {lstMissingValues.SelectedItems.Count} items til clipboard");
        }

        /// <summary>
        /// Eksporter alle missing values til tekstfil - MEMORY OPTIMIZED
        /// </summary>
        private async void btnExportMissing_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentMissingValuesFile) || !File.Exists(currentMissingValuesFile))
            {
                MessageBox.Show("Ingen data at eksportere");
                return;
            }

            saveFileDialog1.Filter = "Tekstfiler|*.txt|Alle filer|*.*";
            saveFileDialog1.FileName = $"missing_fk_values_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sb = new StringBuilder();

                    // Header information
                    sb.AppendLine("Missing Foreign Key Values Report");
                    sb.AppendLine("=");
                    sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sb.AppendLine($"Parent CSV: {txtParentCSV.Text}");
                    sb.AppendLine($"Child CSV: {txtChildCSV.Text}");

                    // Column mapping info
                    sb.AppendLine($"Parent Column: {cmbParentColumn.Text}");
                    sb.AppendLine($"Child Column: {cmbChildColumn.Text}");

                    if (columnPairs.Count > 0)
                    {
                        sb.AppendLine("Additional sammensatte PK Columns:");
                        foreach (var pair in columnPairs)
                        {
                            sb.AppendLine($"  Parent {pair.PairNumber}: {pair.ParentComboBox.Text}");
                            sb.AppendLine($"  Child {pair.PairNumber}: {pair.ChildComboBox.Text}");
                        }
                    }

                    // Count total missing values
                    int totalMissing = File.ReadAllLines(currentMissingValuesFile).Length;
                    sb.AppendLine($"Total Missing Values: {totalMissing}");
                    sb.AppendLine();
                    sb.AppendLine("Missing Values:");
                    sb.AppendLine("---------------");

                    // Write header til fil
                    File.WriteAllText(saveFileDialog1.FileName, sb.ToString(), Encoding.UTF8);

                    // Append missing values direkte fra temp fil
                    using (var reader = new StreamReader(currentMissingValuesFile, Encoding.UTF8))
                    using (var writer = new StreamWriter(saveFileDialog1.FileName, append: true, Encoding.UTF8))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            await writer.WriteLineAsync(line);
                        }
                    }

                    MessageBox.Show($"Eksporteret {totalMissing} værdier til {saveFileDialog1.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fejl ved eksport: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Keyboard shortcuts til ListBox
        /// </summary>
        private void lstMissingValues_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                btnCopySelected_Click(sender, new EventArgs());
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                for (int i = 0; i < lstMissingValues.Items.Count; i++)
                    lstMissingValues.SetSelected(i, true);
                e.Handled = true;
            }
        }

        #endregion

        private void labelDummyText_Click(object sender, EventArgs e)
        {

        }
    }
}