namespace ForskningsdataReparationskit
{
    partial class WelcomeForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlMain = new System.Windows.Forms.Panel();
            lblTitle = new System.Windows.Forms.Label();
            lblSubtitle = new System.Windows.Forms.Label();
            pnlCards = new System.Windows.Forms.Panel();
            cardCSVFK = new System.Windows.Forms.Panel();
            lblCSVFKTitle = new System.Windows.Forms.Label();
            lblCSVFKDesc = new System.Windows.Forms.Label();
            btnCSVFKRepair = new System.Windows.Forms.Button();
            cardXMLFK = new System.Windows.Forms.Panel();
            lblXMLFKTitle = new System.Windows.Forms.Label();
            lblXMLFKDesc = new System.Windows.Forms.Label();
            btnXMLFKRepair = new System.Windows.Forms.Button();
            cardTableSplit = new System.Windows.Forms.Panel();
            lblTableSplitTitle = new System.Windows.Forms.Label();
            lblTableSplitDesc = new System.Windows.Forms.Label();
            btnXMLTableSplitter = new System.Windows.Forms.Button();
            pnlMain.SuspendLayout();
            pnlCards.SuspendLayout();
            cardCSVFK.SuspendLayout();
            cardXMLFK.SuspendLayout();
            cardTableSplit.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            pnlMain.Controls.Add(lblTitle);
            pnlMain.Controls.Add(lblSubtitle);
            pnlMain.Controls.Add(pnlCards);
            pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlMain.Location = new System.Drawing.Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Padding = new System.Windows.Forms.Padding(40);
            pnlMain.Size = new System.Drawing.Size(1000, 700);
            pnlMain.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(51, 51, 51);
            lblTitle.Location = new System.Drawing.Point(43, 50);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(373, 45);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Reparationskit til forskningsdata";
            lblTitle.Click += lblTitle_Click;
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 12F);
            lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(102, 102, 102);
            lblSubtitle.Location = new System.Drawing.Point(47, 105);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new System.Drawing.Size(390, 21);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Vælg det værktøj du vil bruge til behandle afleveringen";
            // 
            // pnlCards
            // 
            pnlCards.Controls.Add(cardCSVFK);
            pnlCards.Controls.Add(cardXMLFK);
            pnlCards.Controls.Add(cardTableSplit);
            pnlCards.Location = new System.Drawing.Point(50, 150);
            pnlCards.Name = "pnlCards";
            pnlCards.Size = new System.Drawing.Size(900, 500);
            pnlCards.TabIndex = 2;
            // 
            // cardCSVFK
            // 
            cardCSVFK.BackColor = System.Drawing.Color.White;
            cardCSVFK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            cardCSVFK.Controls.Add(lblCSVFKTitle);
            cardCSVFK.Controls.Add(lblCSVFKDesc);
            cardCSVFK.Controls.Add(btnCSVFKRepair);
            cardCSVFK.Cursor = System.Windows.Forms.Cursors.Hand;
            cardCSVFK.Location = new System.Drawing.Point(0, 0);
            cardCSVFK.Name = "cardCSVFK";
            cardCSVFK.Size = new System.Drawing.Size(440, 240);
            cardCSVFK.TabIndex = 0;
            cardCSVFK.Paint += Card_Paint;
            // 
            // lblCSVFKTitle
            // 
            lblCSVFKTitle.AutoSize = true;
            lblCSVFKTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblCSVFKTitle.ForeColor = System.Drawing.Color.FromArgb(51, 51, 51);
            lblCSVFKTitle.Location = new System.Drawing.Point(30, 30);
            lblCSVFKTitle.Name = "lblCSVFKTitle";
            lblCSVFKTitle.Size = new System.Drawing.Size(270, 30);
            lblCSVFKTitle.TabIndex = 0;
            lblCSVFKTitle.Text = "Foreign Key Repair (CSV)";
            // 
            // lblCSVFKDesc
            // 
            lblCSVFKDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblCSVFKDesc.ForeColor = System.Drawing.Color.FromArgb(102, 102, 102);
            lblCSVFKDesc.Location = new System.Drawing.Point(30, 75);
            lblCSVFKDesc.Name = "lblCSVFKDesc";
            lblCSVFKDesc.Size = new System.Drawing.Size(380, 80);
            lblCSVFKDesc.TabIndex = 1;
            lblCSVFKDesc.Text = "Identificér og reparér brudte foreign key referencer i CSV-filer. Genererer dummy records med korrekte datatyper for at opretholde referentiel integritet. Understøtter sammensatte PK.";
            // 
            // btnCSVFKRepair
            // 
            btnCSVFKRepair.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            btnCSVFKRepair.FlatAppearance.BorderSize = 0;
            btnCSVFKRepair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCSVFKRepair.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnCSVFKRepair.ForeColor = System.Drawing.Color.White;
            btnCSVFKRepair.Location = new System.Drawing.Point(30, 180);
            btnCSVFKRepair.Name = "btnCSVFKRepair";
            btnCSVFKRepair.Size = new System.Drawing.Size(120, 35);
            btnCSVFKRepair.TabIndex = 2;
            btnCSVFKRepair.Text = "Åbn værktøj";
            btnCSVFKRepair.UseVisualStyleBackColor = false;
            btnCSVFKRepair.Click += btnCSVFKRepair_Click;
            // 
            // cardXMLFK
            // 
            cardXMLFK.BackColor = System.Drawing.Color.White;
            cardXMLFK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            cardXMLFK.Controls.Add(lblXMLFKTitle);
            cardXMLFK.Controls.Add(lblXMLFKDesc);
            cardXMLFK.Controls.Add(btnXMLFKRepair);
            cardXMLFK.Cursor = System.Windows.Forms.Cursors.Hand;
            cardXMLFK.Location = new System.Drawing.Point(460, 0);
            cardXMLFK.Name = "cardXMLFK";
            cardXMLFK.Size = new System.Drawing.Size(440, 240);
            cardXMLFK.TabIndex = 1;
            cardXMLFK.Paint += Card_Paint;
            // 
            // lblXMLFKTitle
            // 
            lblXMLFKTitle.AutoSize = true;
            lblXMLFKTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblXMLFKTitle.ForeColor = System.Drawing.Color.FromArgb(51, 51, 51);
            lblXMLFKTitle.Location = new System.Drawing.Point(30, 30);
            lblXMLFKTitle.Name = "lblXMLFKTitle";
            lblXMLFKTitle.Size = new System.Drawing.Size(275, 30);
            lblXMLFKTitle.TabIndex = 0;
            lblXMLFKTitle.Text = "Foreign Key Repair (XML)";
            // 
            // lblXMLFKDesc
            // 
            lblXMLFKDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblXMLFKDesc.ForeColor = System.Drawing.Color.FromArgb(102, 102, 102);
            lblXMLFKDesc.Location = new System.Drawing.Point(30, 75);
            lblXMLFKDesc.Name = "lblXMLFKDesc";
            lblXMLFKDesc.Size = new System.Drawing.Size(380, 80);
            lblXMLFKDesc.TabIndex = 1;
            lblXMLFKDesc.Text = "Reparér foreign key brud i XML-tabeller. Analyserer tabeldata i XML og opretter dummy records for at sikre data integritet i arkiveringsversionen.";
            // 
            // btnXMLFKRepair
            // 
            btnXMLFKRepair.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            btnXMLFKRepair.FlatAppearance.BorderSize = 0;
            btnXMLFKRepair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnXMLFKRepair.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnXMLFKRepair.ForeColor = System.Drawing.Color.White;
            btnXMLFKRepair.Location = new System.Drawing.Point(30, 180);
            btnXMLFKRepair.Name = "btnXMLFKRepair";
            btnXMLFKRepair.Size = new System.Drawing.Size(120, 35);
            btnXMLFKRepair.TabIndex = 2;
            btnXMLFKRepair.Text = "Åbn værktøj";
            btnXMLFKRepair.UseVisualStyleBackColor = false;
            btnXMLFKRepair.Click += btnXMLFKRepair_Click;
            // 
            // cardTableSplit
            // 
            cardTableSplit.BackColor = System.Drawing.Color.White;
            cardTableSplit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            cardTableSplit.Controls.Add(lblTableSplitTitle);
            cardTableSplit.Controls.Add(lblTableSplitDesc);
            cardTableSplit.Controls.Add(btnXMLTableSplitter);
            cardTableSplit.Cursor = System.Windows.Forms.Cursors.Hand;
            cardTableSplit.Location = new System.Drawing.Point(0, 260);
            cardTableSplit.Name = "cardTableSplit";
            cardTableSplit.Size = new System.Drawing.Size(440, 240);
            cardTableSplit.TabIndex = 3;
            cardTableSplit.Paint += Card_Paint;
            // 
            // lblTableSplitTitle
            // 
            lblTableSplitTitle.AutoSize = true;
            lblTableSplitTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblTableSplitTitle.ForeColor = System.Drawing.Color.FromArgb(51, 51, 51);
            lblTableSplitTitle.Location = new System.Drawing.Point(30, 30);
            lblTableSplitTitle.Name = "lblTableSplitTitle";
            lblTableSplitTitle.Size = new System.Drawing.Size(203, 30);
            lblTableSplitTitle.TabIndex = 0;
            lblTableSplitTitle.Text = "XML Table Splitter";
            // 
            // lblTableSplitDesc
            // 
            lblTableSplitDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblTableSplitDesc.ForeColor = System.Drawing.Color.FromArgb(102, 102, 102);
            lblTableSplitDesc.Location = new System.Drawing.Point(30, 75);
            lblTableSplitDesc.Name = "lblTableSplitDesc";
            lblTableSplitDesc.Size = new System.Drawing.Size(380, 80);
            lblTableSplitDesc.TabIndex = 1;
            lblTableSplitDesc.Text = "Split XML-tabeller med 1000+ kolonner. Opretholder primary key relationer, opdaterer tableIndex.xml automatisk ";
            // 
            // btnXMLTableSplitter
            // 
            btnXMLTableSplitter.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            btnXMLTableSplitter.FlatAppearance.BorderSize = 0;
            btnXMLTableSplitter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnXMLTableSplitter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnXMLTableSplitter.ForeColor = System.Drawing.Color.White;
            btnXMLTableSplitter.Location = new System.Drawing.Point(30, 180);
            btnXMLTableSplitter.Name = "btnXMLTableSplitter";
            btnXMLTableSplitter.Size = new System.Drawing.Size(120, 35);
            btnXMLTableSplitter.TabIndex = 2;
            btnXMLTableSplitter.Text = "Åbn værktøj";
            btnXMLTableSplitter.UseVisualStyleBackColor = false;
            btnXMLTableSplitter.Click += btnXMLTableSplitter_Click;
            // 
            // WelcomeForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
            ClientSize = new System.Drawing.Size(1000, 700);
            Controls.Add(pnlMain);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "WelcomeForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Reparationskit til forskningsdata";
            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();
            pnlCards.ResumeLayout(false);
            cardCSVFK.ResumeLayout(false);
            cardCSVFK.PerformLayout();
            cardXMLFK.ResumeLayout(false);
            cardXMLFK.PerformLayout();
            cardTableSplit.ResumeLayout(false);
            cardTableSplit.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel pnlCards;

        // Card 1 - CSV FK
        private System.Windows.Forms.Panel cardCSVFK;
        private System.Windows.Forms.Label lblCSVFKTitle;
        private System.Windows.Forms.Label lblCSVFKDesc;
        private System.Windows.Forms.Button btnCSVFKRepair;

        // Card 2 - XML FK
        private System.Windows.Forms.Panel cardXMLFK;
        private System.Windows.Forms.Label lblXMLFKTitle;
        private System.Windows.Forms.Label lblXMLFKDesc;
        private System.Windows.Forms.Button btnXMLFKRepair;

        // Card 4 - Table Split
        private System.Windows.Forms.Panel cardTableSplit;
        private System.Windows.Forms.Label lblTableSplitTitle;
        private System.Windows.Forms.Label lblTableSplitDesc;
        private System.Windows.Forms.Button btnXMLTableSplitter;
    }
}