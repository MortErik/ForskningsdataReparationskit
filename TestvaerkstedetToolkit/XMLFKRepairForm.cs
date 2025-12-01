using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using TestvaerkstedetToolkit.Models;
using TestvaerkstedetToolkit.Utilities;

namespace TestvaerkstedetToolkit
{
    public partial class XMLFKRepairForm : Form
    {
        #region Private Classes

        /// <summary>
        /// XML FK Repair state holder
        /// </summary>
        private class XmlFKRepair
        {
            public string ParentXmlPath { get; set; }
            public string ChildXmlPath { get; set; }
            public string TableIndexPath { get; set; }  // OPTIONAL for metadata
            public List<string> ParentKeyColumns { get; set; } = new List<string>();
            public List<string> ChildKeyColumns { get; set; } = new List<string>();
            public string IntegrityErrorDescription { get; set; } = "Betydning ukendt. Rækken er tilføjet under aflevering til arkiv, for at sikre referentiel integritet i databasen af hensyn til langtidsbevaring";

            // Optional metadata from tableIndex
            public TableIndexEntry ParentTableEntry { get; set; }
            public TableIndexEntry ChildTableEntry { get; set; }
            public string AvidRootPath { get; set; }
        }

        /// <summary>
        /// XML kolonne information parsed direkte fra XML
        /// </summary>
        private class XmlColumnInfo
        {
            public string Name { get; set; }           // c1, c2, etc.
            public int Position { get; set; }
            public bool IsNillable { get; set; }

            // Optional fra tableIndex
            public string DisplayName { get; set; }    // barn_id, koen, etc.
            public string DataType { get; set; }       // VARCHAR(5), DECIMAL, etc.
            public string Description { get; set; }
        }

        /// <summary>
        /// Wrapper til TableIndexEntry for ComboBox display
        /// </summary>
        private class TableComboBoxItem
        {
            public string DisplayText { get; set; }     // "Forældre (table6)"
            public TableIndexEntry TableEntry { get; set; }

            public override string ToString() => DisplayText;
        }

        /// <summary>
        /// Repræsenterer et kolonne par til sammensatte PK support
        /// </summary>
        private class ColumnPair
        {
            public System.Windows.Forms.ComboBox ParentComboBox { get; set; }
            public System.Windows.Forms.ComboBox ChildComboBox { get; set; }
            public System.Windows.Forms.Label ParentLabel { get; set; }
            public System.Windows.Forms.Label ChildLabel { get; set; }
            public int PairNumber { get; set; }
        }

        #endregion

        #region Fields

        private XmlFKRepair currentXmlRepair = new XmlFKRepair();
        private List<ColumnPair> xmlColumnPairs = new List<ColumnPair>();
        private int nextXmlPairNumber = 2;
        private List<XmlColumnInfo> currentParentXmlColumns = null;
        private List<XmlColumnInfo> currentChildXmlColumns = null;

        // Custom output path (null = brug Desktop default)
        private string customOutputPath = null;
        private const string OUTPUT_FOLDER_NAME = "XML_FK_Repairs";

        #endregion

        #region Constructor

        public XMLFKRepairForm()
        {
            InitializeComponent();
            SetupXmlListBoxContextMenu();

            // Tilføj Load event handler
            this.Load += XMLFKRepairForm_Load;
        }

        /// <summary>
        /// Form Load event - kaldes EFTER WelcomeForm har tilføjet backPanel
        /// </summary>
        private void XMLFKRepairForm_Load(object sender, EventArgs e)
        {
            AddOutputControlsToBackPanel();
            UpdateOutputPathLabel();
        }

        #endregion

        #region Browse Events

        /// <summary>
        /// Browse for parent XML file
        /// </summary>
        private void BtnBrowseParentXml_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML filer|*.xml|Alle filer|*.*";
            openFileDialog1.Title = "Vælg Parent XML fil";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentXmlRepair.ParentXmlPath = openFileDialog1.FileName;
                txtParentXml.Text = openFileDialog1.FileName;

                // Hvis tableIndex er loaded, find metadata
                if (!string.IsNullOrEmpty(currentXmlRepair.TableIndexPath))
                {
                    try
                    {
                        var tables = TableIndexParser.ParseTableIndex(currentXmlRepair.TableIndexPath);
                        currentXmlRepair.ParentTableEntry = FindTableEntryByXmlPath(tables, currentXmlRepair.ParentXmlPath);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Could not enrich with tableIndex: {ex.Message}");
                    }
                }

