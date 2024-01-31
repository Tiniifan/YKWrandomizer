using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YKWrandomizer.Yokai_Watch.Games.YW1;
using YKWrandomizer.Yokai_Watch.Games.YW2;
using YKWrandomizer.Yokai_Watch.Games.YW3;
using YKWrandomizer.Yokai_Watch.Games.YWB;
using YKWrandomizer.Yokai_Watch.Games.YWB2;

namespace YKWrandomizer
{
    public partial class NewProjectWindow : Form
    {
        public string GameCode { get; set; }

        public string RomfsPath { get; set; }

        public string LanguageCode { get; set; }

        public NewProjectWindow()
        {
            InitializeComponent();
        }

        private string GetLanguageCode(string language)
        {
            switch (gameComboBox.SelectedIndex)
            {
                case 0:
                    return YW1Support.AvailableLanguages[language];
                case 1:
                    return YW2Support.AvailableLanguages[language];
                case 2:
                    return YW3Support.AvailableLanguages[language];
                case 3:
                    return YWBSupport.AvailableLanguages[language];
                case 4:
                    return YWB2Support.AvailableLanguages[language];
                default:
                    return null;
            }
        }

        private void ProjectCanBeCreated()
        {
            if (languageComboBox.SelectedIndex != -1 && gameComboBox.SelectedIndex != -1 && romfsTextBox.Text != string.Empty)
            {
                openButton.Enabled = true;
            }
            else
            {
                openButton.Enabled = false;
            }
        }

        private void GameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gameComboBox.SelectedIndex != -1)
            {
                languageComboBox.Items.Clear();

                switch (gameComboBox.SelectedIndex)
                {
                    case 0:
                        languageComboBox.Items.AddRange(YW1Support.AvailableLanguages.Keys.ToArray());
                        break;
                    case 1:
                        languageComboBox.Items.AddRange(YW2Support.AvailableLanguages.Keys.ToArray());
                        break;
                    case 2:
                        languageComboBox.Items.AddRange(YW3Support.AvailableLanguages.Keys.ToArray());
                        break;
                    case 3:
                        languageComboBox.Items.AddRange(YWBSupport.AvailableLanguages.Keys.ToArray());
                        break;
                    case 4:
                        languageComboBox.Items.AddRange(YWB2Support.AvailableLanguages.Keys.ToArray());
                        break;
                }

                ProjectCanBeCreated();
            }
        }

        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProjectCanBeCreated();
        }

        private void RomfsTextBox_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            DialogResult result = ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                romfsTextBox.Text = ofd.SelectedPath;
            }

            ProjectCanBeCreated();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            bool projectCreated = false;

            string languageCode = GetLanguageCode(languageComboBox.Text);

            if (gameComboBox.SelectedIndex == 0)
            {
                projectCreated = File.Exists(romfsTextBox.Text + @"\yw1_a.fa");
                if (projectCreated)
                {
                    GameCode = "yw1";
                    RomfsPath = romfsTextBox.Text;
                    LanguageCode = languageCode;
                }
            }
            else if (gameComboBox.SelectedIndex == 1)
            {
                projectCreated = File.Exists(romfsTextBox.Text + @"\yw2_a.fa") && File.Exists(romfsTextBox.Text + @"\yw2_lg_" + languageCode + @".fa");
                if (projectCreated)
                {
                    GameCode = "yw2";
                    RomfsPath = romfsTextBox.Text;
                    LanguageCode = languageCode;
                }
            }
            else if (gameComboBox.SelectedIndex == 2)
            {
                projectCreated = File.Exists(romfsTextBox.Text + @"\yw_a.fa") && File.Exists(romfsTextBox.Text + @"\yw_lg_" + languageCode + @".fa");
                if (projectCreated)
                {
                    GameCode = "yw3";
                    RomfsPath = romfsTextBox.Text;
                    LanguageCode = languageCode;
                }
            }
            else if (gameComboBox.SelectedIndex == 3)
            {
                projectCreated = File.Exists(romfsTextBox.Text + @"\yw_a.fa") && File.Exists(romfsTextBox.Text + @"\ywb_lg_" + languageCode + @".fa");
                if (projectCreated)
                {
                    GameCode = "ywb";
                    RomfsPath = romfsTextBox.Text;
                    LanguageCode = languageCode;
                }
            }
            else if (gameComboBox.SelectedIndex == 4)
            {
                projectCreated = File.Exists(romfsTextBox.Text + @"\yw_a.fa");
                if (projectCreated)
                {
                    GameCode = "ywb2";
                    RomfsPath = romfsTextBox.Text;
                    LanguageCode = languageCode;
                }
            }

            if (projectCreated)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Cannot open this ExtractedRomfs folder due to a missing file.");
            }
        }
    }
}
