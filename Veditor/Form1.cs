using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;



namespace WindowsForms
{
    public partial class Veditor : Form
    {

        public String current_file = null;

        public Veditor()
        {
            InitializeComponent();

            InitializeFonts();

            
        }
        private void InitializeFonts()
        {
            InstalledFontCollection ifc = new InstalledFontCollection();

            Single initialFontSize = 14.0f;

            FontFamily[] fontFamilies = ifc.Families;

            //Trying to find Consolas
            if ( Array.Exists( fontFamilies, element => element.Name.Equals("Consolas") ) )
            {
                textPanel.Font = new Font("Consolas", initialFontSize);
            }
            //Then Courier should exist
            else if (Array.Exists(fontFamilies, element => element.Name.Equals("Courier")))
            {
                textPanel.Font = new Font("Courier", initialFontSize);
            }
            

            

        }

        private async void OpenMenuItem_Click(object sender, EventArgs e)
        {
            Stream virta = null;
            OpenFileDialog avausdialogi = new OpenFileDialog();

            avausdialogi.InitialDirectory = "c://";
            avausdialogi.Filter = "txt files (*.txt)|*.txt";
            avausdialogi.FilterIndex = 2;
            avausdialogi.RestoreDirectory = true;

            //If dialog box opens
            if (avausdialogi.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((virta = avausdialogi.OpenFile()) != null)
                    {
                        using (virta)
                        {

                            byte[] result = new byte[virta.Length];
                            await virta.ReadAsync(result, 0, (int)virta.Length);

                            textPanel.Text = Encoding.UTF8.GetString(result);

                            current_file = avausdialogi.FileName;

                            textPanel.Refresh();

                            virta.Close();
                        }
                    }

                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
                }
            }

        }

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            string sulkemisvaroitus = "Haluatko varmasti lopettaa?";
            MessageBoxButtons button = MessageBoxButtons.YesNo;

            


            switch (e.CloseReason){
                case (CloseReason.WindowsShutDown):
                    Application.Exit();
                    break;
                default:
                    DialogResult result = MessageBox.Show(sulkemisvaroitus, "", button);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            break;
                        default:
                            e.Cancel = true;
                            break;
                    }
                    break;
            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            saveAs();

            
        }

        private void textPanel_KeyPressed(object sender, KeyPressEventArgs e)
        {
            string currentText = textPanel.Text;


            
            if (currentText.Length >= 0)
            {
                //Sulkee automaattisesti hakasulut '{}'
                if (e.KeyChar == '{')
                {
                    int kursori = textPanel.SelectionStart;
                    textPanel.Text = textPanel.Text.Insert(textPanel.SelectionStart, "}");
                    textPanel.SelectionStart = kursori;
                }

                //Sulkee automaattisesti sulut '()'
                if (e.KeyChar == '(')
                {
                    int kursori = textPanel.SelectionStart;
                    textPanel.Text = textPanel.Text.Insert(textPanel.SelectionStart, ")");
                    textPanel.SelectionStart = kursori;
                }

                //Sulkee ""
                if (e.KeyChar == '"')
                {
                    int kursori = textPanel.SelectionStart;
                    textPanel.Text = textPanel.Text.Insert(textPanel.SelectionStart, "\"");
                    textPanel.SelectionStart = kursori;
                }
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog font_dialog = new FontDialog();
            
            font_dialog.Font = textPanel.Font;

            font_dialog.ShowDialog();
            
            textPanel.Font = font_dialog.Font;
                
        }

        private void saveAs()
        {
            FileStream virta = null;
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.InitialDirectory = "c://";
            saveDialog.Filter = "txt files (*.txt)|*.txt";
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                if ((virta = (FileStream)saveDialog.OpenFile()) != null)
                {
                    byte[] fileContents = Encoding.UTF8.GetBytes(textPanel.Text);

                    virta.Write(fileContents, 0, fileContents.Length);
                    virta.Close();
                    Console.WriteLine(saveDialog.FileName);
                    current_file = saveDialog.FileName;
                }
            }
        }

        private void save()
        {
            if(current_file != null)
            {
                FileStream virta = new FileStream(current_file,FileMode.Append);

                byte[] fileContents = Encoding.UTF8.GetBytes(textPanel.Text);
                virta.Write(fileContents, 0, fileContents.Length);
                virta.Close();
            }
            
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(current_file == null)
            {
                saveAs();
            }
            else
            {
                save();
            }
        }
    }
}
