using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TestvaerkstedetToolkit
{
    public partial class XMLConversionForm : Form
    {
        #region Private Classes

        /// <summary>
        /// XML schema kolonne information
        /// </summary>
        private class columnClass
        {
            public int colNr = 0;
            public string dataType = "";
            public string nillable = "";
        }

        #endregion

        #region Fields

        // XML conversion fields
        int currentRowCount = 0;
        Dictionary<int, string> xmlElementer = new Dictionary<int, string>();
        List<columnClass> colList = new List<columnClass>();
        List<NumericUpDown> numList = new List<NumericUpDown>();

        // Dialogs
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;

        #endregion

        #region Constructor

        public XMLConversionForm()
        {
            InitializeComponent();
            SetupXmlConversionEventHandlers();
        }

        /// <summary>
        /// Setup event handlers for XML Conversion controls
        /// </summary>
        private void SetupXmlConversionEventHandlers()
        {
            try
            {
                // Map button handlers
                if (buttonLæsCSV != null)
                    buttonLæsCSV.Click += button1_Click;  // Læs CSV handler

                if (buttonTilføjRækker != null)
                    buttonTilføjRækker.Click += button3_Click;  // Tilføj Rækker handler

                if (button4 != null)
                    button4.Click += button4_Click;  // Gem XML handler

                System.Diagnostics.Debug.WriteLine("[Setup] XML Conversion event handlers wired");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetupXmlConversionEventHandlers error: {ex.Message}");
            }
        }

        #endregion

        #region XML Conversion - Eksisterende Funktionalitet

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Tekst|*.*";
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                textBoxID.Text = openFileDialog1.FileName;
                richTextBoxID.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
            }

            string[] items = richTextBoxID.Lines[0].Split(';');
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private int GetColCount(string tabelFilename)
        {
            bool firstFound = false;
            int currentRowCount = 0;
            foreach (string line in File.ReadAllLines(tabelFilename, Encoding.Default))
            {
                if (firstFound == false)
                {
                    if (line.Trim().ToLower() == "<row>")
                    {
                        firstFound = true;
                    }
                }
                else
                {
                    if (line.Trim().ToLower() == "</row>")
                    {
                        break;
                    }
                    currentRowCount++;
                }
            }
            return (currentRowCount);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (isKolonnevalgValid().ToString().ToLower() != "true")
            {
                MessageBox.Show("De valgte kolonner overlapper hinanden!");
                return;
            }

            string destinationFilename = "";
            saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(textBoxTabel.Text) + "_new.xml";
            DialogResult res = saveFileDialog1.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                destinationFilename = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }

            if (File.Exists(textBoxTabel.Text) == false)
            {
                MessageBox.Show("Kunne ikke finde tabelfil:" + textBoxTabel.Text);
                return;
            }

            string schemaFilePath = Path.Combine(Path.GetDirectoryName(textBoxTabel.Text), Path.GetFileNameWithoutExtension(textBoxTabel.Text) + ".xsd");
            if (File.Exists(schemaFilePath) == false)
            {
                MessageBox.Show("Kunne ikke finde den tilhørende schemafil:" + schemaFilePath);
                return;
            }

            LavKolonneliste(@schemaFilePath);

            TextWriter tw = File.CreateText(destinationFilename);
            foreach (string line in File.ReadAllLines(textBoxTabel.Text, Encoding.UTF8))
            {
                if (line.Trim().ToLower() == "</table>")
                {
                    break;
                }
                tw.WriteLine(line);
            }

            progressBar1.Maximum = richTextBoxID.Lines.Length;
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            foreach (string line in richTextBoxID.Lines)
            {
                progressBar1.Value++;
                if (line.Trim() == "") continue;

                string[] CSV_items = line.Trim().Split(';');

                tw.WriteLine("\t<row>");

                for (int i = 1; i < currentRowCount + 1; i++)
                {
                    bool fundet = false;
                    for (int CSV_No = 0; CSV_No < numList.Count; CSV_No++)
                    {
                        if (numList[CSV_No].Value == i)
                        {
                            fundet = true;
                            tw.WriteLine("\t\t" + "<c" + i.ToString() + ">" + CSV_items[CSV_No].Trim() + "</c" + i.ToString() + ">");
                        }
                    }

                    if (!fundet)
                    {
                        if (i == numericUpDownTekst.Value)
                        {
                            tw.WriteLine("\t\t" + "<c" + i.ToString() + ">" + textBoxStandardTekst.Text + "</c" + i.ToString() + ">");
                        }
                        else
                        {
                            if (colList[i - 1].dataType == "string")
                            {
                                if (colList[i - 1].nillable == "true")
                                {
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + " xsi:nil=\"true\"/>");
                                }
                                else if (colList[i - 1].nillable == "false")
                                {
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + ">" + "default" + "</c" + i.ToString() + ">");
                                }
                            }
                            else if (colList[i - 1].dataType == "date")
                            {
                                if (colList[i - 1].nillable == "true")
                                {
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + " xsi:nil=\"true\"/>");
                                }
                                else
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + ">" + "9999-12-31" + "</c" + i.ToString() + ">");
                            }
                            else if (colList[i - 1].dataType == "time")
                            {
                                if (colList[i - 1].nillable == "true")
                                {
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + " xsi:nil=\"true\"/>");
                                }
                                else
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + ">" + "23:59:59" + "</c" + i.ToString() + ">");
                            }
                            else if (colList[i - 1].dataType == "datetime")
                            {
                                if (colList[i - 1].nillable == "true")
                                {
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + " xsi:nil=\"true\"/>");
                                }
                                else
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + ">" + "9999-12-31T23:59:59" + "</c" + i.ToString() + ">");
                            }
                            else
                            {
                                if (colList[i - 1].nillable == "true")
                                {
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + " xsi:nil=\"true\"/>");
                                }
                                else if (colList[i - 1].nillable == "false")
                                {
                                    tw.WriteLine("\t\t" + "<c" + i.ToString() + ">" + "" + "</c" + i.ToString() + ">");
                                }
                            }
                        }
                    }
                }

                tw.WriteLine("\t</row>");
            }

            tw.WriteLine("</table>");
            tw.Close();

            progressBar1.Visible = false;
            MessageBox.Show("Done");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            buttonLæsCSV.Enabled = false;
            buttonTilføjRækker.Enabled = false;

            richTextBoxXML.Clear();
            openFileDialog1.Filter = "XML|*.xml|Tekst|*.*";
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                textBoxTabel.Text = openFileDialog1.FileName;

                currentRowCount = GetColCount(textBoxTabel.Text);

                richTextBoxXML.Clear();
                int counter = 0;
                foreach (string line in File.ReadAllLines(textBoxTabel.Text, Encoding.Default))
                {
                    richTextBoxXML.AppendText(line + "\r\n");
                    counter++;
                    if (counter > 1000) break;
                }

                buttonLæsCSV.Enabled = true;
                buttonTilføjRækker.Enabled = true;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
        }

        private void LavKolonneliste(string xsdPath)
        {
            colList = new List<columnClass>();

            foreach (string line in File.ReadAllLines(@xsdPath))
            {
                if (line.Contains("name=\"c"))
                {
                    int start = line.IndexOf("name=\"c");
                    int slut = line.IndexOf("\"", start + 7);

                    string cNum = line.Substring(start + 7, slut - (start + 7));

                    columnClass c = new columnClass();
                    c.colNr = int.Parse(cNum);

                    if (line.ToLower().Contains("nillable=\"true\"") == true)
                        c.nillable = "true";
                    else
                        c.nillable = "false";

                    if (line.ToLower().Contains("type=\"xs:integer\"") == true)
                        c.dataType = "integer";
                    else if (line.ToLower().Contains("type=\"xs:string\"") == true)
                        c.dataType = "string";
                    else if (line.ToLower().Contains("type=\"xs:decimal\"") == true)
                        c.dataType = "decimal";
                    else if (line.ToLower().Contains("type=\"xs:float\"") == true)
                        c.dataType = "float";
                    else if (line.ToLower().Contains("type=\"xs:double\"") == true)
                        c.dataType = "double";
                    else if (line.ToLower().Contains("type=\"xs:boolean\"") == true)
                        c.dataType = "boolean";
                    else if (line.ToLower().Contains("type=\"xs:date\"") == true)
                        c.dataType = "date";
                    else if (line.ToLower().Contains("type=\"xs:time\"") == true)
                        c.dataType = "time";
                    else if (line.ToLower().Contains("type=\"xs:datetime\"") == true)
                        c.dataType = "datetime";
                    else if (line.ToLower().Contains("type=\"xs:duration\"") == true)
                        c.dataType = "duration";
                    else MessageBox.Show("Kunne ikke bestemme datatype!");

                    colList.Add(c);
                }
            }
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
        }

        private bool isKolonnevalgValid()
        {
            SortedList<decimal, string> sint = new SortedList<decimal, string>();

            sint.Add(numericUpDownTekst.Value, "sdfsdf");

            foreach (NumericUpDown num in numList)
            {
                try
                {
                    sint.Add(num.Value, "sdfsdf");
                }
                catch
                {
                    return (false);
                }
            }
            return (true);
        }

        #endregion
    }
}