                LoadXmlColumns(currentXmlRepair.ParentXmlPath, cmbParentXmlColumns, true);
            }
        }

        /// <summary>
        /// Browse for child XML file
        /// </summary>
        private void BtnBrowseChildXml_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML filer|*.xml|Alle filer|*.*";
            openFileDialog1.Title = "Vælg Child XML fil";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentXmlRepair.ChildXmlPath = openFileDialog1.FileName;
                txtChildXml.Text = openFileDialog1.FileName;

                // Hvis tableIndex er loaded, find metadata
                if (!string.IsNullOrEmpty(currentXmlRepair.TableIndexPath))
                {
                    try
                    {
                        var tables = TableIndexParser.ParseTableIndex(currentXmlRepair.TableIndexPath);
                        currentXmlRepair.ChildTableEntry = FindTableEntryByXmlPath(tables, currentXmlRepair.ChildXmlPath);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Could not enrich with tableIndex: {ex.Message}");
                    }
                }

                LoadXmlColumns(currentXmlRepair.ChildXmlPath, cmbChildXmlColumns, false);
            }
        }

        /// <summary>
        /// Browse for tableIndex.xml
        /// </summary>
        private void BtnBrowseTableIndex_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "XML filer|*.xml|Alle filer|*.*";
            openFileDialog1.Title = "Vælg tableIndex.xml fil";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string tableIndexPath = openFileDialog1.FileName;

                    // 1. DETECT AVID ROOT
                    string tableIndexDir = Path.GetDirectoryName(tableIndexPath);  // C:\...\Indices
                    string avidRoot = Path.GetDirectoryName(tableIndexDir);        // C:\AVID.SA.12345.1

                    System.Diagnostics.Debug.WriteLine($"[TableIndex] Path: {tableIndexPath}");
                    System.Diagnostics.Debug.WriteLine($"[TableIndex] AVID Root: {avidRoot}");

                    // 2. PARSE TABLEINDEX
                    var tables = TableIndexParser.ParseTableIndex(tableIndexPath);
                    System.Diagnostics.Debug.WriteLine($"[TableIndex] Parsed {tables.Count} tables");

                    // 3. SAVE TO STATE
                    currentXmlRepair.TableIndexPath = tableIndexPath;
                    currentXmlRepair.AvidRootPath = avidRoot;
                    txtTableIndex.Text = tableIndexPath;

                    // 4. POPULATE DROPDOWNS
                    cmbParentTable.Items.Clear();
                    cmbChildTable.Items.Clear();
                    PopulateTableDropdowns(tables);

                    // 5. ENABLE DROPDOWNS
                    cmbParentTable.Enabled = true;
                    cmbChildTable.Enabled = true;

                    System.Diagnostics.Debug.WriteLine($"[TableIndex] Dropdowns populated and enabled");

                    // 6. OPTIONAL: Hvis Parent/Child XML allerede valgt via browse, enrich metadata
                    if (!string.IsNullOrEmpty(currentXmlRepair.ParentXmlPath))
                    {
                        currentXmlRepair.ParentTableEntry = FindTableEntryByXmlPath(tables, currentXmlRepair.ParentXmlPath);
                        LoadXmlColumns(currentXmlRepair.ParentXmlPath, cmbParentXmlColumns, true);
                        System.Diagnostics.Debug.WriteLine("[TableIndex] Re-enriched parent columns");
                    }

                    if (!string.IsNullOrEmpty(currentXmlRepair.ChildXmlPath))
                    {
                        currentXmlRepair.ChildTableEntry = FindTableEntryByXmlPath(tables, currentXmlRepair.ChildXmlPath);
                        LoadXmlColumns(currentXmlRepair.ChildXmlPath, cmbChildXmlColumns, false);
                        System.Diagnostics.Debug.WriteLine("[TableIndex] Re-enriched child columns");
                    }

                    MessageBox.Show($"TableIndex indlæst: {tables.Count} tabeller fundet\n\nVælg Parent og Child tabel fra dropdowns",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fejl ved indlæsning af tableIndex: {ex.Message}", "Fejl",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    currentXmlRepair.TableIndexPath = null;
                    currentXmlRepair.AvidRootPath = null;
                    txtTableIndex.Text = "";

                    cmbParentTable.Enabled = false;
                    cmbChildTable.Enabled = false;
                }
            }
        }

        private void PopulateTableDropdowns(List<TableIndexEntry> tables)
        {
            foreach (var table in tables)
            {
                // Format: "Forældre (table6)"
                string displayText = $"{table.Name} ({table.Folder})";

                var item = new TableComboBoxItem
                {
                    DisplayText = displayText,
                    TableEntry = table
                };

                cmbParentTable.Items.Add(item);
                cmbChildTable.Items.Add(item);
            }
        }

        private void CmbParentTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbParentTable.SelectedItem == null)
                return;

            var selectedItem = (TableComboBoxItem)cmbParentTable.SelectedItem;
            var tableEntry = selectedItem.TableEntry;

            System.Diagnostics.Debug.WriteLine($"[Parent Dropdown] Selected: {tableEntry.Name} ({tableEntry.Folder})");

            // Auto-construct path
            string xmlPath = Path.Combine(
                currentXmlRepair.AvidRootPath,
                "Tables",
                tableEntry.Folder,
                $"{tableEntry.Folder}.xml"
            );

            txtParentXml.Text = xmlPath;
            System.Diagnostics.Debug.WriteLine($"[Parent Dropdown] Auto-path: {xmlPath}");

            if (File.Exists(xmlPath))
            {
                // SUCCESS - load columns
                txtParentXml.ForeColor = Color.Blue;
                currentXmlRepair.ParentXmlPath = xmlPath;
                currentXmlRepair.ParentTableEntry = tableEntry;

                LoadXmlColumns(xmlPath, cmbParentXmlColumns, true);
                System.Diagnostics.Debug.WriteLine($"[Parent Dropdown] ✓ Loaded {cmbParentXmlColumns.Items.Count} columns");

                btnBrowseParentXml.Enabled = true;  // Enable override
            }
            else
            {
                // ERROR - show warning
                txtParentXml.ForeColor = Color.Red;
                System.Diagnostics.Debug.WriteLine("[Parent Dropdown] File not found!");

                MessageBox.Show($"Fil ikke fundet:\n{xmlPath}\n\nBrug 'Browse...' knappen for at vælge manuelt.",
                    "Fil ikke fundet", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                btnBrowseParentXml.Enabled = true;  // Enable override
            }
        }

        private void CmbChildTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChildTable.SelectedItem == null)
                return;

            var selectedItem = (TableComboBoxItem)cmbChildTable.SelectedItem;
            var tableEntry = selectedItem.TableEntry;

            System.Diagnostics.Debug.WriteLine($"[Child Dropdown] Selected: {tableEntry.Name} ({tableEntry.Folder})");

            // Auto-construct path
            string xmlPath = Path.Combine(
                currentXmlRepair.AvidRootPath,
                "Tables",
                tableEntry.Folder,
                $"{tableEntry.Folder}.xml"
            );

            txtChildXml.Text = xmlPath;
            System.Diagnostics.Debug.WriteLine($"[Child Dropdown] Auto-path: {xmlPath}");

            if (File.Exists(xmlPath))
            {
                // SUCCESS - load columns
                txtChildXml.ForeColor = Color.Blue;
                currentXmlRepair.ChildXmlPath = xmlPath;
                currentXmlRepair.ChildTableEntry = tableEntry;

                LoadXmlColumns(xmlPath, cmbChildXmlColumns, false);
                System.Diagnostics.Debug.WriteLine($"[Child Dropdown] ✓ Loaded {cmbChildXmlColumns.Items.Count} columns");

                btnBrowseChildXml.Enabled = true;  // Enable override
            }
            else
            {
                // ERROR - show warning
                txtChildXml.ForeColor = Color.Red;
                System.Diagnostics.Debug.WriteLine("[Child Dropdown] ✗ File not found!");

                MessageBox.Show($"Fil ikke fundet:\n{xmlPath}\n\nBrug 'Browse...' knappen for at vælge manuelt.",
                    "Fil ikke fundet", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                btnBrowseChildXml.Enabled = true;  // Enable override
            }
        }

        #endregion

        #region Column Parsing

        /// <summary>
        /// Load XML kolonner direkte fra XML fil
        /// </summary>
        private void LoadXmlColumns(string xmlPath, ComboBox comboBox, bool isParent)
        {
            try
            {
                var columns = ParseColumnsFromXml(xmlPath);

                // Enrich med tableIndex metadata hvis tilgængelig
                if (!string.IsNullOrEmpty(currentXmlRepair.TableIndexPath))
                {
                    var tableEntry = isParent ? currentXmlRepair.ParentTableEntry : currentXmlRepair.ChildTableEntry;
                    if (tableEntry != null)
                    {
                        columns = EnrichColumnsWithTableIndexMetadata(columns, tableEntry);
                    }
                }

                comboBox.Items.Clear();

                foreach (var column in columns.OrderBy(c => c.Position))
                {
                    // Display format: c1: barn_id (VARCHAR) - Description hvis tilgængelig
                    string displayText = column.Name;

                    if (!string.IsNullOrEmpty(column.DisplayName))
                    {
                        displayText = $"{column.Name}: {column.DisplayName}";
                    }

                    if (!string.IsNullOrEmpty(column.DataType))
                    {
                        displayText += $" ({column.DataType})";
                    }

                    if (!string.IsNullOrEmpty(column.Description))
                    {
                        displayText += $" - {column.Description}";
                    }

                    comboBox.Items.Add(displayText);
                }

                if (comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;

                // GEM kolonner til auto-population
                if (isParent)
                    currentParentXmlColumns = columns;
                else
                    currentChildXmlColumns = columns;

                // AUTO-POPULER eksisterende pairs
                var currentTableEntry = isParent ? currentXmlRepair.ParentTableEntry : currentXmlRepair.ChildTableEntry;
                foreach (var pair in xmlColumnPairs)
                {
                    if (isParent)
                        PopulateXmlComboBoxWithColumns(pair.ParentComboBox, columns, currentTableEntry);
                    else
                        PopulateXmlComboBoxWithColumns(pair.ChildComboBox, columns, currentTableEntry);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved læsning af XML kolonner: {ex.Message}", "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Parse kolonner direkte fra XML første row
        /// </summary>
        private List<XmlColumnInfo> ParseColumnsFromXml(string xmlPath)
        {
            var columns = new List<XmlColumnInfo>();

            var settings = new XmlReaderSettings
            {
                IgnoreWhitespace = true,
                IgnoreComments = true,
                DtdProcessing = DtdProcessing.Ignore
            };

            using (var reader = XmlReader.Create(xmlPath, settings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
                    {
                        // Læs første row for kolonne struktur
                        var rowSubtree = reader.ReadSubtree();
                        int position = 1;

                        while (rowSubtree.Read())
                        {
                            if (rowSubtree.NodeType == XmlNodeType.Element && rowSubtree.Name.StartsWith("c"))
                            {
                                bool isNillable = rowSubtree.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true";

                                columns.Add(new XmlColumnInfo
                                {
                                    Name = rowSubtree.Name,
                                    Position = position++,
                                    IsNillable = isNillable
                                });
                            }
                        }
                        break; // Kun læs første row
                    }
                }
            }

            return columns;
        }

        /// <summary>
        /// Enrich parsed columns med tableIndex metadata
        /// </summary>
        private List<XmlColumnInfo> EnrichColumnsWithTableIndexMetadata(List<XmlColumnInfo> columns, TableIndexEntry tableEntry)
        {
            if (tableEntry == null || tableEntry.Columns == null)
                return columns;

            // Map columnID (c1, c2, etc.) til tableIndex metadata
            foreach (var col in columns)
            {
                // Find matching column i tableIndex
                var tableColumn = tableEntry.Columns.FirstOrDefault(tc => tc.ColumnID == col.Name);

                if (tableColumn != null)
                {
                    col.DisplayName = tableColumn.Name;        // barn_id, koen, etc.
                    col.DataType = tableColumn.DataType;           // VARCHAR(5), DECIMAL, etc.
                    col.Description = tableColumn.Description;
                }
            }

            return columns;
        }

        /// <summary>
        /// Find TableIndexEntry ved at matche XML filpath
        /// </summary>
        private TableIndexEntry FindTableEntryByXmlPath(List<TableIndexEntry> tables, string xmlPath)
        {
            string xmlFileName = Path.GetFileNameWithoutExtension(xmlPath); // "table1"

            // 1: Match på folder navn
            var match = tables.FirstOrDefault(t => t.Folder == xmlFileName);
            if (match != null) return match;

            // 2: Match på tabel navn
            match = tables.FirstOrDefault(t => string.Equals(t.Name, xmlFileName, StringComparison.OrdinalIgnoreCase));
            if (match != null) return match;

            // 3: Find XML i alle tabeller
            string tableIndexDirectory = Path.GetDirectoryName(currentXmlRepair.TableIndexPath);
            foreach (var table in tables)
            {
                string foundXmlPath = table.FindXmlPath(tableIndexDirectory);
                if (foundXmlPath != null && Path.GetFullPath(foundXmlPath) == Path.GetFullPath(xmlPath))
                {
                    return table;
                }
            }

            return null;
        }

        #endregion

        #region Analysis and Generation

        /// <summary>
        /// Analyzer XML FK relationships
        /// </summary>
        private async void BtnAnalyzeXmlFK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentXmlRepair.ParentXmlPath) || string.IsNullOrEmpty(currentXmlRepair.ChildXmlPath))
            {
                MessageBox.Show("Vælg både Parent og Child XML filer først");
                return;
            }

            // Tjek at alle ekstra kolonner er valgt
            foreach (var pair in xmlColumnPairs)
            {
                if (pair.ParentComboBox.SelectedItem == null || pair.ChildComboBox.SelectedItem == null)
                {
                    MessageBox.Show($"Vælg kolonner for primærnøgle {pair.PairNumber}");
                    return;
                }
            }

            // saml alle primærnøgle kolonner (base + sammensatte)
            var parentKeys = new List<string> { ExtractColumnName(cmbParentXmlColumns.Text) };
            var childKeys = new List<string> { ExtractColumnName(cmbChildXmlColumns.Text) };

            foreach (var pair in xmlColumnPairs)
            {
                parentKeys.Add(ExtractColumnName(pair.ParentComboBox.Text));
                childKeys.Add(ExtractColumnName(pair.ChildComboBox.Text));
            }

            currentXmlRepair.ParentKeyColumns = parentKeys;
            currentXmlRepair.ChildKeyColumns = childKeys;

            try
            {
                btnAnalyzeXmlFK.Enabled = false;
                lblXmlFKStats.Text = "Analyserer...";
                progressBar1.Visible = true;
                progressBar1.Style = ProgressBarStyle.Marquee;

                var progress = new Progress<(long processed, string stage)>(update =>
                {
                    lblXmlFKStats.Text = $"{update.stage}: {update.processed:N0} rækker";
                });

                var repairTool = new ScalableXmlFKRepair();
                var missingKeys = await repairTool.AnalyzeMissingKeysAsync(
                    currentXmlRepair.ParentXmlPath,
                    currentXmlRepair.ChildXmlPath,
                    parentKeys,
                    childKeys,
                    progress,
                    CancellationToken.None
                );

                lstXmlMissingValues.Items.Clear();
                foreach (var key in missingKeys.OrderBy(k => k))
                {
                    lstXmlMissingValues.Items.Add(key);
                }

                progressBar1.Visible = false;
                lblXmlFKStats.Text = $"Analyse komplet: {missingKeys.Count:N0} manglende værdier fundet";

                btnGenerateFixedXml.Enabled = missingKeys.Count > 0;
                btnExportXmlMissing.Enabled = missingKeys.Count > 0;
            }
            catch (Exception ex)
            {
                progressBar1.Visible = false;
                lblXmlFKStats.Text = "Analyse fejlet";
                MessageBox.Show($"Fejl under analyse: {ex.Message}", "Fejl", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnAnalyzeXmlFK.Enabled = true;
            }
        }

        /// <summary>
        /// Generate repareret XML med missing FK rows
        /// </summary>
        private async void BtnGenerateFixedXml_Click(object sender, EventArgs e)
        {
            if (lstXmlMissingValues.Items.Count == 0)
            {
                MessageBox.Show("Ingen manglende værdier at reparere");
                return;
            }

            string outputXmlPath = null;
            string outputDirectory = null;
            bool useStructuredOutput = false;

            // Brug structured output hvis tableIndex er tilgængelig
            if (!string.IsNullOrEmpty(currentXmlRepair.TableIndexPath) &&
                currentXmlRepair.ParentTableEntry != null)
            {
                // OUTPUT PATH
                useStructuredOutput = true;
                string avidVersion = ExtractAvidVersion(currentXmlRepair.TableIndexPath);
                string tableName = currentXmlRepair.ParentTableEntry.Name;

                try
                {
                    outputDirectory = CreateRepairOutputDirectory(avidVersion, tableName);
                    string tableFolder = currentXmlRepair.ParentTableEntry.Folder; // "table1", "table2", etc.
                    outputXmlPath = Path.Combine(outputDirectory, $"{tableFolder}_repaired.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kunne ikke oprette output mappe: {ex.Message}\nBruger fallback SaveFileDialog.",
                                  "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    useStructuredOutput = false;
                }
            }

            // Brug SaveFileDialog hvis structured output ikke er muligt
            if (!useStructuredOutput)
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "XML filer|*.xml";
                    saveDialog.Title = "Gem repareret Parent XML";

                    string defaultFileName;
                    if (currentXmlRepair.ParentTableEntry != null &&
                        !string.IsNullOrEmpty(currentXmlRepair.ParentTableEntry.Folder))
                    {
                        defaultFileName = $"{currentXmlRepair.ParentTableEntry.Folder}_repaired.xml";
                    }
                    else
                    {
                        defaultFileName = Path.GetFileNameWithoutExtension(currentXmlRepair.ParentXmlPath) + "_repaired.xml";
                    }

                    saveDialog.FileName = defaultFileName;

                    if (saveDialog.ShowDialog() != DialogResult.OK)
                        return;

                    outputXmlPath = saveDialog.FileName;
                }
            }

            // EXECUTE REPAIR
            try
            {
                btnGenerateFixedXml.Enabled = false;
                progressBar1.Visible = true;
                progressBar1.Style = ProgressBarStyle.Continuous;

                var missingKeys = lstXmlMissingValues.Items.Cast<string>().ToList();
                string integrityDescription = txtIntegrityDesc.Text;

                // Saml alle key kolonner
                var keyColumns = new List<string>();
                keyColumns.Add(ExtractColumnName(cmbParentXmlColumns.Text));

                foreach (var pair in xmlColumnPairs)
                {
                    string parentColName = ExtractColumnName(pair.ParentComboBox.Text);
                    if (!string.IsNullOrEmpty(parentColName) && !keyColumns.Contains(parentColName))
                    {
                        keyColumns.Add(parentColName);
                    }
                }

                var progress = new Progress<(long processed, string stage)>(update =>
                {
                    lblXmlFKStats.Text = $"{update.stage}: {update.processed:N0}";
                    if (update.processed > 0)
                    {
                        progressBar1.Value = Math.Min((int)(update.processed % 100), 100);
                    }
                });

                var repairTool = new ScalableXmlFKRepair();
                var tableEntry = currentXmlRepair.ParentTableEntry;

                // Generer repareret XML
                await repairTool.GenerateRepairedXmlAsync(
                    currentXmlRepair.ParentXmlPath,
                    outputXmlPath,
                    missingKeys,
                    keyColumns,
                    integrityDescription,
                    tableEntry,
                    progress,
                    CancellationToken.None
                );

                // Opdater tableIndex hvis structured output
                bool tableIndexUpdated = false;
                if (useStructuredOutput && !string.IsNullOrEmpty(currentXmlRepair.TableIndexPath))
                {
                    try
                    {
                        lblXmlFKStats.Text = "Opdaterer tableIndex.xml...";

                        string outputTableIndexPath = Path.Combine(outputDirectory, "tableIndex_updated.xml");

                        UpdateTableIndexAfterRepair(
                            currentXmlRepair.TableIndexPath,      // Source (læs fra original)
                            outputTableIndexPath,                  // Output (gem til ny fil)
                            currentXmlRepair.ParentTableEntry.Name,
                            missingKeys.Count,
                            "Integritsfejl"
                        );

                        tableIndexUpdated = true;
                    }
                    catch (Exception tiEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Kunne ikke opdatere tableIndex: {tiEx.Message}");
                    }
                }

                // UI opdatering
                progressBar1.Visible = false;

                if (useStructuredOutput)
                {
                    lblXmlFKStats.Text = tableIndexUpdated
                        ? $"Genereret {missingKeys.Count:N0} nye rækker - tableIndex opdateret"
                        : $"Genereret {missingKeys.Count:N0} nye rækker";
                }
                else
                {
                    lblXmlFKStats.Text = $"Genereret {missingKeys.Count:N0} nye rækker";
                }

                // Success message
                string successMessage = $"Genereret repareret XML med {missingKeys.Count:N0} nye rækker\n\n";

                if (useStructuredOutput)
                {
                    successMessage += $"Output mappe: {outputDirectory}\n\n";
                    successMessage += "Filer oprettet:\n";
                    successMessage += $"  • {Path.GetFileName(outputXmlPath)}\n";

                    if (tableIndexUpdated)
                    {
                        successMessage += "  • tableIndex_updated.xml\n\n";
                        successMessage += "tableIndex ændringer:\n";
                        successMessage += "  • Tilføjet 'Integritsfejl' kolonne\n";
                        successMessage += $"  • Opdateret antal rækker (+{missingKeys.Count:N0})\n\n";
                    }
                    else
                    {
                        successMessage += "\n";
                    }

                    successMessage += "Vil du åbne output mappen?";
                }
                else
                {
                    successMessage += $"Output: {outputXmlPath}";
                }

                var result = MessageBox.Show(
                    successMessage,
                    "Succes",
                    useStructuredOutput ? MessageBoxButtons.YesNo : MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Åbn output mappe hvis bruger siger ja
                if (useStructuredOutput && result == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", outputDirectory);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Kunne ikke åbne mappe: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                progressBar1.Visible = false;
                lblXmlFKStats.Text = "Fejl under generering";
                MessageBox.Show($"Fejl ved generering af repareret XML: {ex.Message}", "Fejl",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnGenerateFixedXml.Enabled = true;
            }
        }

        /// <summary>
        /// Extract clean column name from ComboBox display text
        /// </summary>
        private string ExtractColumnName(string comboBoxText)
        {
            if (string.IsNullOrEmpty(comboBoxText))
                return "";

            // Format: "c1: barn_id (VARCHAR) - Description" -> extract "c1"
            var match = System.Text.RegularExpressions.Regex.Match(comboBoxText, @"^(c\d+)");
            if (match.Success)
                return match.Groups[1].Value;

            // Fallback: split på kolon eller mellemrum
            return comboBoxText.Split(new[] { ':', ' ', '(' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
        }

        /// <summary>
        /// Opdater tableIndex.xml med ny "Integritsfejl" kolonne og opdateret row count
        /// Gemmer til output path (ikke original)
        /// </summary>
        private void UpdateTableIndexAfterRepair(
            string sourceTableIndexPath,
            string outputTableIndexPath,
            string tableName,
            int addedRowCount,
            string integrityColumnName = "Integritsfejl")
        {
            try
            {
                // Load tableIndex.xml
                var doc = XDocument.Load(sourceTableIndexPath);
                var ns = doc.Root.GetDefaultNamespace();

                // Find target table
                var tablesElement = doc.Descendants(ns + "tables").First();
                var targetTable = tablesElement.Elements(ns + "table")
                    .FirstOrDefault(t => t.Element(ns + "name")?.Value == tableName);

                if (targetTable == null)
                {
                    throw new InvalidOperationException($"Tabel '{tableName}' ikke fundet i tableIndex.xml");
                }

                // STEP 1: Tilføj "Integritsfejl" kolonne hvis den ikke findes
                var columnsElement = targetTable.Element(ns + "columns");
                if (columnsElement != null)
                {
                    // Tjek om kolonnen allerede findes
                    bool integrityColumnExists = columnsElement.Elements(ns + "column")
                        .Any(c => c.Element(ns + "name")?.Value == integrityColumnName);

                    if (!integrityColumnExists)
                    {
                        // Find højeste columnID nummer
                        int maxColumnNumber = 0;
                        foreach (var column in columnsElement.Elements(ns + "column"))
                        {
                            string columnID = column.Element(ns + "columnID")?.Value ?? "";
                            if (columnID.StartsWith("c") && int.TryParse(columnID.Substring(1), out int num))
                            {
                                maxColumnNumber = Math.Max(maxColumnNumber, num);
                            }
                        }

                        // Opret ny kolonne
                        string newColumnID = $"c{maxColumnNumber + 1}";
                        var newColumn = new XElement(ns + "column",
                            new XElement(ns + "name", integrityColumnName),
                            new XElement(ns + "columnID", newColumnID),
                            new XElement(ns + "type", "VARCHAR(50)"),
                            new XElement(ns + "typeOriginal", ""),
                            new XElement(ns + "nullable", "true"),
                            new XElement(ns + "description",
                                "Betydning ukendt. Rækken er tilføjet under aflevering til arkiv, " +
                                "for at sikre referentiel integritet i databasen af hensyn til langtidsbevaring")
                        );

                        columnsElement.Add(newColumn);
                    }
                }

                // STEP 2: Opdater row count
                var rowsElement = targetTable.Element(ns + "rows");
                if (rowsElement != null)
                {
                    int currentRows = int.TryParse(rowsElement.Value, out int rows) ? rows : 0;
                    int newRowCount = currentRows + addedRowCount;
                    rowsElement.Value = newRowCount.ToString();
                }

                // Gem opdateret tableIndex.xml til OUTPUT path (IKKE original)
                doc.Save(outputTableIndexPath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fejl ved opdatering af tableIndex.xml: {ex.Message}", ex);
            }
        }

        #endregion

        #region Composite PK Support

        /// <summary>
        /// Populate en ComboBox med XML kolonner
        /// </summary>
        private void PopulateXmlComboBoxWithColumns(ComboBox comboBox, List<XmlColumnInfo> columns, TableIndexEntry tableEntry)
        {
            if (comboBox == null || columns == null) return;

            try
            {
                comboBox.Items.Clear();

                foreach (var column in columns.OrderBy(c => c.Position))
                {
                    string displayText = column.Name;
                    if (!string.IsNullOrEmpty(column.DisplayName))
                        displayText = $"{column.Name}: {column.DisplayName}";
                    if (!string.IsNullOrEmpty(column.DataType))
                        displayText += $" ({column.DataType})";
                    if (!string.IsNullOrEmpty(column.Description))
                        displayText += $" - {column.Description}";

                    comboBox.Items.Add(displayText);
                }

                if (comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PopulateXmlComboBoxWithColumns error: {ex.Message}");
            }
        }

        /// <summary>
        /// Add button click handler
        /// </summary>
        private void BtnAddXmlPrimaryKey_Click(object sender, EventArgs e)
        {
            AddNewXmlColumnPair();
        }

        /// <summary>
        /// Remove button click handler
        /// </summary>
        private void BtnRemoveXmlPrimaryKey_Click(object sender, EventArgs e)
        {
            RemoveLastXmlColumnPair();
        }

        /// <summary>
        /// Tilføj nyt XML column pair til sammensatte PK
        /// </summary>
        private void AddNewXmlColumnPair()
        {
            var pair = new ColumnPair { PairNumber = nextXmlPairNumber };

            // Skab labels
            pair.ParentLabel = new Label
            {
                Text = $"Parent {nextXmlPairNumber}:",
                Location = new Point(20, 15 + (nextXmlPairNumber - 2) * 30),
                Size = new Size(70, 13),
                AutoSize = true
            };

            pair.ChildLabel = new Label
            {
                Text = $"Child {nextXmlPairNumber}:",
                Location = new Point(500, 15 + (nextXmlPairNumber - 2) * 30),
                Size = new Size(70, 13),
                AutoSize = true
            };

            // Skab ComboBoxes
            pair.ParentComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(110, 12 + (nextXmlPairNumber - 2) * 30),
                Size = new Size(350, 21),
                Name = $"cmbXmlParent{nextXmlPairNumber}"
            };

            pair.ChildComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(580, 12 + (nextXmlPairNumber - 2) * 30),
                Size = new Size(350, 21),
                Name = $"cmbXmlChild{nextXmlPairNumber}"
            };

            // Tilføj til panel
            pnlXmlDynamicColumns.Controls.Add(pair.ParentLabel);
            pnlXmlDynamicColumns.Controls.Add(pair.ChildLabel);
            pnlXmlDynamicColumns.Controls.Add(pair.ParentComboBox);
            pnlXmlDynamicColumns.Controls.Add(pair.ChildComboBox);

            // Auto-populer hvis kolonner allerede er indlæst
            if (currentParentXmlColumns != null)
                PopulateXmlComboBoxWithColumns(pair.ParentComboBox, currentParentXmlColumns, currentXmlRepair.ParentTableEntry);
            if (currentChildXmlColumns != null)
                PopulateXmlComboBoxWithColumns(pair.ChildComboBox, currentChildXmlColumns, currentXmlRepair.ChildTableEntry);

            xmlColumnPairs.Add(pair);
            nextXmlPairNumber++;

            UpdateXmlRemoveButtonState();
        }

        /// <summary>
        /// Fjern sidste XML column pair
        /// </summary>
        private void RemoveLastXmlColumnPair()
        {
            if (xmlColumnPairs.Count == 0) return;

            var lastPair = xmlColumnPairs.Last();

            // Fjern controls fra panel
            pnlXmlDynamicColumns.Controls.Remove(lastPair.ParentLabel);
            pnlXmlDynamicColumns.Controls.Remove(lastPair.ChildLabel);
            pnlXmlDynamicColumns.Controls.Remove(lastPair.ParentComboBox);
            pnlXmlDynamicColumns.Controls.Remove(lastPair.ChildComboBox);

            xmlColumnPairs.Remove(lastPair);
            nextXmlPairNumber--;

            UpdateXmlRemoveButtonState();
        }

        /// <summary>
        /// Opdater remove button enabled state
        /// </summary>
        private void UpdateXmlRemoveButtonState()
        {
            btnRemoveXmlPrimaryKey.Enabled = xmlColumnPairs.Count > 0;
        }

        #endregion

        #region Export/Copy Functionality

        /// <summary>
        /// Setup context menu til XML ListBox
        /// </summary>
        private void SetupXmlListBoxContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            var copyItem = new ToolStripMenuItem("Kopiér markerede");
            copyItem.Click += (s, e) => BtnCopyXmlSelected_Click(s, new EventArgs());
            contextMenu.Items.Add(copyItem);

            var selectAllItem = new ToolStripMenuItem("Markér alle");
            selectAllItem.Click += (s, e) => {
                for (int i = 0; i < lstXmlMissingValues.Items.Count; i++)
                    lstXmlMissingValues.SetSelected(i, true);
            };
            contextMenu.Items.Add(selectAllItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var exportItem = new ToolStripMenuItem("Eksportér alle til fil");
            exportItem.Click += (s, e) => BtnExportXmlMissing_Click(s, new EventArgs());
            contextMenu.Items.Add(exportItem);

            lstXmlMissingValues.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// Kopiér markerede items til clipboard
        /// </summary>
        private void BtnCopyXmlSelected_Click(object sender, EventArgs e)
        {
            if (lstXmlMissingValues.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vælg items at kopiere");
                return;
            }

            var selectedText = string.Join("\r\n", lstXmlMissingValues.SelectedItems.Cast<string>());
            Clipboard.SetText(selectedText);

            MessageBox.Show($"Kopieret {lstXmlMissingValues.SelectedItems.Count} items til clipboard");
        }

        /// <summary>
        /// Eksporter alle missing values til tekstfil
        /// </summary>
        private void BtnExportXmlMissing_Click(object sender, EventArgs e)
        {
            if (lstXmlMissingValues.Items.Count == 0)
            {
                MessageBox.Show("Ingen data at eksportere");
                return;
            }

            saveFileDialog1.Filter = "Tekstfiler|*.txt|Alle filer|*.*";
            saveFileDialog1.FileName = $"missing_xml_fk_values_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sb = new StringBuilder();

                    // Header information
                    sb.AppendLine("Missing XML Foreign Key Values Report");
                    sb.AppendLine("======================================");
                    sb.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    sb.AppendLine();

                    // TABELNAVNE
                    sb.AppendLine("Tables:");
                    string parentTableName = currentXmlRepair.ParentTableEntry?.Name ?? Path.GetFileNameWithoutExtension(currentXmlRepair.ParentXmlPath);
                    string childTableName = currentXmlRepair.ChildTableEntry?.Name ?? Path.GetFileNameWithoutExtension(currentXmlRepair.ChildXmlPath);
                    sb.AppendLine($"  Parent Table: {parentTableName}");
                    sb.AppendLine($"  Child Table: {childTableName}");
                    sb.AppendLine();

                    // FILE PATHS
                    sb.AppendLine("File Paths:");
                    sb.AppendLine($"  Parent XML: {currentXmlRepair.ParentXmlPath}");
                    sb.AppendLine($"  Child XML: {currentXmlRepair.ChildXmlPath}");
                    sb.AppendLine();

                    // KOLONNENAVNE MED DETALJER
                    sb.AppendLine("Foreign Key Columns:");
                    sb.AppendLine("  Parent Columns:");
                    for (int i = 0; i < currentXmlRepair.ParentKeyColumns.Count; i++)
                    {
                        string columnID = currentXmlRepair.ParentKeyColumns[i];
                        string columnName = GetColumnDisplayName(columnID, currentXmlRepair.ParentTableEntry);
                        sb.AppendLine($"    {i + 1}. {columnID} - {columnName}");
                    }
                    sb.AppendLine();
                    sb.AppendLine("  Child Columns:");
                    for (int i = 0; i < currentXmlRepair.ChildKeyColumns.Count; i++)
                    {
                        string columnID = currentXmlRepair.ChildKeyColumns[i];
                        string columnName = GetColumnDisplayName(columnID, currentXmlRepair.ChildTableEntry);
                        sb.AppendLine($"    {i + 1}. {columnID} - {columnName}");
                    }
                    sb.AppendLine();

                    // Key type
                    string keyType = currentXmlRepair.ParentKeyColumns.Count > 1 ? "Composite Primary Key" : "Single Primary Key";
                    sb.AppendLine($"Key Type: {keyType}");

                    if (currentXmlRepair.ParentKeyColumns.Count > 1)
                    {
                        sb.AppendLine($"Key Format: {string.Join(" | ", currentXmlRepair.ParentKeyColumns)}");
                    }

                    sb.AppendLine($"Total Missing Values: {lstXmlMissingValues.Items.Count}");
                    sb.AppendLine();
                    sb.AppendLine("Missing Values:");
                    sb.AppendLine("---------------");

                    foreach (var item in lstXmlMissingValues.Items)
                    {
                        sb.AppendLine(item.ToString());
                    }

                    File.WriteAllText(saveFileDialog1.FileName, sb.ToString(), Encoding.UTF8);

                    MessageBox.Show($"Eksporteret {lstXmlMissingValues.Items.Count} værdier til {saveFileDialog1.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fejl ved eksport: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Hjælpemetode til at få kolonnenavn med metadata
        /// </summary>
        private string GetColumnDisplayName(string columnID, TableIndexEntry tableEntry)
        {
            if (tableEntry != null && tableEntry.Columns != null)
            {
                var column = tableEntry.Columns.FirstOrDefault(c => c.ColumnID == columnID);
                if (column != null)
                {
                    return $"{column.Name} ({column.DataType})";
                }
            }

            return "(metadata ikke tilgængelig)";
        }

        /// <summary>
        /// Keyboard shortcuts til XML ListBox
        /// </summary>
        private void LstXmlMissingValues_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                BtnCopyXmlSelected_Click(sender, new EventArgs());
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                for (int i = 0; i < lstXmlMissingValues.Items.Count; i++)
                    lstXmlMissingValues.SetSelected(i, true);
                e.Handled = true;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Ekstrahér AVID version fra tableIndex path
        /// Eksempel: C:\AVID.SA.18005.1\Indices\tableIndex.xml -> AVID.SA.18005.1
        /// </summary>
        private string ExtractAvidVersion(string tableIndexPath)
        {
            try
            {
                // Gå op i directory strukturen og find AVID mappen
                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(tableIndexPath));

                while (dir != null)
                {
                    // Match AVID pattern: AVID.SA.xxxxx.x eller lignende
                    if (System.Text.RegularExpressions.Regex.IsMatch(dir.Name, @"^AVID\.[A-ZÆØÅa-zæøå]{2,}\.\d+\.\d+"))
                    {
                        return dir.Name;
                    }
                    dir = dir.Parent;
                }

                return "Unknown_AVID";
            }
            catch
            {
                return "Unknown_AVID";
            }
        }

        #endregion

        #region Ouput

        /// <summary>
        /// Opret struktureret output directory for FK repair på Desktop
        /// </summary>
        private string CreateRepairOutputDirectory(string avidVersion, string tableName)
        {
            string basePath = GetOutputBasePath();
            string parentFolder = Path.Combine(basePath, OUTPUT_FOLDER_NAME);

            // Opret parent folder hvis den ikke findes
            Directory.CreateDirectory(parentFolder);

            // Find næste version nummer
            string searchPattern = $"repair_{avidVersion}_{tableName}_v*";
            var existingFolders = Directory.GetDirectories(parentFolder, searchPattern);

            int nextVersion = 1;
            if (existingFolders.Length > 0)
            {
                int maxVersion = 0;
                foreach (var folder in existingFolders)
                {
                    string folderName = Path.GetFileName(folder);
                    var match = System.Text.RegularExpressions.Regex.Match(folderName, @"_v(\d+)$");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int version))
                    {
                        maxVersion = Math.Max(maxVersion, version);
                    }
                }
                nextVersion = maxVersion + 1;
            }

            string repairFolderName = $"repair_{avidVersion}_{tableName}_v{nextVersion}";
            string outputDirectory = Path.Combine(parentFolder, repairFolderName);
            Directory.CreateDirectory(outputDirectory);

            return outputDirectory;
        }

        /// <summary>
        /// Button handler: Vælg custom output directory
        /// </summary>
        private void btnChangeOutput_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Vælg output mappe for XML FK repairs";
                folderDialog.SelectedPath = customOutputPath ??
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    customOutputPath = folderDialog.SelectedPath;
                    UpdateOutputPathLabel();
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
        /// Hent base path for output (Desktop eller custom)
        /// </summary>
        private string GetOutputBasePath()
        {
            return customOutputPath ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
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
                // Fjern controls fra form (hvis de blev tilføjet)
                this.Controls.Remove(lblOutputPath);
                this.Controls.Remove(btnChangeOutput);

                // Tilføj til back panel i stedet
                lblOutputPath.Dock = DockStyle.None;
                lblOutputPath.AutoSize = true;
                lblOutputPath.Location = new Point(200, 17);

                btnChangeOutput.Dock = DockStyle.Right;
                btnChangeOutput.Margin = new Padding(0, 7, 15, 7);

                backPanel.Controls.Add(lblOutputPath);
                backPanel.Controls.Add(btnChangeOutput);
            }
        }

        #endregion
    }
}