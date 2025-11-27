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
            this.lblTableIndex = new System.Windows.Forms.Label();
            this.txtTableIndex = new System.Windows.Forms.TextBox();
            this.btnBrowseTableIndex = new System.Windows.Forms.Button();
            this.lblParentTable = new System.Windows.Forms.Label();
            this.cmbParentTable = new System.Windows.Forms.ComboBox();
            this.lblParentXml = new System.Windows.Forms.Label();
            this.txtParentXml = new System.Windows.Forms.TextBox();
            this.btnBrowseParentXml = new System.Windows.Forms.Button();
            this.lblChildTable = new System.Windows.Forms.Label();
            this.cmbChildTable = new System.Windows.Forms.ComboBox();
            this.lblChildXml = new System.Windows.Forms.Label();
            this.txtChildXml = new System.Windows.Forms.TextBox();
            this.btnBrowseChildXml = new System.Windows.Forms.Button();
            this.lblXmlMapping = new System.Windows.Forms.Label();
            this.cmbParentXmlColumns = new System.Windows.Forms.ComboBox();
            this.cmbChildXmlColumns = new System.Windows.Forms.ComboBox();
            this.lblXmlCompositeKey = new System.Windows.Forms.Label();
            this.btnAddXmlPrimaryKey = new System.Windows.Forms.Button();
            this.btnRemoveXmlPrimaryKey = new System.Windows.Forms.Button();
            this.pnlXmlDynamicColumns = new System.Windows.Forms.Panel();
            this.btnAnalyzeXmlFK = new System.Windows.Forms.Button();
            this.lblXmlFKStats = new System.Windows.Forms.Label();
            this.lstXmlMissingValues = new System.Windows.Forms.ListBox();
            this.btnExportXmlMissing = new System.Windows.Forms.Button();
            this.btnCopyXmlSelected = new System.Windows.Forms.Button();
            this.lblIntegrityDesc = new System.Windows.Forms.Label();
            this.txtIntegrityDesc = new System.Windows.Forms.TextBox();
            this.btnGenerateFixedXml = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.lblTableIndexInfo = new System.Windows.Forms.Label();
            this.separator1 = new System.Windows.Forms.Label();
            this.separator2 = new System.Windows.Forms.Label();
            this.lblParentXmlCol = new System.Windows.Forms.Label();
            this.lblChildXmlCol = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTableIndex
            // 
            this.lblTableIndex.AutoSize = true;
            this.lblTableIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTableIndex.Location = new System.Drawing.Point(20, 75);
            this.lblTableIndex.Name = "lblTableIndex";
            this.lblTableIndex.Size = new System.Drawing.Size(74, 13);
            this.lblTableIndex.TabIndex = 0;
            this.lblTableIndex.Text = "TableIndex:";
            // 
            // txtTableIndex
            // 
            this.txtTableIndex.BackColor = System.Drawing.SystemColors.Control;
            this.txtTableIndex.ForeColor = System.Drawing.Color.Blue;
            this.txtTableIndex.Location = new System.Drawing.Point(130, 75);
            this.txtTableIndex.Name = "txtTableIndex";
            this.txtTableIndex.ReadOnly = true;
            this.txtTableIndex.Size = new System.Drawing.Size(800, 20);
            this.txtTableIndex.TabIndex = 1;
            // 
            // btnBrowseTableIndex
            // 
            this.btnBrowseTableIndex.Location = new System.Drawing.Point(940, 73);
            this.btnBrowseTableIndex.Name = "btnBrowseTableIndex";
            this.btnBrowseTableIndex.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseTableIndex.TabIndex = 2;
            this.btnBrowseTableIndex.Text = "Browse...";
            this.btnBrowseTableIndex.UseVisualStyleBackColor = true;
            this.btnBrowseTableIndex.Click += new System.EventHandler(this.BtnBrowseTableIndex_Click);
            // 
            // lblParentTable
            // 
            this.lblParentTable.AutoSize = true;
            this.lblParentTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblParentTable.Location = new System.Drawing.Point(20, 170);
            this.lblParentTable.Name = "lblParentTable";
            this.lblParentTable.Size = new System.Drawing.Size(84, 13);
            this.lblParentTable.TabIndex = 0;
            this.lblParentTable.Text = "Parent Table:";
            // 
            // cmbParentTable
            // 
            this.cmbParentTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParentTable.Enabled = false;
            this.cmbParentTable.Location = new System.Drawing.Point(130, 170);
            this.cmbParentTable.Name = "cmbParentTable";
            this.cmbParentTable.Size = new System.Drawing.Size(800, 21);
            this.cmbParentTable.TabIndex = 3;
            this.cmbParentTable.SelectedIndexChanged += new System.EventHandler(this.CmbParentTable_SelectedIndexChanged);
            // 
            // lblParentXml
            // 
            this.lblParentXml.AutoSize = true;
            this.lblParentXml.Location = new System.Drawing.Point(21, 200);
            this.lblParentXml.Name = "lblParentXml";
            this.lblParentXml.Size = new System.Drawing.Size(91, 13);
            this.lblParentXml.TabIndex = 0;
            this.lblParentXml.Text = "Parent XML Path:";
            // 
            // txtParentXml
            // 
            this.txtParentXml.BackColor = System.Drawing.SystemColors.Control;
            this.txtParentXml.Location = new System.Drawing.Point(130, 200);
            this.txtParentXml.Name = "txtParentXml";
            this.txtParentXml.ReadOnly = true;
            this.txtParentXml.Size = new System.Drawing.Size(800, 20);
            this.txtParentXml.TabIndex = 4;
            // 
            // btnBrowseParentXml
            // 
            this.btnBrowseParentXml.Location = new System.Drawing.Point(940, 198);
            this.btnBrowseParentXml.Name = "btnBrowseParentXml";
            this.btnBrowseParentXml.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseParentXml.TabIndex = 5;
            this.btnBrowseParentXml.Text = "Browse...";
            this.btnBrowseParentXml.UseVisualStyleBackColor = true;
            this.btnBrowseParentXml.Click += new System.EventHandler(this.BtnBrowseParentXml_Click);
            // 
            // lblChildTable
            // 
            this.lblChildTable.AutoSize = true;
            this.lblChildTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblChildTable.Location = new System.Drawing.Point(20, 235);
            this.lblChildTable.Name = "lblChildTable";
            this.lblChildTable.Size = new System.Drawing.Size(75, 13);
            this.lblChildTable.TabIndex = 0;
            this.lblChildTable.Text = "Child Table:";
            // 
            // cmbChildTable
            // 
            this.cmbChildTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChildTable.Enabled = false;
            this.cmbChildTable.Location = new System.Drawing.Point(130, 235);
            this.cmbChildTable.Name = "cmbChildTable";
            this.cmbChildTable.Size = new System.Drawing.Size(800, 21);
            this.cmbChildTable.TabIndex = 6;
            this.cmbChildTable.SelectedIndexChanged += new System.EventHandler(this.CmbChildTable_SelectedIndexChanged);
            // 
            // lblChildXml
            // 
            this.lblChildXml.AutoSize = true;
            this.lblChildXml.Location = new System.Drawing.Point(21, 265);
            this.lblChildXml.Name = "lblChildXml";
            this.lblChildXml.Size = new System.Drawing.Size(83, 13);
            this.lblChildXml.TabIndex = 0;
            this.lblChildXml.Text = "Child XML Path:";
            // 
            // txtChildXml
            // 
            this.txtChildXml.BackColor = System.Drawing.SystemColors.Control;
            this.txtChildXml.Location = new System.Drawing.Point(130, 265);
            this.txtChildXml.Name = "txtChildXml";
            this.txtChildXml.ReadOnly = true;
            this.txtChildXml.Size = new System.Drawing.Size(800, 20);
            this.txtChildXml.TabIndex = 7;
            // 
            // btnBrowseChildXml
            // 
            this.btnBrowseChildXml.Location = new System.Drawing.Point(940, 263);
            this.btnBrowseChildXml.Name = "btnBrowseChildXml";
            this.btnBrowseChildXml.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseChildXml.TabIndex = 8;
            this.btnBrowseChildXml.Text = "Browse...";
            this.btnBrowseChildXml.UseVisualStyleBackColor = true;
            this.btnBrowseChildXml.Click += new System.EventHandler(this.BtnBrowseChildXml_Click);
            // 
            // lblXmlMapping
            // 
            this.lblXmlMapping.AutoSize = true;
            this.lblXmlMapping.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblXmlMapping.Location = new System.Drawing.Point(20, 315);
            this.lblXmlMapping.Name = "lblXmlMapping";
            this.lblXmlMapping.Size = new System.Drawing.Size(170, 13);
            this.lblXmlMapping.TabIndex = 0;
            this.lblXmlMapping.Text = "Foreign Key Mapping (Base):";
            // 
            // cmbParentXmlColumns
            // 
            this.cmbParentXmlColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParentXmlColumns.Location = new System.Drawing.Point(160, 335);
            this.cmbParentXmlColumns.Name = "cmbParentXmlColumns";
            this.cmbParentXmlColumns.Size = new System.Drawing.Size(450, 21);
            this.cmbParentXmlColumns.TabIndex = 9;
            // 
            // cmbChildXmlColumns
            // 
            this.cmbChildXmlColumns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChildXmlColumns.Location = new System.Drawing.Point(160, 365);
            this.cmbChildXmlColumns.Name = "cmbChildXmlColumns";
            this.cmbChildXmlColumns.Size = new System.Drawing.Size(450, 21);
            this.cmbChildXmlColumns.TabIndex = 10;
            // 
            // lblXmlCompositeKey
            // 
            this.lblXmlCompositeKey.AutoSize = true;
            this.lblXmlCompositeKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblXmlCompositeKey.Location = new System.Drawing.Point(20, 400);
            this.lblXmlCompositeKey.Name = "lblXmlCompositeKey";
            this.lblXmlCompositeKey.Size = new System.Drawing.Size(165, 13);
            this.lblXmlCompositeKey.TabIndex = 12;
            this.lblXmlCompositeKey.Text = "Sammensatte PK (Optional):";
            // 
            // btnAddXmlPrimaryKey
            // 
            this.btnAddXmlPrimaryKey.Location = new System.Drawing.Point(200, 398);
            this.btnAddXmlPrimaryKey.Name = "btnAddXmlPrimaryKey";
            this.btnAddXmlPrimaryKey.Size = new System.Drawing.Size(100, 23);
            this.btnAddXmlPrimaryKey.TabIndex = 11;
            this.btnAddXmlPrimaryKey.Text = "Add Column";
            this.btnAddXmlPrimaryKey.UseVisualStyleBackColor = true;
            this.btnAddXmlPrimaryKey.Click += new System.EventHandler(this.BtnAddXmlPrimaryKey_Click);
            // 
            // btnRemoveXmlPrimaryKey
            // 
            this.btnRemoveXmlPrimaryKey.Enabled = false;
            this.btnRemoveXmlPrimaryKey.Location = new System.Drawing.Point(310, 398);
            this.btnRemoveXmlPrimaryKey.Name = "btnRemoveXmlPrimaryKey";
            this.btnRemoveXmlPrimaryKey.Size = new System.Drawing.Size(100, 23);
            this.btnRemoveXmlPrimaryKey.TabIndex = 12;
            this.btnRemoveXmlPrimaryKey.Text = "Remove Last";
            this.btnRemoveXmlPrimaryKey.UseVisualStyleBackColor = true;
            this.btnRemoveXmlPrimaryKey.Click += new System.EventHandler(this.BtnRemoveXmlPrimaryKey_Click);
            // 
            // pnlXmlDynamicColumns
            // 
            this.pnlXmlDynamicColumns.AutoScroll = true;
            this.pnlXmlDynamicColumns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlXmlDynamicColumns.Location = new System.Drawing.Point(40, 430);
            this.pnlXmlDynamicColumns.Name = "pnlXmlDynamicColumns";
            this.pnlXmlDynamicColumns.Size = new System.Drawing.Size(970, 120);
            this.pnlXmlDynamicColumns.TabIndex = 13;
            // 
            // btnAnalyzeXmlFK
            // 
            this.btnAnalyzeXmlFK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnAnalyzeXmlFK.Location = new System.Drawing.Point(40, 565);
            this.btnAnalyzeXmlFK.Name = "btnAnalyzeXmlFK";
            this.btnAnalyzeXmlFK.Size = new System.Drawing.Size(150, 30);
            this.btnAnalyzeXmlFK.TabIndex = 14;
            this.btnAnalyzeXmlFK.Text = "Analyze FK";
            this.btnAnalyzeXmlFK.UseVisualStyleBackColor = true;
            this.btnAnalyzeXmlFK.Click += new System.EventHandler(this.BtnAnalyzeXmlFK_Click);
            // 
            // lblXmlFKStats
            // 
            this.lblXmlFKStats.AutoSize = true;
            this.lblXmlFKStats.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblXmlFKStats.Location = new System.Drawing.Point(200, 573);
            this.lblXmlFKStats.Name = "lblXmlFKStats";
            this.lblXmlFKStats.Size = new System.Drawing.Size(140, 13);
            this.lblXmlFKStats.TabIndex = 0;
            this.lblXmlFKStats.Text = "Klik \'Analyze FK\' for at starte";
            // 
            // lstXmlMissingValues
            // 
            this.lstXmlMissingValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstXmlMissingValues.Location = new System.Drawing.Point(40, 610);
            this.lstXmlMissingValues.Name = "lstXmlMissingValues";
            this.lstXmlMissingValues.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstXmlMissingValues.Size = new System.Drawing.Size(500, 199);
            this.lstXmlMissingValues.TabIndex = 15;
            this.lstXmlMissingValues.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LstXmlMissingValues_KeyDown);
            // 
            // btnExportXmlMissing
            // 
            this.btnExportXmlMissing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportXmlMissing.Enabled = false;
            this.btnExportXmlMissing.Location = new System.Drawing.Point(560, 610);
            this.btnExportXmlMissing.Name = "btnExportXmlMissing";
            this.btnExportXmlMissing.Size = new System.Drawing.Size(120, 25);
            this.btnExportXmlMissing.TabIndex = 16;
            this.btnExportXmlMissing.Text = "Export til fil";
            this.btnExportXmlMissing.UseVisualStyleBackColor = true;
            this.btnExportXmlMissing.Click += new System.EventHandler(this.BtnExportXmlMissing_Click);
            // 
            // btnCopyXmlSelected
            // 
            this.btnCopyXmlSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyXmlSelected.Enabled = false;
            this.btnCopyXmlSelected.Location = new System.Drawing.Point(560, 640);
            this.btnCopyXmlSelected.Name = "btnCopyXmlSelected";
            this.btnCopyXmlSelected.Size = new System.Drawing.Size(120, 25);
            this.btnCopyXmlSelected.TabIndex = 17;
            this.btnCopyXmlSelected.Text = "Kopiér markerede";
            this.btnCopyXmlSelected.UseVisualStyleBackColor = true;
            this.btnCopyXmlSelected.Click += new System.EventHandler(this.BtnCopyXmlSelected_Click);
            // 
            // lblIntegrityDesc
            // 
            this.lblIntegrityDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIntegrityDesc.AutoSize = true;
            this.lblIntegrityDesc.Location = new System.Drawing.Point(560, 675);
            this.lblIntegrityDesc.Name = "lblIntegrityDesc";
            this.lblIntegrityDesc.Size = new System.Drawing.Size(103, 13);
            this.lblIntegrityDesc.TabIndex = 0;
            this.lblIntegrityDesc.Text = "Integrity beskrivelse:";
            // 
            // txtIntegrityDesc
            // 
            this.txtIntegrityDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIntegrityDesc.Location = new System.Drawing.Point(560, 692);
            this.txtIntegrityDesc.Name = "txtIntegrityDesc";
            this.txtIntegrityDesc.Size = new System.Drawing.Size(600, 20);
            this.txtIntegrityDesc.TabIndex = 18;
            this.txtIntegrityDesc.Text = "Betydning ukendt. Rækken er tilføjet under aflevering til arkiv, for at sikre ref" +
    "erentiel integritet i databasen af hensyn til langtidsbevaring";
            // 
            // btnGenerateFixedXml
            // 
            this.btnGenerateFixedXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateFixedXml.BackColor = System.Drawing.Color.LightGreen;
            this.btnGenerateFixedXml.Enabled = false;
            this.btnGenerateFixedXml.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnGenerateFixedXml.Location = new System.Drawing.Point(560, 717);
            this.btnGenerateFixedXml.Name = "btnGenerateFixedXml";
            this.btnGenerateFixedXml.Size = new System.Drawing.Size(220, 35);
            this.btnGenerateFixedXml.TabIndex = 19;
            this.btnGenerateFixedXml.Text = "Generer Nye Rækker til Parent";
            this.btnGenerateFixedXml.UseVisualStyleBackColor = false;
            this.btnGenerateFixedXml.Click += new System.EventHandler(this.BtnGenerateFixedXml_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 825);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1156, 23);
            this.progressBar1.TabIndex = 20;
            this.progressBar1.Visible = false;
            // 
            // lblTableIndexInfo
            // 
            this.lblTableIndexInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Italic);
            this.lblTableIndexInfo.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblTableIndexInfo.Location = new System.Drawing.Point(130, 110);
            this.lblTableIndexInfo.Name = "lblTableIndexInfo";
            this.lblTableIndexInfo.Size = new System.Drawing.Size(800, 30);
            this.lblTableIndexInfo.TabIndex = 3;
            this.lblTableIndexInfo.Text = "💡 TableIndex giver bedre kolonnenavne, datatyper og beskrivelser.\nKan springes o" +
    "ver - programmet parser direkte fra XML.";
            // 
            // separator1
            // 
            this.separator1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.separator1.Location = new System.Drawing.Point(20, 155);
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(1120, 2);
            this.separator1.TabIndex = 4;
            // 
            // separator2
            // 
            this.separator2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.separator2.Location = new System.Drawing.Point(20, 300);
            this.separator2.Name = "separator2";
            this.separator2.Size = new System.Drawing.Size(1120, 2);
            this.separator2.TabIndex = 9;
            // 
            // lblParentXmlCol
            // 
            this.lblParentXmlCol.AutoSize = true;
            this.lblParentXmlCol.Location = new System.Drawing.Point(40, 335);
            this.lblParentXmlCol.Name = "lblParentXmlCol";
            this.lblParentXmlCol.Size = new System.Drawing.Size(79, 13);
            this.lblParentXmlCol.TabIndex = 10;
            this.lblParentXmlCol.Text = "Parent Column:";
            // 
            // lblChildXmlCol
            // 
            this.lblChildXmlCol.AutoSize = true;
            this.lblChildXmlCol.Location = new System.Drawing.Point(40, 365);
            this.lblChildXmlCol.Name = "lblChildXmlCol";
            this.lblChildXmlCol.Size = new System.Drawing.Size(71, 13);
            this.lblChildXmlCol.TabIndex = 11;
            this.lblChildXmlCol.Text = "Child Column:";
            // 
            // XMLFKRepairForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 860);
            this.Controls.Add(this.lblTableIndex);
            this.Controls.Add(this.txtTableIndex);
            this.Controls.Add(this.btnBrowseTableIndex);
            this.Controls.Add(this.lblTableIndexInfo);
            this.Controls.Add(this.separator1);
            this.Controls.Add(this.lblParentTable);
            this.Controls.Add(this.cmbParentTable);
            this.Controls.Add(this.lblParentXml);
            this.Controls.Add(this.txtParentXml);
            this.Controls.Add(this.btnBrowseParentXml);
            this.Controls.Add(this.lblChildTable);
            this.Controls.Add(this.cmbChildTable);
            this.Controls.Add(this.lblChildXml);
            this.Controls.Add(this.txtChildXml);
            this.Controls.Add(this.btnBrowseChildXml);
            this.Controls.Add(this.separator2);
            this.Controls.Add(this.lblXmlMapping);
            this.Controls.Add(this.lblParentXmlCol);
            this.Controls.Add(this.cmbParentXmlColumns);
            this.Controls.Add(this.lblChildXmlCol);
            this.Controls.Add(this.cmbChildXmlColumns);
            this.Controls.Add(this.lblXmlCompositeKey);
            this.Controls.Add(this.btnAddXmlPrimaryKey);
            this.Controls.Add(this.btnRemoveXmlPrimaryKey);
            this.Controls.Add(this.pnlXmlDynamicColumns);
            this.Controls.Add(this.btnAnalyzeXmlFK);
            this.Controls.Add(this.lblXmlFKStats);
            this.Controls.Add(this.lstXmlMissingValues);
            this.Controls.Add(this.btnExportXmlMissing);
            this.Controls.Add(this.btnCopyXmlSelected);
            this.Controls.Add(this.lblIntegrityDesc);
            this.Controls.Add(this.txtIntegrityDesc);
            this.Controls.Add(this.btnGenerateFixedXml);
            this.Controls.Add(this.progressBar1);
            this.MinimumSize = new System.Drawing.Size(1196, 899);
            this.Name = "XMLFKRepairForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Foreign Key Repair - XML (TableIndex)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Control declarations
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
        private Label separator1;
        private Label separator2;
        private Label lblParentXmlCol;
        private Label lblChildXmlCol;
    }
}