using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using YKWrandomizer.Tool;
using YKWrandomizer.Yokai_Watch.Games;
using YKWrandomizer.Yokai_Watch.Games.YW1;
using YKWrandomizer.Yokai_Watch.Games.YW2;
using YKWrandomizer.Yokai_Watch.Games.YW3;

namespace YKWrandomizer
{
    public partial class RandomizerWindow : Form
    {
        private Randomizer Randomizer = null;

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
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialog1.SelectedPath + "/yw1_a_fa/"))                  
                {
                    groupBoxWaitTime.Enabled = false;
                    checkBoxYoCommunity.Enabled = false;
                    groupBoxWeakness.Enabled = false;
                    groupBoxStrongest.Text = "Attribute damage";
                    //Randomizer = new YW1(folderBrowserDialog1.SelectedPath);
                    Randomizer = null;
                    MessageBox.Show("This game is not yet available to be randomize.");
                    return;
                }
                else if (Directory.Exists(folderBrowserDialog1.SelectedPath + "/yw2_a_fa/"))
                {
                    groupBoxGivenYokai.Enabled = false;
                    groupBoxWaitTime.Enabled = false;
                    checkBoxYoCommunity.Enabled = false;
                    groupBoxWeakness.Enabled = false;
                    groupBoxStrongest.Text = "Attribute damage";
                    Randomizer = new YW2(folderBrowserDialog1.SelectedPath);
                }
                else if (Directory.Exists(folderBrowserDialog1.SelectedPath + "/yw_a_fa/"))
                {
                    groupBoxWaitTime.Enabled = true;
                    checkBoxYoCommunity.Enabled = true;
                    groupBoxWeakness.Enabled = true;
                    groupBoxStrongest.Text = "Strongest";
                    //Randomizer = new YW3(folderBrowserDialog1.SelectedPath);
                    Randomizer = null;
                    MessageBox.Show("This game is not yet available to be randomize.");
                    return;
                }
                else
                {
                    Randomizer = null;
                    MessageBox.Show("Unrecognized Game Folder");
                }
            }

            if (Randomizer != null)
            {
                tabControl1.Enabled = true;
                randomizeSaveToolStripMenuItem.Enabled = true;
                this.Text = Randomizer.Name + " Randomizer";
            } 
            else
            {
                tabControl1.Enabled = false;
                randomizeSaveToolStripMenuItem.Enabled = false;
                this.Text = "YKWrandomizer";
            }
        }

        private void RandomizeSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Randomizer.RandomizeYokai(TabControlToDictOption(tabControl2));
            Randomizer.RandomizeBossStat(numericUpDownBossBaseStat.Value);
            Randomizer.RandomizeStatic(radioButtonStaticYokai2.Checked, numericUpDownStaticYokai.Value);
            Randomizer.RandomizeWild(radioButtonWild2.Checked, numericUpDownWild.Value);
            Randomizer.RandomizeShop(radioButtonShop2.Checked);
            Randomizer.RandomizeTreasureBox(radioButtonTreasureBox2.Checked);
            Randomizer.RandomizeCrankKai(radioButtonCrankKai2.Checked);
            Randomizer.RandomizeLegendary(radioButtonLegendaryYokai2.Checked, checkBoxRequirmentsLegendaryYokai.Checked);
            Randomizer.RandomizeGiven(radioButtonGiven2.Checked);
            Randomizer.FixStory(checkBoxAvoidBlocked.Checked);

            // Save
            foreach (LocalFile file in Randomizer.LocalFiles.Values)
            {
                File.WriteAllBytes(file.Path, file.Data);
            }

            MessageBox.Show("Randomized successful!");
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
