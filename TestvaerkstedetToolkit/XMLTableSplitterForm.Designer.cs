namespace TestvaerkstedetToolkit
{
    partial class XMLTableSplitterForm
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
            groupBoxTableIndex = new System.Windows.Forms.GroupBox();
            btnBrowseTableIndex = new System.Windows.Forms.Button();
            txtTableIndexPath = new System.Windows.Forms.TextBox();
            lblTableIndexPath = new System.Windows.Forms.Label();
            cmbTableSelector = new System.Windows.Forms.ComboBox();
            lblTableSelector = new System.Windows.Forms.Label();
            lblTableInfo = new System.Windows.Forms.Label();
            groupBoxFileSelection = new System.Windows.Forms.GroupBox();
            lblSourceXML = new System.Windows.Forms.Label();
            txtSourceXML = new System.Windows.Forms.TextBox();
            groupBoxPrimaryKey = new System.Windows.Forms.GroupBox();
            compositePKSelector = new TestvaerkstedetToolkit.Controls.CompositePKSelector();
            btnAnalyzePK = new System.Windows.Forms.Button();
            groupBoxSplitConfiguration = new System.Windows.Forms.GroupBox();
            txtSplitPoints = new System.Windows.Forms.TextBox();
            lblSplitPoints = new System.Windows.Forms.Label();
            btnCalculateSplit = new System.Windows.Forms.Button();
            groupBoxSplitPreview = new System.Windows.Forms.GroupBox();
            lstSplitPreview = new System.Windows.Forms.ListBox();
            lblPreviewInfo = new System.Windows.Forms.Label();
            groupBoxExecution = new System.Windows.Forms.GroupBox();
            btnExecuteSplit = new System.Windows.Forms.Button();
            progressBar = new System.Windows.Forms.ProgressBar();
            lblOutputPath = new System.Windows.Forms.Label();
            btnChangeOutput = new System.Windows.Forms.Button();
            groupBoxTableIndex.SuspendLayout();
            groupBoxFileSelection.SuspendLayout();
            groupBoxPrimaryKey.SuspendLayout();
            groupBoxSplitConfiguration.SuspendLayout();
            groupBoxSplitPreview.SuspendLayout();
            groupBoxExecution.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxTableIndex
            // 
            groupBoxTableIndex.Controls.Add(btnBrowseTableIndex);
            groupBoxTableIndex.Controls.Add(txtTableIndexPath);
            groupBoxTableIndex.Controls.Add(lblTableIndexPath);
            groupBoxTableIndex.Controls.Add(cmbTableSelector);
            groupBoxTableIndex.Controls.Add(lblTableSelector);
            groupBoxTableIndex.Controls.Add(lblTableInfo);
            groupBoxTableIndex.Location = new System.Drawing.Point(19, 52);
            groupBoxTableIndex.Margin = new System.Windows.Forms.Padding(2);
            groupBoxTableIndex.Name = "groupBoxTableIndex";
            groupBoxTableIndex.Padding = new System.Windows.Forms.Padding(2);
            groupBoxTableIndex.Size = new System.Drawing.Size(848, 84);
            groupBoxTableIndex.TabIndex = 0;
            groupBoxTableIndex.TabStop = false;
            groupBoxTableIndex.Text = "1. TableIndex Workflow (Hovedindgang)";
            // 
            // btnBrowseTableIndex
            // 
            btnBrowseTableIndex.Location = new System.Drawing.Point(758, 20);
            btnBrowseTableIndex.Margin = new System.Windows.Forms.Padding(2);
            btnBrowseTableIndex.Name = "btnBrowseTableIndex";
            btnBrowseTableIndex.Size = new System.Drawing.Size(75, 23);
            btnBrowseTableIndex.TabIndex = 1;
            btnBrowseTableIndex.Text = "Browse...";
            btnBrowseTableIndex.UseVisualStyleBackColor = true;
            btnBrowseTableIndex.Click += btnBrowseTableIndex_Click;
            // 
            // txtTableIndexPath
            // 
            txtTableIndexPath.Location = new System.Drawing.Point(112, 20);
            txtTableIndexPath.Margin = new System.Windows.Forms.Padding(2);
            txtTableIndexPath.Name = "txtTableIndexPath";
            txtTableIndexPath.ReadOnly = true;
            txtTableIndexPath.Size = new System.Drawing.Size(638, 23);
            txtTableIndexPath.TabIndex = 0;
            // 
            // lblTableIndexPath
            // 
            lblTableIndexPath.AutoSize = true;
            lblTableIndexPath.Location = new System.Drawing.Point(15, 21);
            lblTableIndexPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblTableIndexPath.Name = "lblTableIndexPath";
            lblTableIndexPath.Size = new System.Drawing.Size(103, 15);
            lblTableIndexPath.TabIndex = 0;
            lblTableIndexPath.Text = "TableIndex.xml sti:";
            // 
            // cmbTableSelector
            // 
            cmbTableSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbTableSelector.Enabled = false;
            cmbTableSelector.Location = new System.Drawing.Point(112, 46);
            cmbTableSelector.Margin = new System.Windows.Forms.Padding(2);
            cmbTableSelector.Name = "cmbTableSelector";
            cmbTableSelector.Size = new System.Drawing.Size(638, 23);
            cmbTableSelector.TabIndex = 2;
            cmbTableSelector.SelectedIndexChanged += cmbTableSelector_SelectedIndexChanged;
            // 
            // lblTableSelector
            // 
            lblTableSelector.AutoSize = true;
            lblTableSelector.Location = new System.Drawing.Point(15, 47);
            lblTableSelector.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblTableSelector.Name = "lblTableSelector";
            lblTableSelector.Size = new System.Drawing.Size(65, 15);
            lblTableSelector.TabIndex = 0;
            lblTableSelector.Text = "Vælg tabel:";
            // 
            // lblTableInfo
            // 
            lblTableInfo.AutoSize = true;
            lblTableInfo.ForeColor = System.Drawing.Color.DarkBlue;
            lblTableInfo.Location = new System.Drawing.Point(15, 68);
            lblTableInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblTableInfo.Name = "lblTableInfo";
            lblTableInfo.Size = new System.Drawing.Size(184, 15);
            lblTableInfo.TabIndex = 0;
            lblTableInfo.Text = "Vælg tableIndex.xml for at starte...";
            // 
            // groupBoxFileSelection
            // 
            groupBoxFileSelection.Controls.Add(lblSourceXML);
            groupBoxFileSelection.Controls.Add(txtSourceXML);
            groupBoxFileSelection.Location = new System.Drawing.Point(19, 146);
            groupBoxFileSelection.Margin = new System.Windows.Forms.Padding(2);
            groupBoxFileSelection.Name = "groupBoxFileSelection";
            groupBoxFileSelection.Padding = new System.Windows.Forms.Padding(2);
            groupBoxFileSelection.Size = new System.Drawing.Size(848, 46);
            groupBoxFileSelection.TabIndex = 1;
            groupBoxFileSelection.TabStop = false;
            groupBoxFileSelection.Text = "2. XML Fil (Auto-foreslået fra TableIndex)";
            // 
            // lblSourceXML
            // 
            lblSourceXML.AutoSize = true;
            lblSourceXML.Location = new System.Drawing.Point(11, 18);
            lblSourceXML.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblSourceXML.Name = "lblSourceXML";
            lblSourceXML.Size = new System.Drawing.Size(63, 15);
            lblSourceXML.TabIndex = 0;
            lblSourceXML.Text = "Kilde XML:";
            // 
            // txtSourceXML
            // 
            txtSourceXML.BackColor = System.Drawing.SystemColors.Control;
            txtSourceXML.Location = new System.Drawing.Point(79, 16);
            txtSourceXML.Margin = new System.Windows.Forms.Padding(2);
            txtSourceXML.Name = "txtSourceXML";
            txtSourceXML.ReadOnly = true;
            txtSourceXML.Size = new System.Drawing.Size(754, 23);
            txtSourceXML.TabIndex = 1;
            // 
            // groupBoxPrimaryKey
            // 
            groupBoxPrimaryKey.Controls.Add(compositePKSelector);
            groupBoxPrimaryKey.Controls.Add(btnAnalyzePK);
            groupBoxPrimaryKey.Location = new System.Drawing.Point(19, 202);
            groupBoxPrimaryKey.Margin = new System.Windows.Forms.Padding(2);
            groupBoxPrimaryKey.Name = "groupBoxPrimaryKey";
            groupBoxPrimaryKey.Padding = new System.Windows.Forms.Padding(2);
            groupBoxPrimaryKey.Size = new System.Drawing.Size(848, 289);
            groupBoxPrimaryKey.TabIndex = 2;
            groupBoxPrimaryKey.TabStop = false;
            groupBoxPrimaryKey.Text = "3. Primærnøgle konfiguration";
            // 
            // compositePKSelector
            // 
            compositePKSelector.BackColor = System.Drawing.SystemColors.Control;
            compositePKSelector.Location = new System.Drawing.Point(15, 20);
            compositePKSelector.Margin = new System.Windows.Forms.Padding(2);
            compositePKSelector.Name = "compositePKSelector";
            compositePKSelector.Size = new System.Drawing.Size(686, 265);
            compositePKSelector.TabIndex = 0;
            compositePKSelector.PrimaryKeyChanged += CompositePKSelector_PrimaryKeyChanged;
            compositePKSelector.Load += compositePKSelector_Load;
            // 
            // btnAnalyzePK
            // 
            btnAnalyzePK.Enabled = false;
            btnAnalyzePK.Location = new System.Drawing.Point(705, 20);
            btnAnalyzePK.Margin = new System.Windows.Forms.Padding(2);
            btnAnalyzePK.Name = "btnAnalyzePK";
            btnAnalyzePK.Size = new System.Drawing.Size(128, 23);
            btnAnalyzePK.TabIndex = 1;
            btnAnalyzePK.Text = "🔍 Analysér PK Unikhed";
            btnAnalyzePK.UseVisualStyleBackColor = true;
            btnAnalyzePK.Click += btnAnalyzePK_Click;
            // 
            // groupBoxSplitConfiguration
            // 
            groupBoxSplitConfiguration.Controls.Add(txtSplitPoints);
            groupBoxSplitConfiguration.Controls.Add(lblSplitPoints);
            groupBoxSplitConfiguration.Controls.Add(btnCalculateSplit);
            groupBoxSplitConfiguration.Location = new System.Drawing.Point(19, 495);
            groupBoxSplitConfiguration.Margin = new System.Windows.Forms.Padding(2);
            groupBoxSplitConfiguration.Name = "groupBoxSplitConfiguration";
            groupBoxSplitConfiguration.Padding = new System.Windows.Forms.Padding(2);
            groupBoxSplitConfiguration.Size = new System.Drawing.Size(848, 105);
            groupBoxSplitConfiguration.TabIndex = 3;
            groupBoxSplitConfiguration.TabStop = false;
            groupBoxSplitConfiguration.Text = "4. Split Konfiguration";
            // 
            // txtSplitPoints
            // 
            txtSplitPoints.Enabled = false;
            txtSplitPoints.Font = new System.Drawing.Font("Segoe UI", 9F);
            txtSplitPoints.Location = new System.Drawing.Point(14, 38);
            txtSplitPoints.Margin = new System.Windows.Forms.Padding(2);
            txtSplitPoints.Multiline = true;
            txtSplitPoints.Name = "txtSplitPoints";
            txtSplitPoints.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtSplitPoints.Size = new System.Drawing.Size(676, 40);
            txtSplitPoints.TabIndex = 0;
            txtSplitPoints.Text = "Eksempel: 9, 18 (split efter kolonne 9 og 18)\nLad stå tom for auto-split baseret på PK kapacitet";
            // 
            // lblSplitPoints
            // 
            lblSplitPoints.AutoSize = true;
            lblSplitPoints.Location = new System.Drawing.Point(15, 20);
            lblSplitPoints.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblSplitPoints.Name = "lblSplitPoints";
            lblSplitPoints.Size = new System.Drawing.Size(177, 15);
            lblSplitPoints.TabIndex = 0;
            lblSplitPoints.Text = "Split punkter (kommasepareret):";
            // 
            // btnCalculateSplit
            // 
            btnCalculateSplit.Enabled = false;
            btnCalculateSplit.Location = new System.Drawing.Point(705, 32);
            btnCalculateSplit.Margin = new System.Windows.Forms.Padding(2);
            btnCalculateSplit.Name = "btnCalculateSplit";
            btnCalculateSplit.Size = new System.Drawing.Size(128, 23);
            btnCalculateSplit.TabIndex = 1;
            btnCalculateSplit.Text = "Beregn Split";
            btnCalculateSplit.UseVisualStyleBackColor = true;
            btnCalculateSplit.Click += btnCalculateSplit_Click;
            // 
            // groupBoxSplitPreview
            // 
            groupBoxSplitPreview.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxSplitPreview.Controls.Add(lstSplitPreview);
            groupBoxSplitPreview.Controls.Add(lblPreviewInfo);
            groupBoxSplitPreview.Location = new System.Drawing.Point(19, 604);
            groupBoxSplitPreview.Margin = new System.Windows.Forms.Padding(2);
            groupBoxSplitPreview.Name = "groupBoxSplitPreview";
            groupBoxSplitPreview.Padding = new System.Windows.Forms.Padding(2);
            groupBoxSplitPreview.Size = new System.Drawing.Size(848, 166);
            groupBoxSplitPreview.TabIndex = 4;
            groupBoxSplitPreview.TabStop = false;
            groupBoxSplitPreview.Text = "5. Split Preview";
            // 
            // lstSplitPreview
            // 
            lstSplitPreview.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lstSplitPreview.Font = new System.Drawing.Font("Courier New", 9F);
            lstSplitPreview.ItemHeight = 15;
            lstSplitPreview.Location = new System.Drawing.Point(14, 56);
            lstSplitPreview.Margin = new System.Windows.Forms.Padding(2);
            lstSplitPreview.Name = "lstSplitPreview";
            lstSplitPreview.Size = new System.Drawing.Size(819, 94);
            lstSplitPreview.TabIndex = 0;
            // 
            // lblPreviewInfo
            // 
            lblPreviewInfo.AutoSize = true;
            lblPreviewInfo.ForeColor = System.Drawing.Color.DarkBlue;
            lblPreviewInfo.Location = new System.Drawing.Point(15, 26);
            lblPreviewInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            lblPreviewInfo.Name = "lblPreviewInfo";
            lblPreviewInfo.Size = new System.Drawing.Size(124, 15);
            lblPreviewInfo.TabIndex = 0;
            lblPreviewInfo.Text = "Konfigurer split først...";
            // 
            // groupBoxExecution
            // 
            groupBoxExecution.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBoxExecution.Controls.Add(btnExecuteSplit);
            groupBoxExecution.Controls.Add(progressBar);
            groupBoxExecution.Location = new System.Drawing.Point(19, 780);
            groupBoxExecution.Margin = new System.Windows.Forms.Padding(2);
            groupBoxExecution.Name = "groupBoxExecution";
            groupBoxExecution.Padding = new System.Windows.Forms.Padding(2);
            groupBoxExecution.Size = new System.Drawing.Size(848, 52);
            groupBoxExecution.TabIndex = 5;
            groupBoxExecution.TabStop = false;
            groupBoxExecution.Text = "6. Split Operation";
            // 
            // btnExecuteSplit
            // 
            btnExecuteSplit.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            btnExecuteSplit.Enabled = false;
            btnExecuteSplit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnExecuteSplit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnExecuteSplit.ForeColor = System.Drawing.Color.White;
            btnExecuteSplit.Location = new System.Drawing.Point(15, 16);
            btnExecuteSplit.Margin = new System.Windows.Forms.Padding(2);
            btnExecuteSplit.Name = "btnExecuteSplit";
            btnExecuteSplit.Size = new System.Drawing.Size(165, 26);
            btnExecuteSplit.TabIndex = 0;
            btnExecuteSplit.Text = "Udfør Split";
            btnExecuteSplit.UseVisualStyleBackColor = false;
            btnExecuteSplit.Click += btnExecuteSplit_Click;
            // 
            // progressBar
            // 
            progressBar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressBar.Location = new System.Drawing.Point(195, 16);
            progressBar.Margin = new System.Windows.Forms.Padding(2);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(638, 26);
            progressBar.TabIndex = 1;
            progressBar.Visible = false;
            // 
            // lblOutputPath
            // 
            lblOutputPath.AutoSize = true;
            lblOutputPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblOutputPath.ForeColor = System.Drawing.Color.DarkGreen;
            lblOutputPath.Location = new System.Drawing.Point(19, 15);
            lblOutputPath.Name = "lblOutputPath";
            lblOutputPath.Size = new System.Drawing.Size(200, 13);
            lblOutputPath.TabIndex = 100;
            lblOutputPath.Text = "Output destination: Desktop/XML_Table_Splits";
            // 
            // btnChangeOutput
            // 
            btnChangeOutput.Location = new System.Drawing.Point(750, 10);
            btnChangeOutput.Name = "btnChangeOutput";
            btnChangeOutput.Size = new System.Drawing.Size(110, 25);
            btnChangeOutput.TabIndex = 101;
            btnChangeOutput.Text = "📁 Vælg Output";
            btnChangeOutput.UseVisualStyleBackColor = true;
            btnChangeOutput.Click += btnChangeOutput_Click;
            // 
            // XMLTableSplitterForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoScroll = true;
            ClientSize = new System.Drawing.Size(900, 848);
            Controls.Add(groupBoxTableIndex);
            Controls.Add(groupBoxFileSelection);
            Controls.Add(groupBoxPrimaryKey);
            Controls.Add(groupBoxSplitConfiguration);
            Controls.Add(groupBoxSplitPreview);
            Controls.Add(groupBoxExecution);
            Margin = new System.Windows.Forms.Padding(2);
            MinimumSize = new System.Drawing.Size(904, 534);
            Name = "XMLTableSplitterForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "XML Table Splitter";
            groupBoxTableIndex.ResumeLayout(false);
            groupBoxTableIndex.PerformLayout();
            groupBoxFileSelection.ResumeLayout(false);
            groupBoxFileSelection.PerformLayout();
            groupBoxPrimaryKey.ResumeLayout(false);
            groupBoxSplitConfiguration.ResumeLayout(false);
            groupBoxSplitConfiguration.PerformLayout();
            groupBoxSplitPreview.ResumeLayout(false);
            groupBoxSplitPreview.PerformLayout();
            groupBoxExecution.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        // Controls
        #region Output Controls
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.Button btnChangeOutput;
        #endregion

        #region TableIndex Integration Controls
        private System.Windows.Forms.GroupBox groupBoxTableIndex;
        private System.Windows.Forms.Button btnBrowseTableIndex;
        private System.Windows.Forms.TextBox txtTableIndexPath;
        private System.Windows.Forms.Label lblTableIndexPath;
        private System.Windows.Forms.ComboBox cmbTableSelector;
        private System.Windows.Forms.Label lblTableSelector;
        private System.Windows.Forms.Label lblTableInfo;
        #endregion

        #region File Selection Controls
        private System.Windows.Forms.GroupBox groupBoxFileSelection;
        private System.Windows.Forms.TextBox txtSourceXML;
        private System.Windows.Forms.Label lblSourceXML;
        #endregion

        #region Primary Key Controls
        private System.Windows.Forms.GroupBox groupBoxPrimaryKey;
        private TestvaerkstedetToolkit.Controls.CompositePKSelector compositePKSelector;
        private System.Windows.Forms.Button btnAnalyzePK;
        #endregion

        #region Split Configuration Controls
        private System.Windows.Forms.GroupBox groupBoxSplitConfiguration;
        private System.Windows.Forms.TextBox txtSplitPoints;
        private System.Windows.Forms.Label lblSplitPoints;
        private System.Windows.Forms.Button btnCalculateSplit;
        #endregion

        #region Split Preview Controls
        private System.Windows.Forms.GroupBox groupBoxSplitPreview;
        private System.Windows.Forms.ListBox lstSplitPreview;
        private System.Windows.Forms.Label lblPreviewInfo;
        #endregion

        #region Execution Controls
        private System.Windows.Forms.GroupBox groupBoxExecution;
        private System.Windows.Forms.Button btnExecuteSplit;
        private System.Windows.Forms.ProgressBar progressBar;
        #endregion
    }
}