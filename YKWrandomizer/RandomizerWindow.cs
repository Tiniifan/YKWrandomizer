using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using YKWrandomizer.Tools;
using YKWrandomizer.Yokai_Watch.Games;
using YKWrandomizer.Level5.Archive.ARC0;
using YKWrandomizer.Yokai_Watch;
using YKWrandomizer.Yokai_Watch.Games.YW1;
using YKWrandomizer.Yokai_Watch.Games.YW2;
using YKWrandomizer.Yokai_Watch.Games.YW3;
using YKWrandomizer.Yokai_Watch.Games.YWB;
using YKWrandomizer.Yokai_Watch.Games.YWB2;

namespace YKWrandomizer
{
    public partial class RandomizerWindow : Form
    {
        private Randomizer Randomizer;

        public RandomizerWindow()
        {
            InitializeComponent();
        }

        private Option GroupBoxToRandomizerOption(GroupBox groupBox)
        {
            Option randomizerOption = new Option(groupBox.Controls.OfType<RadioButton>().OrderBy(x => x.Name).ToList());
            randomizerOption.CheckBoxes = groupBox.Controls.OfType<CheckBox>().ToDictionary(x => x.Name, x => x);
            randomizerOption.NumericUpDowns = groupBox.Controls.OfType<NumericUpDown>().ToDictionary(x => x.Name, x => x);

            return randomizerOption;
        }

        private Dictionary<string, Option> TabControlToDictOption(TabControl tabControl)
        {
            Dictionary<string, Option> options = new Dictionary<string, Option>();

            foreach (Control control in tabControl.Controls)
            {
                if (control is TabPage)
                {
                    foreach (Control subControl in control.Controls)
                    {
                        if (subControl is GroupBox)
                        {
                            options.Add(subControl.Name, GroupBoxToRandomizerOption(subControl as GroupBox));
                        }
                    }
                }
            }

            return options;
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProjectWindow newProjectWindow = new NewProjectWindow();

            if (newProjectWindow.ShowDialog() == DialogResult.OK)
            {
                IGame game = null;

                switch (newProjectWindow.GameCode)
                {
                    case "yw1":
                        game = new YW1(newProjectWindow.RomfsPath, newProjectWindow.LanguageCode);
                        break;
                    case "yw2":
                        game = new YW2(newProjectWindow.RomfsPath, newProjectWindow.LanguageCode);
                        break;
                    case "yw3":
                        game = new YW3(newProjectWindow.RomfsPath, newProjectWindow.LanguageCode);
                        break;
                    case "ywb":
                        game = new YWB(newProjectWindow.RomfsPath, newProjectWindow.LanguageCode);
                        break;
                    case "ywb2":
                        game = new YWB2(newProjectWindow.RomfsPath, newProjectWindow.LanguageCode);
                        break;
                }

                if (game is YW1)
                {
                    groupBoxRole.Enabled = false;
                    groupBoxSoul.Enabled = false;
                    groupBoxWeakness.Enabled = false;
                    groupBoxStrongest.Text = "Attribute damage";
                    groupBoxWaitTime.Enabled = false;
                    yokaiTabControl.TabPages.RemoveAt(3);
                    labelStarter1.Text = "Cadin";
                    labelStarter2.Visible = false;
                    comboBoxSetStarter2.Visible = false;
                }
                else if (game is YW2)
                {
                    groupBoxRole.Enabled = false;
                    groupBoxSoul.Enabled = false;
                    groupBoxWeakness.Enabled = false;
                    groupBoxStrongest.Text = "Attribute damage";
                    groupBoxWaitTime.Enabled = false;
                    yokaiTabControl.TabPages.RemoveAt(3);
                    labelStarter1.Text = "Jibanyan";
                    labelStarter2.Visible = false;
                    comboBoxSetStarter2.Visible = false;
                }
                else if (game is YW3)
                {
                    groupBoxRole.Enabled = true;
                    groupBoxWaitTime.Enabled = true;
                    groupBoxWeakness.Enabled = true;
                    groupBoxSoul.Enabled = false;
                    groupBoxStrongest.Text = "Strongest";
                    labelStarter1.Text = "Jibanyan";
                    labelStarter2.Text = "Usapyon";
                    labelStarter2.Visible = true;
                    comboBoxSetStarter2.Visible = true;
                }

                Randomizer = new Randomizer(game);
                tabControl1.Enabled = true;
                randomizeSaveToolStripMenuItem.Enabled = true;
                this.Text = game.Name + " Randomizer";

                comboBoxSetStarter1.Items.AddRange(Randomizer.GetPlayableYokai(false));
                comboBoxSetStarter2.Items.AddRange(comboBoxSetStarter1.Items.Cast<Object>().ToArray());
            }
        }

        private async void RandomizeSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label4.Text = "Randomize";
            label4.Visible = true;
            progressBar1.Visible = true;

            int totalTasks = 10;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = totalTasks;
            progressBar1.Value = 0;
            randomizeSaveToolStripMenuItem.Enabled = false;

