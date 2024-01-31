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
                                    

                    // Tempory restriction
                    groupBoxDrop.Enabled = true;
                    checkBoxScaleMoney.Enabled = true;
                    checkBoxScaleEXP.Enabled = true;
                    groupBoxShop.Enabled = true;
                    groupBoxTreasureBox.Enabled = true;
                    groupBoxCrankKai.Enabled = true;
                }
                else if (game is YW2)
                {
                    groupBoxGivenYokai.Enabled = false;
                    groupBoxWaitTime.Enabled = false;
                    groupBoxWeakness.Enabled = false;
                    groupBoxSoul.Enabled = true;
                    groupBoxStrongest.Text = "Attribute damage";

                    // Tempory restriction
                    groupBoxDrop.Enabled = true;
                    checkBoxScaleMoney.Enabled = true;
                    checkBoxScaleEXP.Enabled = true;
                    groupBoxShop.Enabled = true;
                    groupBoxTreasureBox.Enabled = true;
                    groupBoxCrankKai.Enabled = true;
                }
                else if (game is YW3)
                {
                    groupBoxGivenYokai.Enabled = true;
                    groupBoxWaitTime.Enabled = true;
                    groupBoxWeakness.Enabled = true;
                    groupBoxSoul.Enabled = true;
                    groupBoxStrongest.Text = "Strongest";

                    // Tempory restriction
                    groupBoxDrop.Enabled = false;
                    checkBoxScaleMoney.Enabled = false;
                    checkBoxScaleEXP.Enabled = false;
                    groupBoxSoul.Enabled = false;
                    groupBoxShop.Enabled = false;
                    groupBoxTreasureBox.Enabled = false;
                    groupBoxCrankKai.Enabled = false;
                }

                Randomizer = new Randomizer(game);
                tabControl1.Enabled = true;
                randomizeSaveToolStripMenuItem.Enabled = true;
                this.Text = game.Name + " Randomizer";
            }
        }

        private void RandomizeSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label4.Text = "Randomize";
            label4.Visible = true;
            progressBar1.Visible = true;

            int totalTasks = 12;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = totalTasks;
            progressBar1.Value = 0;
            randomizeSaveToolStripMenuItem.Enabled = false;

            Randomizer.SwapBosses(checkBoxSwapBosses.Checked, checkBoxStatScaling.Checked);
            Randomizer.RandomizeYokai(TabControlToDictOption(yokaiTabControl), numericUpDownBossBaseStat.Value);
            Randomizer.RandomizeStatic(radioButtonStaticYokai2.Checked, numericUpDownStaticYokai.Value);
            Randomizer.RandomizeWild(radioButtonWild2.Checked, numericUpDownWild.Value);

            //await Task.Run(() =>
            //{
            //Randomizer.RemoveUnscoutableYokai(checkBoxUnlockYokai.Checked);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.RandomizeYokaiSoul(radioButtonSoul2.Checked);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.RandomizeLegendary(radioButtonLegendaryYokai2.Checked, checkBoxLockLegendary.Checked ,checkBoxRequirmentsLegendaryYokai.Checked);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.RandomizeYokai(TabControlToDictOption(yokaiTabControl), numericUpDownBossBaseStat.Value);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.RandomizeStatic(radioButtonStaticYokai2.Checked, numericUpDownStaticYokai.Value);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.RandomizeWild(radioButtonWild2.Checked, numericUpDownWild.Value);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.RandomizeShop(radioButtonShop2.Checked);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.RandomizeTreasureBox(radioButtonTreasureBox2.Checked);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.RandomizeCrankKai(radioButtonCrankKai2.Checked);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.RandomizeGiven(radioButtonGiven2.Checked);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.FixStory(checkBoxAvoidBlocked.Checked);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });

            //Randomizer.ForceUltraFixStory(forceUltraFixStoryCheckBox.Checked);
            //progressBar1.Invoke((Action)delegate { progressBar1.Value++; });
            //});

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

                DialogResult dialogResult = MessageBox.Show("Saved! Do you want to save your random log as .txt file?", "Export random log", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    SaveFileDialog randomSaveLogDialog = new SaveFileDialog();
                    randomSaveLogDialog.FileName = "random_log.txt";
                    randomSaveLogDialog.Title = "Save Random Output";
                    randomSaveLogDialog.Filter = "Text files (*.txt)|*.txt";
                    randomSaveLogDialog.InitialDirectory = openFileDialog1.InitialDirectory;

                    if (randomSaveLogDialog.ShowDialog() == DialogResult.OK)
                    {
                        //File.WriteAllText(randomSaveLogDialog.FileName, Randomizer.PrintRandom());
                        MessageBox.Show("Saved log!");
                    }
                }
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
            checkBoxLockLegendary.Enabled = radioButtonLegendaryYokai2.Checked;
            checkBoxRequirmentsLegendaryYokai.Enabled = radioButtonLegendaryYokai2.Checked;

            if (radioButtonLegendaryYokai2.Checked == false)
            {
                checkBoxLockLegendary.Checked = false;
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

        private void ForceUltraFixStoryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Warning: you've activated an option that will force certain yokai to appear in their requested area. Please enable this option only if you are blocked by a yo-net quest during the story, otherwise disable this option and let fix story enable.");
        }
    }
}
