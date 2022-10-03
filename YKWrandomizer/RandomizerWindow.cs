using System;
using System.IO;
using System.Data;
using System.Linq;
using YKWrandomizer.Logic;
using System.Windows.Forms;
using YKWrandomizer.YokaiWatch;

namespace YKWrandomizer
{
    public partial class RandomizerWindow : Form
    {
        private YW Game = null;

        public RandomizerWindow()
        {
            InitializeComponent();
        }

        private int GroupRadioButtonSelectedIndex(GroupBox groupBox)
        {
            var radioButtons = groupBox.Controls.OfType<RadioButton>().OrderBy(x => x.Name);

            int checkedIndex = 0;

            foreach (var radioButton in radioButtons)
            {
                if (radioButton.Checked == true)
                {
                    return checkedIndex;
                }
                checkedIndex++;
            }

            return -1;
        }

        private void FolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();
            DialogResult result = ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (Directory.Exists(ofd.SelectedPath + "/yw1_a_fa/"))
                {
                    Game = new YW("yw1", ofd.SelectedPath);
                    this.Text = "Yo-Kai Watch 1 Randomizer";
                } 
                else
                {
                    MessageBox.Show("Unrecognized Game Folder");
                    this.Text = "Yo-Kai Watch Randomizer";
                    tabControl1.Enabled = false;
                    randomizeSaveToolStripMenuItem.Enabled = false;
                    return;
                }
            }

