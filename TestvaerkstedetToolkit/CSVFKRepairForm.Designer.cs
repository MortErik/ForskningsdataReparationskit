using System.Drawing;
using System.Windows.Forms;

namespace TestvaerkstedetToolkit
{
    partial class CSVFKRepairForm
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
            labelParentCSV = new Label();
            txtParentCSV = new TextBox();
            btnBrowseParent = new Button();
            labelChildCSV = new Label();
            txtChildCSV = new TextBox();
            btnBrowseChild = new Button();
            labelParentCol = new Label();
            cmbParentColumn = new ComboBox();
            labelChildCol = new Label();
            cmbChildColumn = new ComboBox();
            btnAnalyzeFK = new Button();
            btnAddPrimaryKey = new Button();
            btnRemovePrimaryKey = new Button();
            pnlDynamicColumns = new Panel();
            labelDummyText = new Label();
            txtDummyText = new TextBox();
            labelMissingValues = new Label();
            lstMissingValues = new ListBox();
            lblFKStats = new Label();
            btnGenerateDummies = new Button();
            btnExportMissing = new Button();
            btnCopySelected = new Button();
            progressBar1 = new ProgressBar();
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            SuspendLayout();
            // 
            // labelParentCSV
            // 
            labelParentCSV.AutoSize = true;
            labelParentCSV.Location = new Point(25, 75);
            labelParentCSV.Name = "labelParentCSV";
            labelParentCSV.Size = new Size(68, 15);
            labelParentCSV.TabIndex = 0;
            labelParentCSV.Text = "Parent CSV:";
            // 
            // txtParentCSV
            // 
            txtParentCSV.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtParentCSV.Location = new Point(130, 72);
            txtParentCSV.Name = "txtParentCSV";
            txtParentCSV.Size = new Size(920, 23);
            txtParentCSV.TabIndex = 1;
            // 
            // btnBrowseParent
            // 
            btnBrowseParent.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseParent.Location = new Point(1070, 70);
            btnBrowseParent.Name = "btnBrowseParent";
            btnBrowseParent.Size = new Size(75, 23);
            btnBrowseParent.TabIndex = 2;
            btnBrowseParent.Text = "Browse...";
            btnBrowseParent.UseVisualStyleBackColor = true;
            // 
            // labelChildCSV
            // 
            labelChildCSV.AutoSize = true;
            labelChildCSV.Location = new Point(25, 105);
            labelChildCSV.Name = "labelChildCSV";
            labelChildCSV.Size = new Size(62, 15);
            labelChildCSV.TabIndex = 0;
            labelChildCSV.Text = "Child CSV:";
            // 
            // txtChildCSV
            // 
            txtChildCSV.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtChildCSV.Location = new Point(130, 102);
            txtChildCSV.Name = "txtChildCSV";
            txtChildCSV.Size = new Size(920, 23);
            txtChildCSV.TabIndex = 1;
            // 
            // btnBrowseChild
            // 
            btnBrowseChild.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowseChild.Location = new Point(1070, 100);
            btnBrowseChild.Name = "btnBrowseChild";
            btnBrowseChild.Size = new Size(75, 23);
            btnBrowseChild.TabIndex = 2;
            btnBrowseChild.Text = "Browse...";
            btnBrowseChild.UseVisualStyleBackColor = true;
            // 
            // labelParentCol
            // 
            labelParentCol.AutoSize = true;
            labelParentCol.Location = new Point(25, 145);
            labelParentCol.Name = "labelParentCol";
            labelParentCol.Size = new Size(99, 15);
            labelParentCol.TabIndex = 0;
            labelParentCol.Text = "Parent Column 1:";
            // 
            // cmbParentColumn
            // 
            cmbParentColumn.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbParentColumn.Location = new Point(130, 142);
            cmbParentColumn.Name = "cmbParentColumn";
            cmbParentColumn.Size = new Size(300, 23);
            cmbParentColumn.TabIndex = 3;
            // 
            // labelChildCol
            // 
            labelChildCol.AutoSize = true;
            labelChildCol.Location = new Point(451, 145);
            labelChildCol.Name = "labelChildCol";
            labelChildCol.Size = new Size(93, 15);
            labelChildCol.TabIndex = 0;
            labelChildCol.Text = "Child Column 1:";
            // 
            // cmbChildColumn
            // 
            cmbChildColumn.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChildColumn.Location = new Point(564, 142);
            cmbChildColumn.Name = "cmbChildColumn";
            cmbChildColumn.Size = new Size(300, 23);
            cmbChildColumn.TabIndex = 4;
            // 
            // btnAnalyzeFK
            // 
            btnAnalyzeFK.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnAnalyzeFK.Location = new Point(130, 340);
            btnAnalyzeFK.Name = "btnAnalyzeFK";
            btnAnalyzeFK.Size = new Size(150, 30);
            btnAnalyzeFK.TabIndex = 8;
            btnAnalyzeFK.Text = "Analyze FK";
            btnAnalyzeFK.UseVisualStyleBackColor = true;
            // 
            // btnAddPrimaryKey
            // 
            btnAddPrimaryKey.Location = new Point(130, 175);
            btnAddPrimaryKey.Name = "btnAddPrimaryKey";
            btnAddPrimaryKey.Size = new Size(100, 23);
            btnAddPrimaryKey.TabIndex = 5;
            btnAddPrimaryKey.Text = "Add Column";
            btnAddPrimaryKey.UseVisualStyleBackColor = true;
            // 
            // btnRemovePrimaryKey
            // 
            btnRemovePrimaryKey.Enabled = false;
            btnRemovePrimaryKey.Location = new Point(240, 175);
            btnRemovePrimaryKey.Name = "btnRemovePrimaryKey";
            btnRemovePrimaryKey.Size = new Size(100, 23);
            btnRemovePrimaryKey.TabIndex = 6;
            btnRemovePrimaryKey.Text = "Remove Last";
            btnRemovePrimaryKey.UseVisualStyleBackColor = true;
            // 
            // pnlDynamicColumns
            // 
            pnlDynamicColumns.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDynamicColumns.AutoScroll = true;
            pnlDynamicColumns.BorderStyle = BorderStyle.FixedSingle;
            pnlDynamicColumns.Location = new Point(130, 205);
            pnlDynamicColumns.Name = "pnlDynamicColumns";
            pnlDynamicColumns.Size = new Size(900, 120);
            pnlDynamicColumns.TabIndex = 7;
            // 
            // labelDummyText
            // 
            labelDummyText.AutoSize = true;
            labelDummyText.Location = new Point(25, 385);
            labelDummyText.Name = "labelDummyText";
            labelDummyText.Size = new Size(81, 15);
            labelDummyText.TabIndex = 0;
            labelDummyText.Text = "Dummy tekst:";
            labelDummyText.Click += labelDummyText_Click;
            // 
            // txtDummyText
            // 
            txtDummyText.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDummyText.Location = new Point(130, 382);
            txtDummyText.Multiline = true;
            txtDummyText.Name = "txtDummyText";
            txtDummyText.Size = new Size(900, 51);
            txtDummyText.TabIndex = 9;
            txtDummyText.Text = "Betydning ukendt. Rækken er tilføjet under aflevering til arkiv, for at sikre referentiel integritet i databasen af hensyn til langtidsbevaring";
            // 
            // labelMissingValues
            // 
            labelMissingValues.AutoSize = true;
            labelMissingValues.Location = new Point(12, 450);
            labelMissingValues.Name = "labelMissingValues";
            labelMissingValues.Size = new Size(113, 15);
            labelMissingValues.TabIndex = 0;
            labelMissingValues.Text = "Manglende værdier:";
            // 
            // lstMissingValues
            // 
            lstMissingValues.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstMissingValues.ItemHeight = 15;
            lstMissingValues.Location = new Point(130, 450);
            lstMissingValues.Name = "lstMissingValues";
            lstMissingValues.SelectionMode = SelectionMode.MultiExtended;
            lstMissingValues.Size = new Size(700, 199);
            lstMissingValues.TabIndex = 10;
            // 
            // lblFKStats
            // 
            lblFKStats.AutoSize = true;
            lblFKStats.ForeColor = Color.DarkBlue;
            lblFKStats.Location = new Point(300, 350);
            lblFKStats.Name = "lblFKStats";
            lblFKStats.Size = new Size(155, 15);
            lblFKStats.TabIndex = 0;
            lblFKStats.Text = "Klik 'Analyze FK' for at starte";
            // 
            // btnGenerateDummies
            // 
            btnGenerateDummies.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGenerateDummies.BackColor = Color.LightGreen;
            btnGenerateDummies.Enabled = false;
            btnGenerateDummies.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnGenerateDummies.Location = new Point(836, 450);
            btnGenerateDummies.Name = "btnGenerateDummies";
            btnGenerateDummies.Size = new Size(220, 35);
            btnGenerateDummies.TabIndex = 11;
            btnGenerateDummies.Text = "Generer Nye Rækker til Parent";
            btnGenerateDummies.UseVisualStyleBackColor = false;
            // 
            // btnExportMissing
            // 
            btnExportMissing.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportMissing.Enabled = false;
            btnExportMissing.Location = new Point(855, 491);
            btnExportMissing.Name = "btnExportMissing";
            btnExportMissing.Size = new Size(180, 25);
            btnExportMissing.TabIndex = 12;
            btnExportMissing.Text = "Export til fil";
            btnExportMissing.UseVisualStyleBackColor = true;
            // 
            // btnCopySelected
            // 
            btnCopySelected.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopySelected.Enabled = false;
            btnCopySelected.Location = new Point(855, 522);
            btnCopySelected.Name = "btnCopySelected";
            btnCopySelected.Size = new Size(180, 25);
            btnCopySelected.TabIndex = 13;
            btnCopySelected.Text = "Kopiér markerede";
            btnCopySelected.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(12, 668);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(1156, 23);
            progressBar1.TabIndex = 14;
            progressBar1.Visible = false;
            // 
            // CSVFKRepairForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1180, 703);
            Controls.Add(labelParentCSV);
            Controls.Add(txtParentCSV);
            Controls.Add(btnBrowseParent);
            Controls.Add(labelChildCSV);
            Controls.Add(txtChildCSV);
            Controls.Add(btnBrowseChild);
            Controls.Add(labelParentCol);
            Controls.Add(cmbParentColumn);
            Controls.Add(labelChildCol);
            Controls.Add(cmbChildColumn);
            Controls.Add(btnAddPrimaryKey);
            Controls.Add(btnRemovePrimaryKey);
            Controls.Add(pnlDynamicColumns);
            Controls.Add(btnAnalyzeFK);
            Controls.Add(lblFKStats);
            Controls.Add(labelDummyText);
            Controls.Add(txtDummyText);
            Controls.Add(labelMissingValues);
            Controls.Add(lstMissingValues);
            Controls.Add(btnGenerateDummies);
            Controls.Add(btnExportMissing);
            Controls.Add(btnCopySelected);
            Controls.Add(progressBar1);
            MinimumSize = new Size(1196, 669);
            Name = "CSVFKRepairForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Foreign Key Repair - CSV";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        // Control declarations
        private System.Windows.Forms.Label labelParentCSV;
        private System.Windows.Forms.TextBox txtParentCSV;
        private System.Windows.Forms.Button btnBrowseParent;
        private System.Windows.Forms.Label labelChildCSV;
        private System.Windows.Forms.TextBox txtChildCSV;
        private System.Windows.Forms.Button btnBrowseChild;
        private System.Windows.Forms.Label labelParentCol;
        private System.Windows.Forms.ComboBox cmbParentColumn;
        private System.Windows.Forms.Label labelChildCol;
        private System.Windows.Forms.ComboBox cmbChildColumn;
        private System.Windows.Forms.Button btnAnalyzeFK;
        private System.Windows.Forms.Button btnAddPrimaryKey;
        private System.Windows.Forms.Button btnRemovePrimaryKey;
        private System.Windows.Forms.Panel pnlDynamicColumns;
        private System.Windows.Forms.Label labelDummyText;
        private System.Windows.Forms.TextBox txtDummyText;
        private System.Windows.Forms.Label labelMissingValues;
        private System.Windows.Forms.ListBox lstMissingValues;
        private System.Windows.Forms.Label lblFKStats;
        private System.Windows.Forms.Button btnGenerateDummies;
        private System.Windows.Forms.Button btnExportMissing;
        private System.Windows.Forms.Button btnCopySelected;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}