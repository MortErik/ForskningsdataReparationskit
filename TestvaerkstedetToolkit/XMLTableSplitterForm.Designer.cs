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
            this.groupBoxTableIndex = new System.Windows.Forms.GroupBox();
            this.btnBrowseTableIndex = new System.Windows.Forms.Button();
            this.txtTableIndexPath = new System.Windows.Forms.TextBox();
            this.lblTableIndexPath = new System.Windows.Forms.Label();
            this.cmbTableSelector = new System.Windows.Forms.ComboBox();
            this.lblTableSelector = new System.Windows.Forms.Label();
            this.lblTableInfo = new System.Windows.Forms.Label();
            this.groupBoxFileSelection = new System.Windows.Forms.GroupBox();
            this.lblSourceXML = new System.Windows.Forms.Label();
            this.txtSourceXML = new System.Windows.Forms.TextBox();
            this.groupBoxPrimaryKey = new System.Windows.Forms.GroupBox();
            this.compositePKSelector = new TestvaerkstedetToolkit.Controls.CompositePKSelector();
            this.btnAnalyzePK = new System.Windows.Forms.Button();
            this.groupBoxSplitConfiguration = new System.Windows.Forms.GroupBox();
            this.txtSplitPoints = new System.Windows.Forms.TextBox();
            this.lblSplitPoints = new System.Windows.Forms.Label();
            this.btnCalculateSplit = new System.Windows.Forms.Button();
            this.groupBoxSplitPreview = new System.Windows.Forms.GroupBox();
            this.lstSplitPreview = new System.Windows.Forms.ListBox();
            this.lblPreviewInfo = new System.Windows.Forms.Label();
            this.groupBoxExecution = new System.Windows.Forms.GroupBox();
            this.btnExecuteSplit = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.btnChangeOutput = new System.Windows.Forms.Button();
            this.groupBoxTableIndex.SuspendLayout();
            this.groupBoxFileSelection.SuspendLayout();
            this.groupBoxPrimaryKey.SuspendLayout();
            this.groupBoxSplitConfiguration.SuspendLayout();
            this.groupBoxSplitPreview.SuspendLayout();
            this.groupBoxExecution.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxTableIndex
            // 
            this.groupBoxTableIndex.Controls.Add(this.btnBrowseTableIndex);
            this.groupBoxTableIndex.Controls.Add(this.txtTableIndexPath);
            this.groupBoxTableIndex.Controls.Add(this.lblTableIndexPath);
            this.groupBoxTableIndex.Controls.Add(this.cmbTableSelector);
            this.groupBoxTableIndex.Controls.Add(this.lblTableSelector);
            this.groupBoxTableIndex.Controls.Add(this.lblTableInfo);
            this.groupBoxTableIndex.Location = new System.Drawing.Point(19, 52);
            this.groupBoxTableIndex.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxTableIndex.Name = "groupBoxTableIndex";
            this.groupBoxTableIndex.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxTableIndex.Size = new System.Drawing.Size(848, 84);
            this.groupBoxTableIndex.TabIndex = 0;
            this.groupBoxTableIndex.TabStop = false;
            this.groupBoxTableIndex.Text = "1. TableIndex Workflow (Hovedindgang)";
            // 
            // btnBrowseTableIndex
            // 
            this.btnBrowseTableIndex.Location = new System.Drawing.Point(758, 20);
            this.btnBrowseTableIndex.Margin = new System.Windows.Forms.Padding(2);
            this.btnBrowseTableIndex.Name = "btnBrowseTableIndex";
            this.btnBrowseTableIndex.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseTableIndex.TabIndex = 1;
            this.btnBrowseTableIndex.Text = "Browse...";
            this.btnBrowseTableIndex.UseVisualStyleBackColor = true;
            this.btnBrowseTableIndex.Click += new System.EventHandler(this.btnBrowseTableIndex_Click);
            // 
            // txtTableIndexPath
            // 
            this.txtTableIndexPath.Location = new System.Drawing.Point(112, 20);
            this.txtTableIndexPath.Margin = new System.Windows.Forms.Padding(2);
            this.txtTableIndexPath.Name = "txtTableIndexPath";
            this.txtTableIndexPath.ReadOnly = true;
            this.txtTableIndexPath.Size = new System.Drawing.Size(638, 20);
            this.txtTableIndexPath.TabIndex = 0;
            // 
            // lblTableIndexPath
            // 
            this.lblTableIndexPath.AutoSize = true;
            this.lblTableIndexPath.Location = new System.Drawing.Point(15, 21);
            this.lblTableIndexPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTableIndexPath.Name = "lblTableIndexPath";
            this.lblTableIndexPath.Size = new System.Drawing.Size(94, 13);
            this.lblTableIndexPath.TabIndex = 0;
            this.lblTableIndexPath.Text = "TableIndex.xml sti:";
            // 
            // cmbTableSelector
            // 
            this.cmbTableSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTableSelector.Enabled = false;
            this.cmbTableSelector.Location = new System.Drawing.Point(112, 46);
            this.cmbTableSelector.Margin = new System.Windows.Forms.Padding(2);
            this.cmbTableSelector.Name = "cmbTableSelector";
            this.cmbTableSelector.Size = new System.Drawing.Size(638, 21);
            this.cmbTableSelector.TabIndex = 2;
            this.cmbTableSelector.SelectedIndexChanged += new System.EventHandler(this.cmbTableSelector_SelectedIndexChanged);
            // 
            // lblTableSelector
            // 
            this.lblTableSelector.AutoSize = true;
            this.lblTableSelector.Location = new System.Drawing.Point(15, 47);
            this.lblTableSelector.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTableSelector.Name = "lblTableSelector";
            this.lblTableSelector.Size = new System.Drawing.Size(61, 13);
            this.lblTableSelector.TabIndex = 0;
            this.lblTableSelector.Text = "Vælg tabel:";
            // 
            // lblTableInfo
            // 
            this.lblTableInfo.AutoSize = true;
            this.lblTableInfo.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTableInfo.Location = new System.Drawing.Point(15, 68);
            this.lblTableInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTableInfo.Name = "lblTableInfo";
            this.lblTableInfo.Size = new System.Drawing.Size(167, 13);
            this.lblTableInfo.TabIndex = 0;
            this.lblTableInfo.Text = "Vælg tableIndex.xml for at starte...";
            // 
            // groupBoxFileSelection
            // 
            this.groupBoxFileSelection.Controls.Add(this.lblSourceXML);
            this.groupBoxFileSelection.Controls.Add(this.txtSourceXML);
            this.groupBoxFileSelection.Location = new System.Drawing.Point(19, 146);
            this.groupBoxFileSelection.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxFileSelection.Name = "groupBoxFileSelection";
            this.groupBoxFileSelection.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxFileSelection.Size = new System.Drawing.Size(848, 46);
            this.groupBoxFileSelection.TabIndex = 1;
            this.groupBoxFileSelection.TabStop = false;
            this.groupBoxFileSelection.Text = "2. XML Fil (Auto-foreslået fra TableIndex)";
            // 
            // lblSourceXML
            // 
            this.lblSourceXML.AutoSize = true;
            this.lblSourceXML.Location = new System.Drawing.Point(11, 18);
            this.lblSourceXML.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSourceXML.Name = "lblSourceXML";
            this.lblSourceXML.Size = new System.Drawing.Size(58, 13);
            this.lblSourceXML.TabIndex = 0;
            this.lblSourceXML.Text = "Kilde XML:";
            // 
            // txtSourceXML
            // 
            this.txtSourceXML.BackColor = System.Drawing.SystemColors.Control;
            this.txtSourceXML.Location = new System.Drawing.Point(79, 16);
            this.txtSourceXML.Margin = new System.Windows.Forms.Padding(2);
            this.txtSourceXML.Name = "txtSourceXML";
            this.txtSourceXML.ReadOnly = true;
            this.txtSourceXML.Size = new System.Drawing.Size(754, 20);
            this.txtSourceXML.TabIndex = 1;
            // 
            // groupBoxPrimaryKey
            // 
            this.groupBoxPrimaryKey.Controls.Add(this.compositePKSelector);
            this.groupBoxPrimaryKey.Controls.Add(this.btnAnalyzePK);
            this.groupBoxPrimaryKey.Location = new System.Drawing.Point(19, 202);
            this.groupBoxPrimaryKey.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxPrimaryKey.Name = "groupBoxPrimaryKey";
            this.groupBoxPrimaryKey.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxPrimaryKey.Size = new System.Drawing.Size(848, 289);
            this.groupBoxPrimaryKey.TabIndex = 2;
            this.groupBoxPrimaryKey.TabStop = false;
            this.groupBoxPrimaryKey.Text = "3. Primærnøgle konfiguration";
            // 
            // compositePKSelector
            // 
            this.compositePKSelector.BackColor = System.Drawing.SystemColors.Control;
            this.compositePKSelector.Location = new System.Drawing.Point(15, 20);
            this.compositePKSelector.Margin = new System.Windows.Forms.Padding(2);
            this.compositePKSelector.Name = "compositePKSelector";
            this.compositePKSelector.Size = new System.Drawing.Size(686, 265);
            this.compositePKSelector.TabIndex = 0;
            this.compositePKSelector.PrimaryKeyChanged += new System.EventHandler(this.CompositePKSelector_PrimaryKeyChanged);
            this.compositePKSelector.Load += new System.EventHandler(this.compositePKSelector_Load);
            // 
            // btnAnalyzePK
            // 
            this.btnAnalyzePK.Enabled = false;
            this.btnAnalyzePK.Location = new System.Drawing.Point(705, 20);
            this.btnAnalyzePK.Margin = new System.Windows.Forms.Padding(2);
            this.btnAnalyzePK.Name = "btnAnalyzePK";
            this.btnAnalyzePK.Size = new System.Drawing.Size(128, 23);
            this.btnAnalyzePK.TabIndex = 1;
            this.btnAnalyzePK.Text = "🔍 Analysér PK Unikhed";
            this.btnAnalyzePK.UseVisualStyleBackColor = true;
            this.btnAnalyzePK.Click += new System.EventHandler(this.btnAnalyzePK_Click);
            // 
            // groupBoxSplitConfiguration
            // 
            this.groupBoxSplitConfiguration.Controls.Add(this.txtSplitPoints);
            this.groupBoxSplitConfiguration.Controls.Add(this.lblSplitPoints);
            this.groupBoxSplitConfiguration.Controls.Add(this.btnCalculateSplit);
            this.groupBoxSplitConfiguration.Location = new System.Drawing.Point(19, 495);
            this.groupBoxSplitConfiguration.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxSplitConfiguration.Name = "groupBoxSplitConfiguration";
            this.groupBoxSplitConfiguration.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxSplitConfiguration.Size = new System.Drawing.Size(848, 105);
            this.groupBoxSplitConfiguration.TabIndex = 3;
            this.groupBoxSplitConfiguration.TabStop = false;
            this.groupBoxSplitConfiguration.Text = "4. Split Konfiguration";
            // 
            // txtSplitPoints
            // 
            this.txtSplitPoints.Enabled = false;
            this.txtSplitPoints.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSplitPoints.Location = new System.Drawing.Point(14, 38);
            this.txtSplitPoints.Margin = new System.Windows.Forms.Padding(2);
            this.txtSplitPoints.Multiline = true;
            this.txtSplitPoints.Name = "txtSplitPoints";
            this.txtSplitPoints.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSplitPoints.Size = new System.Drawing.Size(676, 40);
            this.txtSplitPoints.TabIndex = 0;
            this.txtSplitPoints.Text = "Eksempel: 9, 18 (split efter kolonne 9 og 18)\nLad stå tom for auto-split baseret " +
    "på PK kapacitet";
            // 
            // lblSplitPoints
            // 
            this.lblSplitPoints.AutoSize = true;
            this.lblSplitPoints.Location = new System.Drawing.Point(15, 20);
            this.lblSplitPoints.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSplitPoints.Name = "lblSplitPoints";
            this.lblSplitPoints.Size = new System.Drawing.Size(156, 13);
            this.lblSplitPoints.TabIndex = 0;
            this.lblSplitPoints.Text = "Split punkter (kommasepareret):";
            // 
            // btnCalculateSplit
            // 
            this.btnCalculateSplit.Enabled = false;
            this.btnCalculateSplit.Location = new System.Drawing.Point(705, 32);
            this.btnCalculateSplit.Margin = new System.Windows.Forms.Padding(2);
            this.btnCalculateSplit.Name = "btnCalculateSplit";
            this.btnCalculateSplit.Size = new System.Drawing.Size(128, 23);
            this.btnCalculateSplit.TabIndex = 1;
            this.btnCalculateSplit.Text = "Beregn Split";
            this.btnCalculateSplit.UseVisualStyleBackColor = true;
            this.btnCalculateSplit.Click += new System.EventHandler(this.btnCalculateSplit_Click);
            // 
            // groupBoxSplitPreview
            // 
            this.groupBoxSplitPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSplitPreview.Controls.Add(this.lstSplitPreview);
            this.groupBoxSplitPreview.Controls.Add(this.lblPreviewInfo);
            this.groupBoxSplitPreview.Location = new System.Drawing.Point(19, 604);
            this.groupBoxSplitPreview.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxSplitPreview.Name = "groupBoxSplitPreview";
            this.groupBoxSplitPreview.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxSplitPreview.Size = new System.Drawing.Size(848, 134);
            this.groupBoxSplitPreview.TabIndex = 4;
            this.groupBoxSplitPreview.TabStop = false;
            this.groupBoxSplitPreview.Text = "5. Split Preview";
            // 
            // lstSplitPreview
            // 
            this.lstSplitPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSplitPreview.Font = new System.Drawing.Font("Courier New", 9F);
            this.lstSplitPreview.ItemHeight = 15;
            this.lstSplitPreview.Location = new System.Drawing.Point(14, 56);
            this.lstSplitPreview.Margin = new System.Windows.Forms.Padding(2);
            this.lstSplitPreview.Name = "lstSplitPreview";
            this.lstSplitPreview.Size = new System.Drawing.Size(819, 64);
            this.lstSplitPreview.TabIndex = 0;
            // 
            // lblPreviewInfo
            // 
            this.lblPreviewInfo.AutoSize = true;
            this.lblPreviewInfo.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblPreviewInfo.Location = new System.Drawing.Point(15, 26);
            this.lblPreviewInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPreviewInfo.Name = "lblPreviewInfo";
            this.lblPreviewInfo.Size = new System.Drawing.Size(108, 13);
            this.lblPreviewInfo.TabIndex = 0;
            this.lblPreviewInfo.Text = "Konfigurer split først...";
            // 
            // groupBoxExecution
            // 
            this.groupBoxExecution.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxExecution.Controls.Add(this.btnExecuteSplit);
            this.groupBoxExecution.Controls.Add(this.progressBar);
            this.groupBoxExecution.Location = new System.Drawing.Point(19, 748);
            this.groupBoxExecution.Margin = new System.Windows.Forms.Padding(2);
            this.groupBoxExecution.Name = "groupBoxExecution";
            this.groupBoxExecution.Padding = new System.Windows.Forms.Padding(2);
            this.groupBoxExecution.Size = new System.Drawing.Size(848, 52);
            this.groupBoxExecution.TabIndex = 5;
            this.groupBoxExecution.TabStop = false;
            this.groupBoxExecution.Text = "6. Split Operation";
            // 
            // btnExecuteSplit
            // 
            this.btnExecuteSplit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnExecuteSplit.Enabled = false;
            this.btnExecuteSplit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExecuteSplit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExecuteSplit.ForeColor = System.Drawing.Color.White;
            this.btnExecuteSplit.Location = new System.Drawing.Point(15, 16);
            this.btnExecuteSplit.Margin = new System.Windows.Forms.Padding(2);
            this.btnExecuteSplit.Name = "btnExecuteSplit";
            this.btnExecuteSplit.Size = new System.Drawing.Size(165, 26);
            this.btnExecuteSplit.TabIndex = 0;
            this.btnExecuteSplit.Text = "Udfør Split";
            this.btnExecuteSplit.UseVisualStyleBackColor = false;
            this.btnExecuteSplit.Click += new System.EventHandler(this.btnExecuteSplit_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(195, 16);
            this.progressBar.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(638, 26);
            this.progressBar.TabIndex = 1;
            this.progressBar.Visible = false;
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.AutoSize = true;
            this.lblOutputPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutputPath.ForeColor = System.Drawing.Color.DarkGreen;
            this.lblOutputPath.Location = new System.Drawing.Point(19, 15);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(200, 13);
            this.lblOutputPath.TabIndex = 100;
            this.lblOutputPath.Text = "Output destination: Desktop/XML_Table_Splits";
            // 
            // btnChangeOutput
            // 
            this.btnChangeOutput.Location = new System.Drawing.Point(750, 10);
            this.btnChangeOutput.Name = "btnChangeOutput";
            this.btnChangeOutput.Size = new System.Drawing.Size(110, 25);
            this.btnChangeOutput.TabIndex = 101;
            this.btnChangeOutput.Text = "📁 Vælg Output";
            this.btnChangeOutput.UseVisualStyleBackColor = true;
            this.btnChangeOutput.Click += new System.EventHandler(this.btnChangeOutput_Click);
            // 
            // XMLTableSplitterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(900, 816);
            this.Controls.Add(this.groupBoxTableIndex);
            this.Controls.Add(this.groupBoxFileSelection);
            this.Controls.Add(this.groupBoxPrimaryKey);
            this.Controls.Add(this.groupBoxSplitConfiguration);
            this.Controls.Add(this.groupBoxSplitPreview);
            this.Controls.Add(this.groupBoxExecution);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(904, 534);
            this.Name = "XMLTableSplitterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XML Table Splitter";
            this.groupBoxTableIndex.ResumeLayout(false);
            this.groupBoxTableIndex.PerformLayout();
            this.groupBoxFileSelection.ResumeLayout(false);
            this.groupBoxFileSelection.PerformLayout();
            this.groupBoxPrimaryKey.ResumeLayout(false);
            this.groupBoxSplitConfiguration.ResumeLayout(false);
            this.groupBoxSplitConfiguration.PerformLayout();
            this.groupBoxSplitPreview.ResumeLayout(false);
            this.groupBoxSplitPreview.PerformLayout();
            this.groupBoxExecution.ResumeLayout(false);
            this.ResumeLayout(false);

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