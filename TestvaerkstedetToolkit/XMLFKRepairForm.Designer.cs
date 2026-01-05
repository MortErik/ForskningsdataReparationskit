using System.Drawing;
using System.Windows.Forms;

namespace TestvaerkstedetToolkit
{
    partial class XMLFKRepairForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTableIndex = new Label();
            txtTableIndex = new TextBox();
            btnBrowseTableIndex = new Button();
            lblParentTable = new Label();
            cmbParentTable = new ComboBox();
            lblParentXml = new Label();
            txtParentXml = new TextBox();
            btnBrowseParentXml = new Button();
            lblChildTable = new Label();
            cmbChildTable = new ComboBox();
            lblChildXml = new Label();
            txtChildXml = new TextBox();
            btnBrowseChildXml = new Button();
            lblXmlMapping = new Label();
            cmbParentXmlColumns = new ComboBox();
            cmbChildXmlColumns = new ComboBox();
            lblXmlCompositeKey = new Label();
            btnAddXmlPrimaryKey = new Button();
            btnRemoveXmlPrimaryKey = new Button();
            pnlXmlDynamicColumns = new Panel();
            btnAnalyzeXmlFK = new Button();
            lblXmlFKStats = new Label();
            lstXmlMissingValues = new ListBox();
            btnExportXmlMissing = new Button();
            btnCopyXmlSelected = new Button();
            lblIntegrityDesc = new Label();
            txtIntegrityDesc = new TextBox();
            btnGenerateFixedXml = new Button();
            progressBar1 = new ProgressBar();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            lblTableIndexInfo = new Label();
            lblParentXmlCol = new Label();
            lblChildXmlCol = new Label();
            lblOutputPath = new Label();
            btnChangeOutput = new Button();
            separator1 = new Label();
            separator2 = new Label();
            SuspendLayout();
            // 
            // lblTableIndex
            // 
            lblTableIndex.AutoSize = true;
            lblTableIndex.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            lblTableIndex.Location = new Point(20, 75);
            lblTableIndex.Name = "lblTableIndex";
            lblTableIndex.Size = new Size(74, 13);
            lblTableIndex.TabIndex = 0;
            lblTableIndex.Text = "TableIndex:";
            // 
            // txtTableIndex
            // 
            txtTableIndex.BackColor = SystemColors.Control;
            txtTableIndex.ForeColor = Color.Blue;
            txtTableIndex.Location = new Point(130, 75);
            txtTableIndex.Name = "txtTableIndex";
            txtTableIndex.ReadOnly = true;
            txtTableIndex.Size = new Size(800, 23);
            txtTableIndex.TabIndex = 1;
            // 
            // btnBrowseTableIndex
            // 
            btnBrowseTableIndex.Location = new Point(940, 73);
            btnBrowseTableIndex.Name = "btnBrowseTableIndex";
            btnBrowseTableIndex.Size = new Size(75, 23);
            btnBrowseTableIndex.TabIndex = 2;
            btnBrowseTableIndex.Text = "Browse...";
            btnBrowseTableIndex.UseVisualStyleBackColor = true;
            btnBrowseTableIndex.Click += BtnBrowseTableIndex_Click;
            // 
            // lblParentTable
            // 
            lblParentTable.AutoSize = true;
            lblParentTable.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            lblParentTable.Location = new Point(20, 170);
            lblParentTable.Name = "lblParentTable";
            lblParentTable.Size = new Size(84, 13);
            lblParentTable.TabIndex = 0;
            lblParentTable.Text = "Parent Table:";
            // 
            // cmbParentTable
            // 
            cmbParentTable.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbParentTable.Enabled = false;
            cmbParentTable.Location = new Point(130, 170);
            cmbParentTable.Name = "cmbParentTable";
            cmbParentTable.Size = new Size(800, 23);
            cmbParentTable.TabIndex = 3;
            cmbParentTable.SelectedIndexChanged += CmbParentTable_SelectedIndexChanged;
            // 
            // lblParentXml
            // 
            lblParentXml.AutoSize = true;
            lblParentXml.Location = new Point(21, 200);
            lblParentXml.Name = "lblParentXml";
            lblParentXml.Size = new Size(98, 15);
            lblParentXml.TabIndex = 0;
            lblParentXml.Text = "Parent XML Path:";
            // 
            // txtParentXml
            // 
            txtParentXml.BackColor = SystemColors.Control;
            txtParentXml.Location = new Point(130, 200);
            txtParentXml.Name = "txtParentXml";
            txtParentXml.ReadOnly = true;
            txtParentXml.Size = new Size(800, 23);
            txtParentXml.TabIndex = 4;
            // 
            // btnBrowseParentXml
            // 
            btnBrowseParentXml.Location = new Point(940, 198);
            btnBrowseParentXml.Name = "btnBrowseParentXml";
            btnBrowseParentXml.Size = new Size(75, 23);
            btnBrowseParentXml.TabIndex = 5;
            btnBrowseParentXml.Text = "Browse...";
            btnBrowseParentXml.UseVisualStyleBackColor = true;
            btnBrowseParentXml.Click += BtnBrowseParentXml_Click;
            // 
            // lblChildTable
            // 
            lblChildTable.AutoSize = true;
            lblChildTable.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            lblChildTable.Location = new Point(20, 235);
            lblChildTable.Name = "lblChildTable";
            lblChildTable.Size = new Size(75, 13);
            lblChildTable.TabIndex = 0;
            lblChildTable.Text = "Child Table:";
            // 
            // cmbChildTable
            // 
            cmbChildTable.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChildTable.Enabled = false;
            cmbChildTable.Location = new Point(130, 235);
            cmbChildTable.Name = "cmbChildTable";
            cmbChildTable.Size = new Size(800, 23);
            cmbChildTable.TabIndex = 6;
            cmbChildTable.SelectedIndexChanged += CmbChildTable_SelectedIndexChanged;
            // 
            // lblChildXml
            // 
            lblChildXml.AutoSize = true;
            lblChildXml.Location = new Point(21, 265);
            lblChildXml.Name = "lblChildXml";
            lblChildXml.Size = new Size(92, 15);
            lblChildXml.TabIndex = 0;
            lblChildXml.Text = "Child XML Path:";
            // 
            // txtChildXml
            // 
            txtChildXml.BackColor = SystemColors.Control;
            txtChildXml.Location = new Point(130, 265);
            txtChildXml.Name = "txtChildXml";
            txtChildXml.ReadOnly = true;
            txtChildXml.Size = new Size(800, 23);
            txtChildXml.TabIndex = 7;
            // 
            // btnBrowseChildXml
            // 
            btnBrowseChildXml.Location = new Point(940, 263);
            btnBrowseChildXml.Name = "btnBrowseChildXml";
            btnBrowseChildXml.Size = new Size(75, 23);
            btnBrowseChildXml.TabIndex = 8;
            btnBrowseChildXml.Text = "Browse...";
            btnBrowseChildXml.UseVisualStyleBackColor = true;
            btnBrowseChildXml.Click += BtnBrowseChildXml_Click;
            // 
            // lblXmlMapping
            // 
            lblXmlMapping.AutoSize = true;
            lblXmlMapping.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            lblXmlMapping.Location = new Point(20, 315);
            lblXmlMapping.Name = "lblXmlMapping";
            lblXmlMapping.Size = new Size(170, 13);
            lblXmlMapping.TabIndex = 0;
            lblXmlMapping.Text = "Foreign Key Mapping (Base):";
            // 
            // cmbParentXmlColumns
            // 
            cmbParentXmlColumns.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbParentXmlColumns.Location = new Point(160, 335);
            cmbParentXmlColumns.Name = "cmbParentXmlColumns";
            cmbParentXmlColumns.Size = new Size(450, 23);
            cmbParentXmlColumns.TabIndex = 9;
            // 
            // cmbChildXmlColumns
            // 
            cmbChildXmlColumns.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChildXmlColumns.Location = new Point(160, 365);
            cmbChildXmlColumns.Name = "cmbChildXmlColumns";
            cmbChildXmlColumns.Size = new Size(450, 23);
            cmbChildXmlColumns.TabIndex = 10;
            // 
            // lblXmlCompositeKey
            // 
            lblXmlCompositeKey.AutoSize = true;
            lblXmlCompositeKey.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            lblXmlCompositeKey.Location = new Point(20, 400);
            lblXmlCompositeKey.Name = "lblXmlCompositeKey";
            lblXmlCompositeKey.Size = new Size(165, 13);
            lblXmlCompositeKey.TabIndex = 12;
            lblXmlCompositeKey.Text = "Sammensatte PK (Optional):";
            // 
            // btnAddXmlPrimaryKey
            // 
            btnAddXmlPrimaryKey.Location = new Point(200, 398);
            btnAddXmlPrimaryKey.Name = "btnAddXmlPrimaryKey";
            btnAddXmlPrimaryKey.Size = new Size(100, 23);
            btnAddXmlPrimaryKey.TabIndex = 11;
            btnAddXmlPrimaryKey.Text = "Add Column";
            btnAddXmlPrimaryKey.UseVisualStyleBackColor = true;
            btnAddXmlPrimaryKey.Click += BtnAddXmlPrimaryKey_Click;
            // 
            // btnRemoveXmlPrimaryKey
            // 
            btnRemoveXmlPrimaryKey.Enabled = false;
            btnRemoveXmlPrimaryKey.Location = new Point(310, 398);
            btnRemoveXmlPrimaryKey.Name = "btnRemoveXmlPrimaryKey";
            btnRemoveXmlPrimaryKey.Size = new Size(100, 23);
            btnRemoveXmlPrimaryKey.TabIndex = 12;
            btnRemoveXmlPrimaryKey.Text = "Remove Last";
            btnRemoveXmlPrimaryKey.UseVisualStyleBackColor = true;
            btnRemoveXmlPrimaryKey.Click += BtnRemoveXmlPrimaryKey_Click;
            // 
            // pnlXmlDynamicColumns
            // 
            pnlXmlDynamicColumns.AutoScroll = true;
            pnlXmlDynamicColumns.BorderStyle = BorderStyle.FixedSingle;
            pnlXmlDynamicColumns.Location = new Point(40, 430);
            pnlXmlDynamicColumns.Name = "pnlXmlDynamicColumns";
            pnlXmlDynamicColumns.Size = new Size(970, 120);
            pnlXmlDynamicColumns.TabIndex = 13;
            // 
            // btnAnalyzeXmlFK
            // 
            btnAnalyzeXmlFK.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnAnalyzeXmlFK.Location = new Point(40, 565);
            btnAnalyzeXmlFK.Name = "btnAnalyzeXmlFK";
            btnAnalyzeXmlFK.Size = new Size(150, 30);
            btnAnalyzeXmlFK.TabIndex = 14;
            btnAnalyzeXmlFK.Text = "Analyze FK";
            btnAnalyzeXmlFK.UseVisualStyleBackColor = true;
            btnAnalyzeXmlFK.Click += BtnAnalyzeXmlFK_Click;
            // 
            // lblXmlFKStats
            // 
            lblXmlFKStats.AutoSize = true;
            lblXmlFKStats.ForeColor = Color.DarkBlue;
            lblXmlFKStats.Location = new Point(200, 573);
            lblXmlFKStats.Name = "lblXmlFKStats";
            lblXmlFKStats.Size = new Size(155, 15);
            lblXmlFKStats.TabIndex = 0;
            lblXmlFKStats.Text = "Klik 'Analyze FK' for at starte";
            // 
            // lstXmlMissingValues
            // 
            lstXmlMissingValues.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstXmlMissingValues.ItemHeight = 15;
            lstXmlMissingValues.Location = new Point(40, 610);
            lstXmlMissingValues.Name = "lstXmlMissingValues";
            lstXmlMissingValues.SelectionMode = SelectionMode.MultiExtended;
            lstXmlMissingValues.Size = new Size(500, 199);
            lstXmlMissingValues.TabIndex = 15;
            lstXmlMissingValues.KeyDown += LstXmlMissingValues_KeyDown;
            // 
            // btnExportXmlMissing
            // 
            btnExportXmlMissing.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportXmlMissing.Enabled = false;
            btnExportXmlMissing.Location = new Point(560, 610);
            btnExportXmlMissing.Name = "btnExportXmlMissing";
            btnExportXmlMissing.Size = new Size(120, 25);
            btnExportXmlMissing.TabIndex = 16;
            btnExportXmlMissing.Text = "Export til fil";
            btnExportXmlMissing.UseVisualStyleBackColor = true;
            btnExportXmlMissing.Click += BtnExportXmlMissing_Click;
            // 
            // btnCopyXmlSelected
            // 
            btnCopyXmlSelected.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopyXmlSelected.Enabled = false;
            btnCopyXmlSelected.Location = new Point(560, 640);
            btnCopyXmlSelected.Name = "btnCopyXmlSelected";
            btnCopyXmlSelected.Size = new Size(120, 25);
            btnCopyXmlSelected.TabIndex = 17;
            btnCopyXmlSelected.Text = "Kopiér markerede";
            btnCopyXmlSelected.UseVisualStyleBackColor = true;
            btnCopyXmlSelected.Click += BtnCopyXmlSelected_Click;
            // 
            // lblIntegrityDesc
            // 
            lblIntegrityDesc.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblIntegrityDesc.AutoSize = true;
            lblIntegrityDesc.Location = new Point(560, 675);
            lblIntegrityDesc.Name = "lblIntegrityDesc";
            lblIntegrityDesc.Size = new Size(114, 15);
            lblIntegrityDesc.TabIndex = 0;
            lblIntegrityDesc.Text = "Kolonnebeskrivelse:";
            // 
            // txtIntegrityDesc
            // 
            txtIntegrityDesc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtIntegrityDesc.Location = new Point(560, 692);
            txtIntegrityDesc.Multiline = true;
            txtIntegrityDesc.Name = "txtIntegrityDesc";
            txtIntegrityDesc.Size = new Size(600, 52);
            txtIntegrityDesc.TabIndex = 18;
            txtIntegrityDesc.Text = "Betydning ukendt. Rækken er tilføjet under aflevering til arkiv, for at sikre referentiel integritet i databasen af hensyn til langtidsbevaring";
            // 
            // btnGenerateFixedXml
            // 
            btnGenerateFixedXml.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGenerateFixedXml.BackColor = Color.LightGreen;
            btnGenerateFixedXml.Enabled = false;
            btnGenerateFixedXml.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnGenerateFixedXml.Location = new Point(560, 750);
            btnGenerateFixedXml.Name = "btnGenerateFixedXml";
            btnGenerateFixedXml.Size = new Size(220, 35);
            btnGenerateFixedXml.TabIndex = 19;
            btnGenerateFixedXml.Text = "Generer Nye Rækker til Parent";
            btnGenerateFixedXml.UseVisualStyleBackColor = false;
            btnGenerateFixedXml.Click += BtnGenerateFixedXml_Click;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(12, 825);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(1156, 23);
            progressBar1.TabIndex = 20;
            progressBar1.Visible = false;
            // 
            // lblTableIndexInfo
            // 
            lblTableIndexInfo.Font = new Font("Microsoft Sans Serif", 7.5F, FontStyle.Italic);
            lblTableIndexInfo.ForeColor = Color.DarkGreen;
            lblTableIndexInfo.Location = new Point(130, 110);
            lblTableIndexInfo.Name = "lblTableIndexInfo";
            lblTableIndexInfo.Size = new Size(800, 30);
            lblTableIndexInfo.TabIndex = 3;
            lblTableIndexInfo.Text = "💡 TableIndex giver bedre kolonnenavne, datatyper og beskrivelser.\nKan springes over - programmet parser direkte fra XML.";
            // 
            // lblParentXmlCol
            // 
            lblParentXmlCol.AutoSize = true;
            lblParentXmlCol.Location = new Point(40, 335);
            lblParentXmlCol.Name = "lblParentXmlCol";
            lblParentXmlCol.Size = new Size(90, 15);
            lblParentXmlCol.TabIndex = 10;
            lblParentXmlCol.Text = "Parent Column:";
            // 
            // lblChildXmlCol
            // 
            lblChildXmlCol.AutoSize = true;
            lblChildXmlCol.Location = new Point(40, 365);
            lblChildXmlCol.Name = "lblChildXmlCol";
            lblChildXmlCol.Size = new Size(84, 15);
            lblChildXmlCol.TabIndex = 11;
            lblChildXmlCol.Text = "Child Column:";
            // 
            // lblOutputPath
            // 
            lblOutputPath.AutoSize = true;
            lblOutputPath.Font = new Font("Microsoft Sans Serif", 8.25F);
            lblOutputPath.ForeColor = Color.DarkGreen;
            lblOutputPath.Location = new Point(200, 15);
            lblOutputPath.Name = "lblOutputPath";
            lblOutputPath.Size = new Size(200, 13);
            lblOutputPath.TabIndex = 200;
            lblOutputPath.Text = "Output destination: Desktop/XML_FK_Repairs";
            // 
            // btnChangeOutput
            // 
            btnChangeOutput.Location = new Point(750, 10);
            btnChangeOutput.Name = "btnChangeOutput";
            btnChangeOutput.Size = new Size(110, 25);
            btnChangeOutput.TabIndex = 201;
            btnChangeOutput.Text = "📁 Vælg Output";
            btnChangeOutput.UseVisualStyleBackColor = true;
            btnChangeOutput.Click += btnChangeOutput_Click;
            // 
            // separator1
            // 
            separator1.BorderStyle = BorderStyle.Fixed3D;
            separator1.Location = new Point(20, 155);
            separator1.Name = "separator1";
            separator1.Size = new Size(1120, 2);
            separator1.TabIndex = 4;
            // 
            // separator2
            // 
            separator2.BorderStyle = BorderStyle.Fixed3D;
            separator2.Location = new Point(20, 300);
            separator2.Name = "separator2";
            separator2.Size = new Size(1120, 2);
            separator2.TabIndex = 9;
            separator2.Click += separator2_Click;
            // 
            // XMLFKRepairForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1180, 860);
            Controls.Add(lblTableIndex);
            Controls.Add(txtTableIndex);
            Controls.Add(btnBrowseTableIndex);
            Controls.Add(lblTableIndexInfo);
            Controls.Add(separator1);
            Controls.Add(lblParentTable);
            Controls.Add(cmbParentTable);
            Controls.Add(lblParentXml);
            Controls.Add(txtParentXml);
            Controls.Add(btnBrowseParentXml);
            Controls.Add(lblChildTable);
            Controls.Add(cmbChildTable);
            Controls.Add(lblChildXml);
            Controls.Add(txtChildXml);
            Controls.Add(btnBrowseChildXml);
            Controls.Add(separator2);
            Controls.Add(lblXmlMapping);
            Controls.Add(lblParentXmlCol);
            Controls.Add(cmbParentXmlColumns);
            Controls.Add(lblChildXmlCol);
            Controls.Add(cmbChildXmlColumns);
            Controls.Add(lblXmlCompositeKey);
            Controls.Add(btnAddXmlPrimaryKey);
            Controls.Add(btnRemoveXmlPrimaryKey);
            Controls.Add(pnlXmlDynamicColumns);
            Controls.Add(btnAnalyzeXmlFK);
            Controls.Add(lblXmlFKStats);
            Controls.Add(lstXmlMissingValues);
            Controls.Add(btnExportXmlMissing);
            Controls.Add(btnCopyXmlSelected);
            Controls.Add(lblIntegrityDesc);
            Controls.Add(txtIntegrityDesc);
            Controls.Add(btnGenerateFixedXml);
            Controls.Add(progressBar1);
            MinimumSize = new Size(1196, 899);
            Name = "XMLFKRepairForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Foreign Key Repair - XML";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        // Control declarations
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.Button btnChangeOutput;
        private System.Windows.Forms.Label lblTableIndex;
        private System.Windows.Forms.TextBox txtTableIndex;
        private System.Windows.Forms.Button btnBrowseTableIndex;
        private System.Windows.Forms.Label lblParentTable;
        private System.Windows.Forms.ComboBox cmbParentTable;
        private System.Windows.Forms.Label lblParentXml;
        private System.Windows.Forms.TextBox txtParentXml;
        private System.Windows.Forms.Button btnBrowseParentXml;
        private System.Windows.Forms.Label lblChildTable;
        private System.Windows.Forms.ComboBox cmbChildTable;
        private System.Windows.Forms.Label lblChildXml;
        private System.Windows.Forms.TextBox txtChildXml;
        private System.Windows.Forms.Button btnBrowseChildXml;
        private System.Windows.Forms.Label lblXmlMapping;
        private System.Windows.Forms.ComboBox cmbParentXmlColumns;
        private System.Windows.Forms.ComboBox cmbChildXmlColumns;
        private System.Windows.Forms.Label lblXmlCompositeKey;
        private System.Windows.Forms.Button btnAddXmlPrimaryKey;
        private System.Windows.Forms.Button btnRemoveXmlPrimaryKey;
        private System.Windows.Forms.Panel pnlXmlDynamicColumns;
        private System.Windows.Forms.Button btnAnalyzeXmlFK;
        private System.Windows.Forms.Label lblXmlFKStats;
        private System.Windows.Forms.ListBox lstXmlMissingValues;
        private System.Windows.Forms.Button btnExportXmlMissing;
        private System.Windows.Forms.Button btnCopyXmlSelected;
        private System.Windows.Forms.Label lblIntegrityDesc;
        private System.Windows.Forms.TextBox txtIntegrityDesc;
        private System.Windows.Forms.Button btnGenerateFixedXml;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Label lblTableIndexInfo;
        private Label lblParentXmlCol;
        private Label lblChildXmlCol;
        private Label separator1;
        private Label separator2;
    }
}