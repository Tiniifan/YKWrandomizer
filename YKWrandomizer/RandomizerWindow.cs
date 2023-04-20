using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using YKWrandomizer.Tool;
using YKWrandomizer.Yokai_Watch.Games;
using YKWrandomizer.Level5.Archive.ARC0;
using YKWrandomizer.Yokai_Watch.Games.YW1;
using YKWrandomizer.Yokai_Watch.Games.YW2;
using YKWrandomizer.Yokai_Watch.Games.YW3;
using YKWrandomizer.Yokai_Watch.Randomizer;

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
            openFileDialog1.Filter = "Level 5 ARC0 files (*.fa)|*.fa";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                IGame game;
                string fileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);

                switch(fileName)
                {
                    case "yw1_a":
                        game = new YW1(openFileDialog1.FileName);
                        break;
                    case "yw2_a":
                        //game = new YW2(openFileDialog1.FileName);
                        game = null;
                        break;
                    case "yw_a":
                        //game = new YW3(openFileDialog1.FileName);
                        game = null;
                        break;
                    default:
                        game = null;
                        break;
                }

                if (game != null)
                {
                    if (game is YW1)
                    {
                        groupBoxGivenYokai.Enabled = true;
                        groupBoxWaitTime.Enabled = false;
                        checkBoxYoCommunity.Enabled = false;
                        groupBoxWeakness.Enabled = false;
                        groupBoxSoul.Enabled = false;
                        groupBoxStrongest.Text = "Attribute damage";
                    } else if (game is YW2)
                    {
                        groupBoxGivenYokai.Enabled = false;
                        groupBoxWaitTime.Enabled = false;
                        checkBoxYoCommunity.Enabled = false;
                        groupBoxWeakness.Enabled = false;
                        groupBoxSoul.Enabled = true;
                        groupBoxStrongest.Text = "Attribute damage";
                    } else if (game is YW3)
                    {
                        groupBoxGivenYokai.Enabled = true;
                        groupBoxWaitTime.Enabled = true;
                        checkBoxYoCommunity.Enabled = true;
                        groupBoxWeakness.Enabled = true;
                        groupBoxSoul.Enabled = true;
                        groupBoxStrongest.Text = "Strongest";
                    }

                    Randomizer = new Randomizer(game);
                    tabControl1.Enabled = true;
                    randomizeSaveToolStripMenuItem.Enabled = true;
                    this.Text = game.Name + " Randomizer";
                }
                else
                {
                    tabControl1.Enabled = false;
                    randomizeSaveToolStripMenuItem.Enabled = false;
                    this.Text = "YKWrandomizer";

                    MessageBox.Show("Unrecognized game");
                    return;
                }
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

                Randomizer.RandomizeLegendary(radioButtonLegendaryYokai2.Checked, checkBoxLockLegendary.Checked ,checkBoxRequirmentsLegendaryYokai.Checked);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.RandomizeYokai(TabControlToDictOption(tabControl2), numericUpDownBossBaseStat.Value);
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

                Randomizer.RandomizeGiven(radioButtonGiven2.Checked);
                progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

                Randomizer.FixStory(checkBoxAvoidBlocked.Checked);
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
    }
}