            await Task.Run(() =>
            {
                Randomizer.RemoveUnscoutableYokai(checkBoxUnlockYokai.Checked);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.RandomizeLegendary(radioButtonLegendaryYokai2.Checked, checkBoxRequirmentsLegendaryYokai.Checked);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.SwapBosses(checkBoxSwapBosses.Checked, checkBoxStatScaling.Checked);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.RandomizeYokai(TabControlToDictOption(yokaiTabControl), numericUpDownBossBaseStat.Value);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.RandomizeStatic(radioButtonStaticYokai2.Checked, numericUpDownStaticYokai.Value);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.RandomizeWild(radioButtonWild2.Checked, numericUpDownWild.Value);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.RandomizeShop(radioButtonShop2.Checked);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.RandomizeTreasureBox(radioButtonTreasureBox2.Checked);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.RandomizeCrankKai(radioButtonCrankKai2.Checked);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                comboBoxSetStarter1.Invoke((Action)delegate
                {
                    comboBoxSetStarter2.Invoke((Action)delegate
                    {
                        Randomizer.RandomizeGiven(radioButtonGiven2.Checked, new int[] { comboBoxSetStarter1.SelectedIndex, comboBoxSetStarter2.SelectedIndex });
                    });
                });
                    progressBar1.Invoke((Action)delegate { progressBar1.Value++; });
            });

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = Path.GetFileName(openFileDialog1.FileName);
            saveFileDialog.Title = "Save Level 5 ARC0 file";
            saveFileDialog.Filter = "Level 5 ARC0 files (*.fa)|*.fa";
            saveFileDialog.InitialDirectory = openFileDialog1.InitialDirectory;
            label4.Text = "Save";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                progressBar1.Value = 0;

                if (openFileDialog1.FileName == saveFileDialog.FileName)
                {
                    string tempPath = @"./temp";
                    string fileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);

                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }

                    // Save
                    Randomizer.Game.Game.Save(tempPath + @"\" + fileName, progressBar1);

                    // Close File
                    Randomizer.Game.Game.Close();

                    if (File.Exists(openFileDialog1.FileName))
                    {
                        File.Delete(openFileDialog1.FileName);
                    }

                    File.Move(tempPath + @"\" + fileName, saveFileDialog.FileName);

                    // Re Open
                    Randomizer.Game.Game = new ARC0(new FileStream(saveFileDialog.FileName, FileMode.Open));
                } else
                {
                    Randomizer.Game.Game.Save(saveFileDialog.FileName, progressBar1);
                }

                MessageBox.Show("Saved!");
            }

            randomizeSaveToolStripMenuItem.Enabled = true;
            label4.Visible = false;
            progressBar1.Visible = false;
        }

        private void RadioButtonStaticYokai2_CheckedChanged(object sender, EventArgs e)
        {
            label2.Enabled = radioButtonStaticYokai2.Checked;
            numericUpDownStaticYokai.Enabled = radioButtonStaticYokai2.Checked;

            if (numericUpDownStaticYokai.Enabled == false)
            {
                numericUpDownStaticYokai.Value = 0;
            }
        }

        private void RadioButtonWild2_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = radioButtonWild2.Checked;
            numericUpDownWild.Enabled = radioButtonWild2.Checked;

            if (numericUpDownWild.Enabled == false)
            {
                numericUpDownWild.Value = 0;
            }
        }

        private void CheckBoxSwapBosses_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxStatScaling.Enabled = checkBoxSwapBosses.Checked;

            if (checkBoxStatScaling.Enabled == false)
            {
                checkBoxStatScaling.Checked = false;
            }
        }

        private void RadioButtonLegendaryYokai2_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxRequirmentsLegendaryYokai.Enabled = radioButtonLegendaryYokai2.Checked;

            if (radioButtonLegendaryYokai2.Checked == false)
            {
                checkBoxRequirmentsLegendaryYokai.Checked = false;
            }
        }

        private void RadioButtonEvolution2_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxCreateNewEvolution.Enabled = radioButtonEvolution2.Checked;

            if (radioButtonEvolution2.Checked == false)
            {
                checkBoxCreateNewEvolution.Checked = false;
            }
        }

        private void RadioButtonGiven2_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxSetStarter.Enabled = radioButtonGiven2.Checked;

            if (!radioButtonGiven2.Checked)
            {
                checkBoxSetStarter.Checked = false;
                comboBoxSetStarter1.SelectedIndex = -1;
                comboBoxSetStarter1.Text = "";
                labelStarter1.Visible = false;
                comboBoxSetStarter2.SelectedIndex = -1;
                comboBoxSetStarter2.Text = "";
                labelStarter2.Visible = false;
            }
        }

        private void CheckBoxSetStarter_CheckedChanged(object sender, EventArgs e)
        {
            labelStarter1.Enabled = checkBoxSetStarter.Checked;
            labelStarter2.Enabled = checkBoxSetStarter.Checked;
            comboBoxSetStarter1.Enabled = checkBoxSetStarter.Checked;
            comboBoxSetStarter2.Enabled = checkBoxSetStarter.Checked;

            if (checkBoxSetStarter.Checked)
            {
                comboBoxSetStarter1.Items.Clear();
                comboBoxSetStarter1.Items.AddRange(Randomizer.GetPlayableYokai(checkBoxUnlockYokai.Checked));
                comboBoxSetStarter2.Items.Clear();
                comboBoxSetStarter2.Items.AddRange(comboBoxSetStarter1.Items.Cast<Object>().ToArray());
            } else
            {
                comboBoxSetStarter1.SelectedIndex = -1;
                comboBoxSetStarter1.Text = "";
                comboBoxSetStarter2.SelectedIndex = -1;
                comboBoxSetStarter2.Text = "";
            }
        }

        private void CheckBoxUnlockYokai_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxSetStarter1.Items.Clear();
            comboBoxSetStarter1.Items.AddRange(Randomizer.GetPlayableYokai(checkBoxUnlockYokai.Checked));
            comboBoxSetStarter2.Items.Clear();
            comboBoxSetStarter2.Items.AddRange(comboBoxSetStarter1.Items.Cast<Object>().ToArray());
        }
    }
}
