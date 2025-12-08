using System.Drawing;
using System.Windows.Forms;

namespace TestvaerkstedetToolkit
{
    partial class XMLConversionForm
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
            this.richTextBoxXML = new System.Windows.Forms.RichTextBox();
            this.buttonLæsCSV = new System.Windows.Forms.Button();
            this.buttonTilføjRækker = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.textBoxTabel = new System.Windows.Forms.TextBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxStandardTekst = new System.Windows.Forms.TextBox();
            this.numericUpDownTekst = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBoxID = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label_CSVKolonner = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTekst)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBoxXML
            // 
            this.richTextBoxXML.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxXML.Location = new System.Drawing.Point(384, 276);
            this.richTextBoxXML.Name = "richTextBoxXML";
            this.richTextBoxXML.Size = new System.Drawing.Size(796, 304);
            this.richTextBoxXML.TabIndex = 8;
            this.richTextBoxXML.Text = "";
            this.richTextBoxXML.WordWrap = false;
            // 
            // buttonLæsCSV
            // 
            this.buttonLæsCSV.Enabled = false;
            this.buttonLæsCSV.Location = new System.Drawing.Point(11, 105);
            this.buttonLæsCSV.Name = "buttonLæsCSV";
            this.buttonLæsCSV.Size = new System.Drawing.Size(106, 23);
            this.buttonLæsCSV.TabIndex = 2;
            this.buttonLæsCSV.Text = "Indlæs CSV";
            this.buttonLæsCSV.UseVisualStyleBackColor = true;
            // 
            // buttonTilføjRækker
            // 
            this.buttonTilføjRækker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTilføjRækker.Enabled = false;
            this.buttonTilføjRækker.Location = new System.Drawing.Point(1063, 249);
            this.buttonTilføjRækker.Name = "buttonTilføjRækker";
            this.buttonTilføjRækker.Size = new System.Drawing.Size(117, 23);
            this.buttonTilføjRækker.TabIndex = 10;
            this.buttonTilføjRækker.Text = "Tilføj nye rækker";
            this.buttonTilføjRækker.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(11, 73);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(106, 23);
            this.button4.TabIndex = 0;
            this.button4.Text = "Vælg tabel ...";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // textBoxTabel
            // 
            this.textBoxTabel.Location = new System.Drawing.Point(123, 73);
            this.textBoxTabel.Name = "textBoxTabel";
            this.textBoxTabel.Size = new System.Drawing.Size(405, 20);
            this.textBoxTabel.TabIndex = 1;
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(123, 107);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(405, 20);
            this.textBoxID.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Standard tekst";
            // 
            // textBoxStandardTekst
            // 
            this.textBoxStandardTekst.Location = new System.Drawing.Point(123, 139);
            this.textBoxStandardTekst.Name = "textBoxStandardTekst";
            this.textBoxStandardTekst.Size = new System.Drawing.Size(405, 20);
            this.textBoxStandardTekst.TabIndex = 4;
            this.textBoxStandardTekst.Text = ".... INDTAST TEKST ....";
            // 
            // numericUpDownTekst
            // 
            this.numericUpDownTekst.Location = new System.Drawing.Point(629, 139);
            this.numericUpDownTekst.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownTekst.Name = "numericUpDownTekst";
            this.numericUpDownTekst.Size = new System.Drawing.Size(40, 20);
            this.numericUpDownTekst.TabIndex = 5;
            this.numericUpDownTekst.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Vælg tabelkolonne";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(534, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Indsæt  i kolonne";
            // 
            // richTextBoxID
            // 
            this.richTextBoxID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBoxID.Location = new System.Drawing.Point(123, 276);
            this.richTextBoxID.Name = "richTextBoxID";
            this.richTextBoxID.Size = new System.Drawing.Size(246, 304);
            this.richTextBoxID.TabIndex = 7;
            this.richTextBoxID.Text = "";
            this.richTextBoxID.WordWrap = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(120, 254);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "CSV";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(381, 254);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Tabel (kun delvist indlæst)";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(709, 73);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(330, 86);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "HUSK!  Dette program arbejder kun på valide XML filer og skemaer som er linie bas" +
    "eret (pretty print)  ";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(123, 192);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(405, 43);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // label_CSVKolonner
            // 
            this.label_CSVKolonner.AutoSize = true;
            this.label_CSVKolonner.Location = new System.Drawing.Point(120, 176);
            this.label_CSVKolonner.Name = "label_CSVKolonner";
            this.label_CSVKolonner.Size = new System.Drawing.Size(49, 13);
            this.label_CSVKolonner.TabIndex = 0;
            this.label_CSVKolonner.Text = "CSV1 ....";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 605);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1168, 23);
            this.progressBar1.TabIndex = 11;
            this.progressBar1.Visible = false;
            // 
            // XMLConversionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1200, 660);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBoxTabel);
            this.Controls.Add(this.buttonLæsCSV);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxStandardTekst);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDownTekst);
            this.Controls.Add(this.label_CSVKolonner);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.richTextBoxID);
            this.Controls.Add(this.richTextBoxXML);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonTilføjRækker);
            this.Controls.Add(this.progressBar1);
            this.MinimumSize = new System.Drawing.Size(1216, 699);
            this.Name = "XMLConversionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CSV til XML Konvertering";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTekst)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // Control declarations
        private System.Windows.Forms.RichTextBox richTextBoxXML;
        private System.Windows.Forms.Button buttonLæsCSV;
        private System.Windows.Forms.Button buttonTilføjRækker;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox textBoxTabel;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStandardTekst;
        private System.Windows.Forms.NumericUpDown numericUpDownTekst;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox richTextBoxID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label_CSVKolonner;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}