using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Veditor.Properties;


namespace WindowsForms
{
    public partial class Veditor : Form
    {
        

        const String TEXTBOX_KEY = "textbox";
        const String NEW_FILE_TEXT = "New file";

        //Todo: Making these .NET settings
        private Dictionary<String, Object> settings = new Dictionary<string, object>();

        public Veditor()
        {
            InitializeComponent();

            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(150,17);
            tabControl1.Dock = DockStyle.Fill;

            settings.Add("font", null );
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

                            byte[] contents = new byte[virta.Length];
                            await virta.ReadAsync(contents, 0, (int)virta.Length);


                            TextBox new_textbox = new TextBox();
                            new_textbox.Text = Encoding.UTF8.GetString(contents);
                            new_textbox.Name = TEXTBOX_KEY;
                            new_textbox.Multiline = true;
                            new_textbox.ScrollBars = ScrollBars.Both;
                            new_textbox.Dock = DockStyle.Fill;


                            //Cutting folder from full file name
                            String fname = trimFilename(avausdialogi.FileName);

                            //Opening file to new tab
                            TabPage new_tab = AddTab(fname);
                            new_tab.Controls.Add(new_textbox);

                            new_textbox.Refresh();

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

        public String trimFilename(String filename)
        {

            const int MAX_LENGTH = 23;

            //Splitting the filename with \
            String[] split = filename.Split('\\');

            String trimmed_filename;

            
            //Only the filename
            trimmed_filename = split.Last();
            
            //Folder and filename
            //trimmed_filename = split[split.Length - 2 ] + '\\' + split[split.Length - 1];

            //Making trimmed filename max some characters long
            if(trimmed_filename.Length <= MAX_LENGTH)
            {
                return trimmed_filename;
            }
            else
            {
                return ".." + trimmed_filename.Substring(trimmed_filename.Length - MAX_LENGTH);
            }
        }

        public TabPage AddTab(String filename) {


            
            tabControl1.TabPages.Add(filename);

            var lastTabIndex = tabControl1.TabCount - 1;
            tabControl1.SelectedIndex = lastTabIndex;

            


            return tabControl1.SelectedTab;
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

            TextBox currentTextBox = tabControl1.SelectedTab.Controls.OfType<TextBox>().First();

            if (currentTextBox.Text.Length >= 0)
            {
                //Sulkee automaattisesti hakasulut '{}'
                if (e.KeyChar == '{')
                {
                    int kursori = currentTextBox.SelectionStart;
                    currentTextBox.Text = currentTextBox.Text.Insert(currentTextBox.SelectionStart, "}");
                    currentTextBox.SelectionStart = kursori;
                }

                //Sulkee automaattisesti sulut '()'
                if (e.KeyChar == '(')
                {
                    int kursori = currentTextBox.SelectionStart;
                    currentTextBox.Text = currentTextBox.Text.Insert(currentTextBox.SelectionStart, ")");
                    currentTextBox.SelectionStart = kursori;
                }

                //Sulkee ""
                if (e.KeyChar == '"')
                {
                    int kursori = currentTextBox.SelectionStart;
                    currentTextBox.Text = currentTextBox.Text.Insert(currentTextBox.SelectionStart, "\"");
                    currentTextBox.SelectionStart = kursori;
                }
            }
        }

        

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog font_dialog = new FontDialog();

            
            
            
            font_dialog.Font = Settings.Default.Font;

            font_dialog.ShowDialog();
            
            changeFont(font_dialog.Font);

            Settings.Default.Font = font_dialog.Font;

            Settings.Default.Save();
                
        }

        private void changeFont(Font font)
        {
            if(font != null)
            {
                foreach (TabPage tabpage in tabControl1.Controls.OfType<TabPage>())
                {
                    tabpage.Controls[TEXTBOX_KEY].Font = font;
                }
            }
            
        }

        private void saveAs()
        {
            FileStream virta = null;
            SaveFileDialog saveDialog = new SaveFileDialog();


            TextBox currentTextBox = tabControl1.SelectedTab.Controls.OfType<TextBox>().First();


            saveDialog.InitialDirectory = "c://";
            saveDialog.Filter = "txt files (*.txt)|*.txt";
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                if ((virta = (FileStream)saveDialog.OpenFile()) != null)
                {
                    byte[] fileContents = Encoding.UTF8.GetBytes(currentTextBox.Text);

                    virta.Write(fileContents, 0, fileContents.Length);
                    virta.Close();

                    currentTextBox.Name = saveDialog.FileName;
                    //Updating file by adding and removing
                    tabControl1.SelectedTab.Controls.RemoveByKey(TEXTBOX_KEY);
                    tabControl1.SelectedTab.Controls.Add(currentTextBox);
                }
            }
        }

        private void save()
        {
            //Current text box should always exist
            TextBox currentTextBox = tabControl1.SelectedTab.Controls.OfType<TextBox>().First();

            if (currentTextBox.Name != null)
            {
                FileStream virta = new FileStream(currentTextBox.Name,FileMode.Append);

                byte[] fileContents = Encoding.UTF8.GetBytes(currentTextBox.Text);
                virta.Write(fileContents, 0, fileContents.Length);
                virta.Close();

                
            }
            
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TextBox currentTextBox = tabControl1.SelectedTab.Controls.OfType<TextBox>().First();

            //If filename has not been added
            if (currentTextBox.Name != null)
            {
                saveAs();
            }
            else
            {
                save();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox new_textbox = new TextBox();
            new_textbox.Name = TEXTBOX_KEY;
            new_textbox.Multiline = true;
            new_textbox.ScrollBars = ScrollBars.Both;
            new_textbox.Dock = DockStyle.Fill;


            //Opening file to new tab
            TabPage new_tab = AddTab(NEW_FILE_TEXT);
            new_tab.Controls.Add(new_textbox);

            new_textbox.Refresh();
        }
    }
}