            tabControl1.Enabled = true;
            randomizeSaveToolStripMenuItem.Enabled = true;
        }

        private void RandomizeSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Game.RandomizeLegendary(radioButtonLegendaryYokai2.Checked, checkBoxAllowBossLegendaryYokai.Checked, checkBoxRequirment.Checked);
            Game.BossBaseStat(numericUpDownBossBaseStat.Value);
            Game.BossSwap(radioButtonBossSwap2.Checked, checkBoxScaleStat.Checked);
            Game.RandomizeYokais(
                GroupRadioButtonSelectedIndex(groupBoxTribe),
                GroupRadioButtonSelectedIndex(groupBoxRank),
                GroupRadioButtonSelectedIndex(groupBoxRarity),
                GroupRadioButtonSelectedIndex(groupBoxDrop),
                GroupRadioButtonSelectedIndex(groupBoxEvolution),
                GroupRadioButtonSelectedIndex(groupBoxSkill),
                GroupRadioButtonSelectedIndex(groupBoxAttack),
                GroupRadioButtonSelectedIndex(groupBoxTechnique),
                GroupRadioButtonSelectedIndex(groupBoxInspirit),
                GroupRadioButtonSelectedIndex(groupBoxSoultimateMove),
                GroupRadioButtonSelectedIndex(groupBoxBaseStat),
                Convert.ToInt32(checkBoxScaleMoney.Checked),
                Convert.ToInt32(checkBoxScaleEXP.Checked),
                Convert.ToInt32(checkBoxFriendly.Checked)
                );

            Game.RandomizeCrankKai(radioButtonCrankKai2.Checked, checkBoxAllowBossCrankKai.Checked);
            Game.RandomizeTreasureBox(radioButtonTreasureBox2.Checked);
            Game.RandomizeShop(radioButtonShop2.Checked);
            
            Game.RandomizeStatic(radioButtonStaticYokai2.Checked, numericUpDownStaticYokai.Value);
            Game.RandomizeGiven(radioButtonGivenYokai2.Checked, checkBoxAllowBossGiven.Checked);
            Game.RandomizeWild(radioButtonWild2.Checked, checkBoxAllowBossWild.Checked, checkBoxScoutAllYokai.Checked, numericUpDownWild.Value);

            DialogResult dialogResult = MessageBox.Show("Randomized successful!\nDo you want to export the log of random in .txt?", "Save random log", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Print Yokais
                string randomLog = "--Yokai Traits--\n";
                for (int i = 0; i< Game.Yokais.Count; i++)
                {
                    Yokai yokai = null;

                    // Guet The Yokai Order
                    if (i < 223)
                    {
                        yokai = Game.Yokais.ElementAt(Game.MedalliumOrder[i]).Value;
                    } 
                    else
                    {
                        yokai = Game.Yokais.ElementAt(i).Value;
                    }

                    // Exclude NPC & Unused YoKais
                    if (yokai.Status.Name != "NPC" & yokai.Status.Name != "Unused")
                    {
                        randomLog += string.Join("\n", new String[]
                        {
                            yokai.Name + ":",
                            "- Tribe: " + yokai.Tribe.Name,
                            "- Rank: " + yokai.Rank.Name,
                            "- Drop: [" + yokai.Drops[0].Name + "; " +  yokai.Drops[1].Name + "]",
                            "- Skill: " + yokai.Skill.Name,
                            "- Attack: " + yokai.Moveset[0].Name,
                            "- Technique: " + yokai.Moveset[1].Name,
                            "- Inspirit: " + yokai.Moveset[2].Name,
                            "- Soultimate: " + yokai.Moveset[3].Name,
                            "- Base Stat: [ HP: " + yokai.BaseStat[0] + ", Strength: " + yokai.BaseStat[1] + ", Spirit: " + yokai.BaseStat[2] + ", Defense: " + yokai.BaseStat[3]+ ", Speed: " + yokai.BaseStat[4] + " ]",
                            "- Status: " + yokai.Status.Name,
                            "\n",
                        });
                    }
                }

                // Print Evolutions
                randomLog += "--Evolutions--\n";
                for (int i = 0; i < Game.Yokais.Count; i++)
                {
                    Yokai yokai = null;

                    // Guet The Yokai Order
                    if (i < 223)
                    {
                        yokai = Game.Yokais.ElementAt(Game.MedalliumOrder[i]).Value;
                    }
                    else
                    {
                        yokai = Game.Yokais.ElementAt(i).Value;
                    }

                    if (yokai.Evolution != null)
                    {
                        randomLog += yokai.Name + " evolves at level " + yokai.Evolution.Level + " to " + yokai.Evolution.EvolutionTo.Name + "\n";
                    }
                }

                // Print Legendary
                randomLog += "\n--Legends Yokai--\n";
                for (int i = 0; i < Game.Yokais.Count; i++)
                {
                    Yokai yokai = null;

                    // Guet The Yokai Order
                    if (i < 223)
                    {
                        yokai = Game.Yokais.ElementAt(Game.MedalliumOrder[i]).Value;
                    }
                    else
                    {
                        yokai = Game.Yokais.ElementAt(i).Value;
                    }

                    if (yokai.IsLegendary == true)
                    {
                        randomLog += "- " + yokai.Name + "\n";
                    }
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save Random Log";
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, randomLog);
                    MessageBox.Show("Saved");
                }
            }
        }

        private void RadioButtonWild2_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = radioButtonWild2.Checked;
            numericUpDownWild.Enabled = radioButtonWild2.Checked;
            checkBoxAllowBossWild.Enabled = radioButtonWild2.Checked;
            checkBoxScoutAllYokai.Enabled = radioButtonWild2.Checked;

            if (radioButtonWild2.Checked == false)
            {
                numericUpDownWild.Value = 0;
                checkBoxAllowBossWild.Checked = false;
                checkBoxScoutAllYokai.Checked = true;
            }
        }

        private void RadioButtonBossSwap2_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxScaleStat.Enabled = radioButtonBossSwap2.Checked;

            if (radioButtonBossSwap2.Checked == false)
            {
                checkBoxScaleStat.Checked = false;
            }
        }

        private void RadioButtonGivenYokai2_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAllowBossGiven.Enabled = radioButtonGivenYokai2.Checked;

            if (radioButtonGivenYokai2.Checked == false)
            {
                checkBoxAllowBossGiven.Checked = false;
            }
        }

        private void RadioButtonStaticYokai2_CheckedChanged(object sender, EventArgs e)
        {
            label2.Enabled = radioButtonStaticYokai2.Checked;
            numericUpDownStaticYokai.Enabled = radioButtonStaticYokai2.Checked;

            if (radioButtonStaticYokai2.Checked == false)
            {
                numericUpDownStaticYokai.Value = 0;
            }
        }

        private void RadioButtonCrankKai2_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxAllowBossCrankKai.Enabled = radioButtonCrankKai2.Checked;

            if (radioButtonCrankKai2.Checked == false)
            {
                checkBoxAllowBossCrankKai.Checked = false;
            }
        }

        private void RadioButtonLegendaryYokai2_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxRequirment.Enabled = radioButtonLegendaryYokai2.Checked;
            checkBoxAllowBossLegendaryYokai.Enabled = radioButtonLegendaryYokai2.Checked;

            if (radioButtonLegendaryYokai2.Checked == false)
            {
                checkBoxRequirment.Checked = false;
                checkBoxAllowBossLegendaryYokai.Checked = false;
            }
        }
    }
}
