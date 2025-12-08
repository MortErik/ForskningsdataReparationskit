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
            this.labelParentCSV = new System.Windows.Forms.Label();
            this.txtParentCSV = new System.Windows.Forms.TextBox();
            this.btnBrowseParent = new System.Windows.Forms.Button();
            this.labelChildCSV = new System.Windows.Forms.Label();
            this.txtChildCSV = new System.Windows.Forms.TextBox();
            this.btnBrowseChild = new System.Windows.Forms.Button();
            this.labelParentCol = new System.Windows.Forms.Label();
            this.cmbParentColumn = new System.Windows.Forms.ComboBox();
            this.labelChildCol = new System.Windows.Forms.Label();
            this.cmbChildColumn = new System.Windows.Forms.ComboBox();
            this.btnAnalyzeFK = new System.Windows.Forms.Button();
            this.btnAddPrimaryKey = new System.Windows.Forms.Button();
            this.btnRemovePrimaryKey = new System.Windows.Forms.Button();
            this.pnlDynamicColumns = new System.Windows.Forms.Panel();
            this.labelDummyText = new System.Windows.Forms.Label();
            this.txtDummyText = new System.Windows.Forms.TextBox();
            this.labelMissingValues = new System.Windows.Forms.Label();
            this.lstMissingValues = new System.Windows.Forms.ListBox();
            this.lblFKStats = new System.Windows.Forms.Label();
            this.btnGenerateDummies = new System.Windows.Forms.Button();
            this.btnExportMissing = new System.Windows.Forms.Button();
            this.btnCopySelected = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // labelParentCSV
            // 
            this.labelParentCSV.AutoSize = true;
            this.labelParentCSV.Location = new System.Drawing.Point(20, 75);
            this.labelParentCSV.Name = "labelParentCSV";
            this.labelParentCSV.Size = new System.Drawing.Size(65, 13);
            this.labelParentCSV.TabIndex = 0;
            this.labelParentCSV.Text = "Parent CSV:";
            // 
            // txtParentCSV
            // 
            this.txtParentCSV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParentCSV.Location = new System.Drawing.Point(130, 72);
            this.txtParentCSV.Name = "txtParentCSV";
            this.txtParentCSV.Size = new System.Drawing.Size(920, 20);
            this.txtParentCSV.TabIndex = 1;
            // 
            // btnBrowseParent
            // 
            this.btnBrowseParent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseParent.Location = new System.Drawing.Point(1070, 70);
            this.btnBrowseParent.Name = "btnBrowseParent";
            this.btnBrowseParent.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseParent.TabIndex = 2;
            this.btnBrowseParent.Text = "Browse...";
            this.btnBrowseParent.UseVisualStyleBackColor = true;
            // 
            // labelChildCSV
            // 
            this.labelChildCSV.AutoSize = true;
            this.labelChildCSV.Location = new System.Drawing.Point(20, 105);
            this.labelChildCSV.Name = "labelChildCSV";
            this.labelChildCSV.Size = new System.Drawing.Size(57, 13);
            this.labelChildCSV.TabIndex = 0;
            this.labelChildCSV.Text = "Child CSV:";
            // 
            // txtChildCSV
            // 
            this.txtChildCSV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChildCSV.Location = new System.Drawing.Point(130, 102);
            this.txtChildCSV.Name = "txtChildCSV";
            this.txtChildCSV.Size = new System.Drawing.Size(920, 20);
            this.txtChildCSV.TabIndex = 1;
            // 
            // btnBrowseChild
            // 
            this.btnBrowseChild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseChild.Location = new System.Drawing.Point(1070, 100);
            this.btnBrowseChild.Name = "btnBrowseChild";
            this.btnBrowseChild.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseChild.TabIndex = 2;
            this.btnBrowseChild.Text = "Browse...";
            this.btnBrowseChild.UseVisualStyleBackColor = true;
            // 
            // labelParentCol
            // 
            this.labelParentCol.AutoSize = true;
            this.labelParentCol.Location = new System.Drawing.Point(20, 145);
            this.labelParentCol.Name = "labelParentCol";
            this.labelParentCol.Size = new System.Drawing.Size(88, 13);
            this.labelParentCol.TabIndex = 0;
            this.labelParentCol.Text = "Parent Column 1:";
            // 
            // cmbParentColumn
            // 
            this.cmbParentColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParentColumn.Location = new System.Drawing.Point(130, 142);
            this.cmbParentColumn.Name = "cmbParentColumn";
            this.cmbParentColumn.Size = new System.Drawing.Size(300, 21);
            this.cmbParentColumn.TabIndex = 3;
            // 
            // labelChildCol
            // 
            this.labelChildCol.AutoSize = true;
            this.labelChildCol.Location = new System.Drawing.Point(450, 145);
            this.labelChildCol.Name = "labelChildCol";
            this.labelChildCol.Size = new System.Drawing.Size(80, 13);
            this.labelChildCol.TabIndex = 0;
            this.labelChildCol.Text = "Child Column 1:";
            // 
            // cmbChildColumn
            // 
            this.cmbChildColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChildColumn.Location = new System.Drawing.Point(550, 142);
            this.cmbChildColumn.Name = "cmbChildColumn";
            this.cmbChildColumn.Size = new System.Drawing.Size(300, 21);
            this.cmbChildColumn.TabIndex = 4;
            // 
            // btnAnalyzeFK
            // 
            this.btnAnalyzeFK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnAnalyzeFK.Location = new System.Drawing.Point(130, 340);
            this.btnAnalyzeFK.Name = "btnAnalyzeFK";
            this.btnAnalyzeFK.Size = new System.Drawing.Size(150, 30);
            this.btnAnalyzeFK.TabIndex = 8;
            this.btnAnalyzeFK.Text = "Analyze FK";
            this.btnAnalyzeFK.UseVisualStyleBackColor = true;
            // 
            // btnAddPrimaryKey
            // 
            this.btnAddPrimaryKey.Location = new System.Drawing.Point(130, 175);
            this.btnAddPrimaryKey.Name = "btnAddPrimaryKey";
            this.btnAddPrimaryKey.Size = new System.Drawing.Size(100, 23);
            this.btnAddPrimaryKey.TabIndex = 5;
            this.btnAddPrimaryKey.Text = "Add Column";
            this.btnAddPrimaryKey.UseVisualStyleBackColor = true;
            // 
            // btnRemovePrimaryKey
            // 
            this.btnRemovePrimaryKey.Enabled = false;
            this.btnRemovePrimaryKey.Location = new System.Drawing.Point(240, 175);
            this.btnRemovePrimaryKey.Name = "btnRemovePrimaryKey";
            this.btnRemovePrimaryKey.Size = new System.Drawing.Size(100, 23);
            this.btnRemovePrimaryKey.TabIndex = 6;
            this.btnRemovePrimaryKey.Text = "Remove Last";
            this.btnRemovePrimaryKey.UseVisualStyleBackColor = true;
            // 
            // pnlDynamicColumns
            // 
            this.pnlDynamicColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDynamicColumns.AutoScroll = true;
            this.pnlDynamicColumns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDynamicColumns.Location = new System.Drawing.Point(130, 205);
            this.pnlDynamicColumns.Name = "pnlDynamicColumns";
            this.pnlDynamicColumns.Size = new System.Drawing.Size(900, 120);
            this.pnlDynamicColumns.TabIndex = 7;
            // 
            // labelDummyText
            // 
            this.labelDummyText.AutoSize = true;
            this.labelDummyText.Location = new System.Drawing.Point(20, 385);
            this.labelDummyText.Name = "labelDummyText";
            this.labelDummyText.Size = new System.Drawing.Size(71, 13);
            this.labelDummyText.TabIndex = 0;
            this.labelDummyText.Text = "Dummy tekst:";
            // 
            // txtDummyText
            // 
            this.txtDummyText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDummyText.Location = new System.Drawing.Point(130, 382);
            this.txtDummyText.Name = "txtDummyText";
            this.txtDummyText.Size = new System.Drawing.Size(900, 20);
            this.txtDummyText.TabIndex = 9;
            this.txtDummyText.Text = "Betydning ukendt. Rækken er tilføjet under aflevering til arkiv, for at sikre ref" +
    "erentiel integritet i databasen af hensyn til langtidsbevaring";
            // 
            // labelMissingValues
            // 
            this.labelMissingValues.AutoSize = true;
            this.labelMissingValues.Location = new System.Drawing.Point(20, 420);
            this.labelMissingValues.Name = "labelMissingValues";
            this.labelMissingValues.Size = new System.Drawing.Size(102, 13);
            this.labelMissingValues.TabIndex = 0;
            this.labelMissingValues.Text = "Manglende værdier:";
            // 
            // lstMissingValues
            // 
            this.lstMissingValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstMissingValues.Location = new System.Drawing.Point(130, 420);
            this.lstMissingValues.Name = "lstMissingValues";
            this.lstMissingValues.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstMissingValues.Size = new System.Drawing.Size(700, 238);
            this.lstMissingValues.TabIndex = 10;
            // 
            // lblFKStats
            // 
            this.lblFKStats.AutoSize = true;
            this.lblFKStats.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblFKStats.Location = new System.Drawing.Point(300, 350);
            this.lblFKStats.Name = "lblFKStats";
            this.lblFKStats.Size = new System.Drawing.Size(140, 13);
            this.lblFKStats.TabIndex = 0;
            this.lblFKStats.Text = "Klik \'Analyze FK\' for at starte";
            // 
            // btnGenerateDummies
            // 
            this.btnGenerateDummies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateDummies.BackColor = System.Drawing.Color.LightGreen;
            this.btnGenerateDummies.Enabled = false;
            this.btnGenerateDummies.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnGenerateDummies.Location = new System.Drawing.Point(850, 420);
            this.btnGenerateDummies.Name = "btnGenerateDummies";
            this.btnGenerateDummies.Size = new System.Drawing.Size(220, 35);
            this.btnGenerateDummies.TabIndex = 11;
            this.btnGenerateDummies.Text = "Generer Nye Rækker til Parent";
            this.btnGenerateDummies.UseVisualStyleBackColor = false;
            // 
            // btnExportMissing
            // 
            this.btnExportMissing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportMissing.Enabled = false;
            this.btnExportMissing.Location = new System.Drawing.Point(869, 465);
            this.btnExportMissing.Name = "btnExportMissing";
            this.btnExportMissing.Size = new System.Drawing.Size(180, 25);
            this.btnExportMissing.TabIndex = 12;
            this.btnExportMissing.Text = "Export til fil";
            this.btnExportMissing.UseVisualStyleBackColor = true;
            // 
            // btnCopySelected
            // 
            this.btnCopySelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopySelected.Enabled = false;
            this.btnCopySelected.Location = new System.Drawing.Point(869, 500);
            this.btnCopySelected.Name = "btnCopySelected";
            this.btnCopySelected.Size = new System.Drawing.Size(180, 25);
            this.btnCopySelected.TabIndex = 13;
            this.btnCopySelected.Text = "Kopiér markerede";
            this.btnCopySelected.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 668);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1156, 23);
            this.progressBar1.TabIndex = 14;
            this.progressBar1.Visible = false;
            // 
            // CSVFKRepairForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1180, 703);
            this.Controls.Add(this.labelParentCSV);
            this.Controls.Add(this.txtParentCSV);
            this.Controls.Add(this.btnBrowseParent);
            this.Controls.Add(this.labelChildCSV);
            this.Controls.Add(this.txtChildCSV);
            this.Controls.Add(this.btnBrowseChild);
            this.Controls.Add(this.labelParentCol);
            this.Controls.Add(this.cmbParentColumn);
            this.Controls.Add(this.labelChildCol);
            this.Controls.Add(this.cmbChildColumn);
            this.Controls.Add(this.btnAddPrimaryKey);
            this.Controls.Add(this.btnRemovePrimaryKey);
            this.Controls.Add(this.pnlDynamicColumns);
            this.Controls.Add(this.btnAnalyzeFK);
            this.Controls.Add(this.lblFKStats);
            this.Controls.Add(this.labelDummyText);
            this.Controls.Add(this.txtDummyText);
            this.Controls.Add(this.labelMissingValues);
            this.Controls.Add(this.lstMissingValues);
            this.Controls.Add(this.btnGenerateDummies);
            this.Controls.Add(this.btnExportMissing);
            this.Controls.Add(this.btnCopySelected);
            this.Controls.Add(this.progressBar1);
            this.MinimumSize = new System.Drawing.Size(1196, 669);
            this.Name = "CSVFKRepairForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Foreign Key Repair - CSV";
            this.ResumeLayout(false);
            this.PerformLayout();

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