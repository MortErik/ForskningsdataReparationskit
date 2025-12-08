namespace TestvaerkstedetToolkit
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlCards = new System.Windows.Forms.Panel();

            // Card 1 - CSV FK Repair
            this.cardCSVFK = new System.Windows.Forms.Panel();
            this.lblCSVFKTitle = new System.Windows.Forms.Label();
            this.lblCSVFKDesc = new System.Windows.Forms.Label();
            this.btnCSVFKRepair = new System.Windows.Forms.Button();

            // Card 2 - XML FK Repair
            this.cardXMLFK = new System.Windows.Forms.Panel();
            this.lblXMLFKTitle = new System.Windows.Forms.Label();
            this.lblXMLFKDesc = new System.Windows.Forms.Label();
            this.btnXMLFKRepair = new System.Windows.Forms.Button();

            // Card 3 - XML Conversion
            this.cardXMLConv = new System.Windows.Forms.Panel();
            this.lblXMLConvTitle = new System.Windows.Forms.Label();
            this.lblXMLConvDesc = new System.Windows.Forms.Label();
            this.btnXMLConversion = new System.Windows.Forms.Button();

            // Card 4 - Table Splitter
            this.cardTableSplit = new System.Windows.Forms.Panel();
            this.lblTableSplitTitle = new System.Windows.Forms.Label();
            this.lblTableSplitDesc = new System.Windows.Forms.Label();
            this.btnXMLTableSplitter = new System.Windows.Forms.Button();

            this.pnlMain.SuspendLayout();
            this.pnlCards.SuspendLayout();
            this.cardCSVFK.SuspendLayout();
            this.cardXMLFK.SuspendLayout();
            this.cardXMLConv.SuspendLayout();
            this.cardTableSplit.SuspendLayout();
            this.SuspendLayout();

            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Controls.Add(this.lblSubtitle);
            this.pnlMain.Controls.Add(this.pnlCards);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(40);
            this.pnlMain.Size = new System.Drawing.Size(1000, 700);
            this.pnlMain.TabIndex = 0;

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblTitle.Location = new System.Drawing.Point(43, 50);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(367, 45);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Testværkstedets Toolkit";

            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.lblSubtitle.Location = new System.Drawing.Point(47, 105);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(451, 21);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Vælg det værktøj du vil bruge til behandle afleveringen";

            // 
            // pnlCards
            // 
            this.pnlCards.Controls.Add(this.cardCSVFK);
            this.pnlCards.Controls.Add(this.cardXMLFK);
            this.pnlCards.Controls.Add(this.cardXMLConv);
            this.pnlCards.Controls.Add(this.cardTableSplit);
            this.pnlCards.Location = new System.Drawing.Point(50, 150);
            this.pnlCards.Name = "pnlCards";
            this.pnlCards.Size = new System.Drawing.Size(900, 500);
            this.pnlCards.TabIndex = 2;

            // 
            // cardCSVFK (Top Left)
            // 
            this.cardCSVFK.BackColor = System.Drawing.Color.White;
            this.cardCSVFK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardCSVFK.Controls.Add(this.lblCSVFKTitle);
            this.cardCSVFK.Controls.Add(this.lblCSVFKDesc);
            this.cardCSVFK.Controls.Add(this.btnCSVFKRepair);
            this.cardCSVFK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cardCSVFK.Location = new System.Drawing.Point(0, 0);
            this.cardCSVFK.Name = "cardCSVFK";
            this.cardCSVFK.Size = new System.Drawing.Size(440, 240);
            this.cardCSVFK.TabIndex = 0;
            this.cardCSVFK.Paint += new System.Windows.Forms.PaintEventHandler(this.Card_Paint);

            // 
            // lblCSVFKTitle
            // 
            this.lblCSVFKTitle.AutoSize = true;
            this.lblCSVFKTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblCSVFKTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblCSVFKTitle.Location = new System.Drawing.Point(30, 30);
            this.lblCSVFKTitle.Name = "lblCSVFKTitle";
            this.lblCSVFKTitle.Size = new System.Drawing.Size(248, 30);
            this.lblCSVFKTitle.TabIndex = 0;
            this.lblCSVFKTitle.Text = "Foreign Key Repair (CSV)";

            // 
            // lblCSVFKDesc
            // 
            this.lblCSVFKDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCSVFKDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.lblCSVFKDesc.Location = new System.Drawing.Point(30, 75);
            this.lblCSVFKDesc.Name = "lblCSVFKDesc";
            this.lblCSVFKDesc.Size = new System.Drawing.Size(380, 80);
            this.lblCSVFKDesc.TabIndex = 1;
            this.lblCSVFKDesc.Text = "Identificér og reparér brudte foreign key referencer i CSV-filer. " +
                        "Genererer dummy records med korrekte datatyper for at opretholde " +
                        "referentiel integritet. Understøtter sammensatte PK.";

            // 
            // btnCSVFKRepair
            // 
            this.btnCSVFKRepair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnCSVFKRepair.FlatAppearance.BorderSize = 0;
            this.btnCSVFKRepair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCSVFKRepair.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCSVFKRepair.ForeColor = System.Drawing.Color.White;
            this.btnCSVFKRepair.Location = new System.Drawing.Point(30, 180);
            this.btnCSVFKRepair.Name = "btnCSVFKRepair";
            this.btnCSVFKRepair.Size = new System.Drawing.Size(120, 35);
            this.btnCSVFKRepair.TabIndex = 2;
            this.btnCSVFKRepair.Text = "Åbn værktøj";
            this.btnCSVFKRepair.UseVisualStyleBackColor = false;
            this.btnCSVFKRepair.Click += new System.EventHandler(this.btnCSVFKRepair_Click);

            // 
            // cardXMLFK (Top Right)
            // 
            this.cardXMLFK.BackColor = System.Drawing.Color.White;
            this.cardXMLFK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardXMLFK.Controls.Add(this.lblXMLFKTitle);
            this.cardXMLFK.Controls.Add(this.lblXMLFKDesc);
            this.cardXMLFK.Controls.Add(this.btnXMLFKRepair);
            this.cardXMLFK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cardXMLFK.Location = new System.Drawing.Point(460, 0);
            this.cardXMLFK.Name = "cardXMLFK";
            this.cardXMLFK.Size = new System.Drawing.Size(440, 240);
            this.cardXMLFK.TabIndex = 1;
            this.cardXMLFK.Paint += new System.Windows.Forms.PaintEventHandler(this.Card_Paint);

            // 
            // lblXMLFKTitle
            // 
            this.lblXMLFKTitle.AutoSize = true;
            this.lblXMLFKTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblXMLFKTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblXMLFKTitle.Location = new System.Drawing.Point(30, 30);
            this.lblXMLFKTitle.Name = "lblXMLFKTitle";
            this.lblXMLFKTitle.Size = new System.Drawing.Size(250, 30);
            this.lblXMLFKTitle.TabIndex = 0;
            this.lblXMLFKTitle.Text = "Foreign Key Repair (XML)";

            // 
            // lblXMLFKDesc
            // 
            this.lblXMLFKDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblXMLFKDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.lblXMLFKDesc.Location = new System.Drawing.Point(30, 75);
            this.lblXMLFKDesc.Name = "lblXMLFKDesc";
            this.lblXMLFKDesc.Size = new System.Drawing.Size(380, 80);
            this.lblXMLFKDesc.TabIndex = 1;
            this.lblXMLFKDesc.Text = "Reparér foreign key brud i XML-tabeller. " +
                        "Analyserer tabeldata i XML og opretter dummy records " +
                        "for at sikre data integritet i arkiveringsversionen.";

            // 
            // btnXMLFKRepair
            // 
            this.btnXMLFKRepair.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnXMLFKRepair.FlatAppearance.BorderSize = 0;
            this.btnXMLFKRepair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXMLFKRepair.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXMLFKRepair.ForeColor = System.Drawing.Color.White;
            this.btnXMLFKRepair.Location = new System.Drawing.Point(30, 180);
            this.btnXMLFKRepair.Name = "btnXMLFKRepair";
            this.btnXMLFKRepair.Size = new System.Drawing.Size(120, 35);
            this.btnXMLFKRepair.TabIndex = 2;
            this.btnXMLFKRepair.Text = "Åbn værktøj";
            this.btnXMLFKRepair.UseVisualStyleBackColor = false;
            this.btnXMLFKRepair.Click += new System.EventHandler(this.btnXMLFKRepair_Click);

            // 
            // cardXMLConv (Bottom Left)
            // 
            this.cardXMLConv.BackColor = System.Drawing.Color.White;
            this.cardXMLConv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardXMLConv.Controls.Add(this.lblXMLConvTitle);
            this.cardXMLConv.Controls.Add(this.lblXMLConvDesc);
            this.cardXMLConv.Controls.Add(this.btnXMLConversion);
            this.cardXMLConv.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cardXMLConv.Location = new System.Drawing.Point(0, 260);
            this.cardXMLConv.Name = "cardXMLConv";
            this.cardXMLConv.Size = new System.Drawing.Size(440, 240);
            this.cardXMLConv.TabIndex = 2;
            this.cardXMLConv.Paint += new System.Windows.Forms.PaintEventHandler(this.Card_Paint);

            // 
            // lblXMLConvTitle
            // 
            this.lblXMLConvTitle.AutoSize = true;
            this.lblXMLConvTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblXMLConvTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblXMLConvTitle.Location = new System.Drawing.Point(30, 30);
            this.lblXMLConvTitle.Name = "lblXMLConvTitle";
            this.lblXMLConvTitle.Size = new System.Drawing.Size(218, 30);
            this.lblXMLConvTitle.TabIndex = 0;
            this.lblXMLConvTitle.Text = "CSV til XML Konvertering";

            // 
            // lblXMLConvDesc
            // 
            this.lblXMLConvDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblXMLConvDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.lblXMLConvDesc.Location = new System.Drawing.Point(30, 75);
            this.lblXMLConvDesc.Name = "lblXMLConvDesc";
            this.lblXMLConvDesc.Size = new System.Drawing.Size(380, 80);
            this.lblXMLConvDesc.TabIndex = 1;
            this.lblXMLConvDesc.Text = "Konvertér ved hjælp af regneark til CSV-data til XML-format med schema-validering. " +
                          "Integrér nye data i eksisterende XML-struktur";


            // 
            // btnXMLConversion
            // 
            this.btnXMLConversion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnXMLConversion.FlatAppearance.BorderSize = 0;
            this.btnXMLConversion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXMLConversion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXMLConversion.ForeColor = System.Drawing.Color.White;
            this.btnXMLConversion.Location = new System.Drawing.Point(30, 180);
            this.btnXMLConversion.Name = "btnXMLConversion";
            this.btnXMLConversion.Size = new System.Drawing.Size(120, 35);
            this.btnXMLConversion.TabIndex = 2;
            this.btnXMLConversion.Text = "Åbn værktøj";
            this.btnXMLConversion.UseVisualStyleBackColor = false;
            this.btnXMLConversion.Click += new System.EventHandler(this.btnXMLConversion_Click);

            // 
            // cardTableSplit (Bottom Right)
            // 
            this.cardTableSplit.BackColor = System.Drawing.Color.White;
            this.cardTableSplit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardTableSplit.Controls.Add(this.lblTableSplitTitle);
            this.cardTableSplit.Controls.Add(this.lblTableSplitDesc);
            this.cardTableSplit.Controls.Add(this.btnXMLTableSplitter);
            this.cardTableSplit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cardTableSplit.Location = new System.Drawing.Point(460, 260);
            this.cardTableSplit.Name = "cardTableSplit";
            this.cardTableSplit.Size = new System.Drawing.Size(440, 240);
            this.cardTableSplit.TabIndex = 3;
            this.cardTableSplit.Paint += new System.Windows.Forms.PaintEventHandler(this.Card_Paint);

            // 
            // lblTableSplitTitle
            // 
            this.lblTableSplitTitle.AutoSize = true;
            this.lblTableSplitTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTableSplitTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lblTableSplitTitle.Location = new System.Drawing.Point(30, 30);
            this.lblTableSplitTitle.Name = "lblTableSplitTitle";
            this.lblTableSplitTitle.Size = new System.Drawing.Size(188, 30);
            this.lblTableSplitTitle.TabIndex = 0;
            this.lblTableSplitTitle.Text = "XML Table Splitter";

            // 
            // lblTableSplitDesc
            // 
            this.lblTableSplitDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTableSplitDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.lblTableSplitDesc.Location = new System.Drawing.Point(30, 75);
            this.lblTableSplitDesc.Name = "lblTableSplitDesc";
            this.lblTableSplitDesc.Size = new System.Drawing.Size(380, 80);
            this.lblTableSplitDesc.TabIndex = 1;
            this.lblTableSplitDesc.Text = "Split XML-tabeller med 1000+ kolonner. " +
                             "Opretholder primary key relationer, opdaterer tableIndex.xml automatisk ";

            // 
            // btnXMLTableSplitter
            // 
            this.btnXMLTableSplitter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnXMLTableSplitter.FlatAppearance.BorderSize = 0;
            this.btnXMLTableSplitter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXMLTableSplitter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXMLTableSplitter.ForeColor = System.Drawing.Color.White;
            this.btnXMLTableSplitter.Location = new System.Drawing.Point(30, 180);
            this.btnXMLTableSplitter.Name = "btnXMLTableSplitter";
            this.btnXMLTableSplitter.Size = new System.Drawing.Size(120, 35);
            this.btnXMLTableSplitter.TabIndex = 2;
            this.btnXMLTableSplitter.Text = "Åbn værktøj";
            this.btnXMLTableSplitter.UseVisualStyleBackColor = false;
            this.btnXMLTableSplitter.Click += new System.EventHandler(this.btnXMLTableSplitter_Click);

            // 
            // WelcomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "WelcomeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testværkstedets Toolkit";

            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.pnlCards.ResumeLayout(false);
            this.cardCSVFK.ResumeLayout(false);
            this.cardCSVFK.PerformLayout();
            this.cardXMLFK.ResumeLayout(false);
            this.cardXMLFK.PerformLayout();
            this.cardXMLConv.ResumeLayout(false);
            this.cardXMLConv.PerformLayout();
            this.cardTableSplit.ResumeLayout(false);
            this.cardTableSplit.PerformLayout();
            this.ResumeLayout(false);
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

        // Card 3 - XML Conv
        private System.Windows.Forms.Panel cardXMLConv;
        private System.Windows.Forms.Label lblXMLConvTitle;
        private System.Windows.Forms.Label lblXMLConvDesc;
        private System.Windows.Forms.Button btnXMLConversion;

        // Card 4 - Table Split
        private System.Windows.Forms.Panel cardTableSplit;
        private System.Windows.Forms.Label lblTableSplitTitle;
        private System.Windows.Forms.Label lblTableSplitDesc;
        private System.Windows.Forms.Button btnXMLTableSplitter;
    }
}