using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;



namespace WindowsForms
{
    public partial class Veditor : Form
    {

        public Veditor()
        {
            InitializeComponent();
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
                }
            }
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
    }
}
