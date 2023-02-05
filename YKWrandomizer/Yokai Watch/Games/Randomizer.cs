using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using YKWrandomizer.Tool;
using YKWrandomizer.Logic;
using YKWrandomizer.Yokai_Watch.Res;
using YKWrandomizer.Yokai_Watch.Games.YW1;
using YKWrandomizer.Yokai_Watch.Games.YW2;
using YKWrandomizer.Yokai_Watch.Games.YW3;

namespace YKWrandomizer.Yokai_Watch.Games
{
    public class Randomizer
    {
        public string Name { get; set; }

        public string NameCode { get; set; }

        public string RomfsPath { get; set; }

        public int TribeCount { get; set; }

        public Dictionary<uint, string> YokaiScoutable { get; set; }

        public Dictionary<uint, string> YokaiStatic { get; set; }

        public Dictionary<uint, string> YokaiBoss { get; set; }

        public Dictionary<uint, string> Attacks { get; set; }

        public Dictionary<uint, string> Techniques { get; set; }

        public Dictionary<uint, string> Inspirits { get; set; }

        public Dictionary<uint, string> Soultimates { get; set; }

        public Dictionary<uint, string> Skills { get; set; }

        public Dictionary<uint, string> Items { get; set; }

        public Dictionary<uint, string> Souls { get; set; }

        public Dictionary<string, List<uint>> YokaiGiven { get; set; }

        public Dictionary<string, LocalFile> LocalFiles { get; set; }

        public string FaceIconFolder { get; set; }

        public RandomNumber Seed = new RandomNumber();

        public List<Yokai> GetYokais()
        {
            List<Yokai> output = new List<Yokai>();

            DataReader charabaseReader = new DataReader(LocalFiles["charabase"].Data);
            DataReader charaparamReader = new DataReader(LocalFiles["charaparam"].Data);

            charaparamReader.Seek(0x18);
            int yokaiCount = charaparamReader.ReadInt32();

            for (int i = 0; i < yokaiCount; i++)
            {
                Yokai yokai = new Yokai();
                ICharabase charabase = null;
                ICharaparam charaparam = null;

                if (NameCode == "YW1")
                {
                    charaparam = new YW1Charaparam();
                    charaparam.Offset = (uint)charaparamReader.BaseStream.Position;
                    charaparam.Read(new DataReader(charaparamReader.ReadBytes(0xE0)));

                    // Try to find charabase data
                    charabase = new YW1Charabase();
                    uint yokaiBasePosition = charabaseReader.FindUInt32BetweenRange(charaparam.BaseID, 0x2BEC, 0x68, 0xBAF0);
                    if (yokaiBasePosition < 0xBAF0)
                    {
                        // It's a valid Yokai
                        charabase.Offset = yokaiBasePosition - 0x0C;
                        charabase.Read(new DataReader(charabaseReader.GetSection(yokaiBasePosition - 0x0C, 0x6C)));
                    }
                }
                else if (NameCode == "YW2")
                {
                    charaparam = new YW2Charaparam();
                    charaparam.Offset = (uint)charaparamReader.BaseStream.Position;
                    charaparam.Read(new DataReader(charaparamReader.ReadBytes(0xE8)));

                    // Try to find charabase data
                    charabase = new YW2Charabase();
                    uint yokaiBasePosition = charabaseReader.FindUInt32BetweenRange(charaparam.BaseID, 0x9388, 0x74, 0x1ADAC);
                    if (yokaiBasePosition < 0x1ADAC)
                    {
                        // It's a valid Yokai
                        charabase.Offset = yokaiBasePosition - 0x0C;
                        charabase.Read(new DataReader(charabaseReader.GetSection(yokaiBasePosition - 0x0C, 0x78)));
                    }
                } 
                else if (NameCode == "YW3")
                {

                }

                if (charabase != null & charaparam != null)
                {
                    yokai.Charaparam = charaparam;
                    yokai.Charabase = charabase;

                    if (YokaiScoutable.ContainsKey(charaparam.ParamID))
                    {
                        if (yokai.Charaparam.FoodID == 0x00)
                        {
                            yokai.Charaparam.FoodID = 0x00654331;
                        }

                        if (yokai.Charaparam.MedaliumOffset == 0x00)
                        {
                            yokai.Charaparam.MedaliumOffset = 0x01;
                        }

                        yokai.Name = YokaiScoutable[charaparam.ParamID];
                        yokai.IsBoss = false;
                        yokai.IsStatic = false;
                        yokai.IsScoutable = true;
                        output.Add(yokai);
                    }
                    else if (YokaiBoss.ContainsKey(charaparam.ParamID))
                    {
                        yokai.Name = YokaiBoss[charaparam.ParamID];
                        yokai.IsBoss = true;
                        yokai.IsStatic = false;
                        yokai.IsScoutable = false;
                        output.Add(yokai);
                    }
                    else if (YokaiStatic.ContainsKey(charaparam.ParamID))
                    {
                        yokai.Name = YokaiStatic[charaparam.ParamID];
                        yokai.IsBoss = false;
                        yokai.IsStatic = true;
                        yokai.IsScoutable = false;
                        output.Add(yokai);
                    }
                }
            }

            charabaseReader.Close();
            charaparamReader.Close();

            return output;
        }

        public void SaveYokais(List<Yokai> yokais)
        {
            // Initialise Face Icon Map
            Bitmap faceIcon = new Bitmap(Image.FromStream(new ResourceReader(NameCode.ToLower() + "_face_icon.png").GetResourceStream()));

            DataWriter charabaseWriter = new DataWriter(LocalFiles["charabase"].Data);
            DataWriter charaparamWriter = new DataWriter(LocalFiles["charaparam"].Data);

            for (int i = 0; i < yokais.Count; i++)
            {
                yokais[i].Charabase.Write(charabaseWriter);
                yokais[i].Charaparam.Write(charaparamWriter);

                // Draw Medal
                if (yokais[i].IsScoutable == true)
                {
                    Image newMedal = null;
                    int tribe = 0;

                    // Get tribe
                    if (NameCode == "YW1")
                    {
                        tribe = (yokais[i].Charaparam as YW1Charaparam).Tribe;
                    }
                    else
                    {
                        tribe = yokais[i].Charabase.Tribe;
                    }

                    if (yokais[i].Charabase.IsLegendary)
                    {
                        newMedal = Image.FromStream(new ResourceReader("y_medal_lege0" + tribe + ".png").GetResourceStream());
                    }
                    else if (yokais[i].Charabase.IsRare)
                    {
                        newMedal = Image.FromStream(new ResourceReader("y_medal_rare0" + tribe + ".png").GetResourceStream());
                    }
                    else
                    {
                        newMedal = Image.FromStream(new ResourceReader("y_medal_nml0" + tribe + ".png").GetResourceStream());
                    }

                    faceIcon = Draw.DrawImage(faceIcon, yokais[i].Charabase.Medal.X * 44, yokais[i].Charabase.Medal.Y * 44, newMedal);
                }
            }

            charabaseWriter.Close();
            charaparamWriter.Close();
            faceIcon.Save(RomfsPath + FaceIconFolder, ImageFormat.Png);
        }

        public List<Evolution> GetEvolutions(List<Yokai> yokai)
        {
            List<Evolution> output = new List<Evolution>();

            DataReader charaparamReader = new DataReader(LocalFiles["charaparam"].Data);
            uint evolutionOffset = charaparamReader.FindUInt32BetweenRange(0x1448C0EF, 0x00, (uint)charaparamReader.Length);

            if (evolutionOffset < (uint)charaparamReader.Length)
            {
                charaparamReader.Seek(evolutionOffset - 0x04);

                int evolutionCount = charaparamReader.ReadInt32();
                for (int i = 0; i < evolutionCount; i++)
                {
                    charaparamReader.Skip(0x08);

                    int level = charaparamReader.ReadInt32();
                    UInt32 evolveTo = charaparamReader.ReadUInt32();
                    UInt32 baseYokai = yokai.FirstOrDefault(x => x.Charaparam.EvolveOffset == i).Charaparam.ParamID;

                    output.Add(new Evolution(baseYokai, evolveTo, level));
                }
            }

            charaparamReader.Close();

            return output;
        }

        public void SaveEvolutions(List<Evolution> evolutions)
        {
            DataReader charaparamReader = new DataReader(LocalFiles["charaparam"].Data);
            DataWriter charaparamWriter = new DataWriter(LocalFiles["charaparam"].Data);

            uint evolutionOffset = charaparamReader.FindUInt32BetweenRange(0x1448C0EF, 0x00, (uint)charaparamReader.Length);

            if (evolutionOffset < (uint)charaparamReader.Length)
            {
                charaparamReader.Seek(evolutionOffset - 0x04);
                int evolutionCount = charaparamReader.ReadInt32();
                charaparamWriter.Seek((uint)charaparamReader.BaseStream.Position);

                for (int i = 0; i < evolutionCount; i++)
                {
                    charaparamWriter.Skip(0x0C);
                    charaparamWriter.WriteUInt32(evolutions[i].EvolveTo);
                }
            }

            charaparamReader.Close();
            charaparamWriter.Close();
        }

        public List<Fusion> GetFusions()
        {
            List<Fusion> output = new List<Fusion>();

            DataReader combineconfigReader = new DataReader(LocalFiles["combineconfig"].Data);

            combineconfigReader.Seek(0x18);
            int fusionCount = combineconfigReader.ReadInt32();

            for (int i = 0; i < fusionCount; i++)
            {
                combineconfigReader.Skip(0x0C);
                UInt32 baseYokai = combineconfigReader.ReadUInt32();
                combineconfigReader.Skip(0x04);
                UInt32 material = combineconfigReader.ReadUInt32();
                combineconfigReader.Skip(0x04);
                UInt32 final = combineconfigReader.ReadUInt32();

                if (NameCode == "YW1")
                {
                    combineconfigReader.Skip(0x04);
                } else
                {
                    combineconfigReader.Skip(0x08);
                }

                // Valid fusion
                if (YokaiScoutable.ContainsKey(baseYokai))
                {
                    output.Add(new Fusion(baseYokai, material, final));
                }
            }

            combineconfigReader.Close();

            return output;
        }

        public void SaveFusions(bool randomize, List<Fusion> fusions)
        {
            if (randomize)
            {
                DataReader combineconfigReader = new DataReader(LocalFiles["combineconfig"].Data);
                DataWriter combineconfigWriter = new DataWriter(LocalFiles["combineconfig"].Data);

                combineconfigReader.Seek(0x18);
                int fusionCount = combineconfigReader.ReadInt32();

                for (int i = 0; i < fusionCount; i++)
                {
                    long position = combineconfigReader.BaseStream.Position;

                    combineconfigReader.Skip(0x0C);
                    UInt32 baseYokai = combineconfigReader.ReadUInt32();
                    combineconfigReader.Skip(0x04);
                    UInt32 material = combineconfigReader.ReadUInt32();
                    combineconfigReader.Skip(0x04);
                    UInt32 final = combineconfigReader.ReadUInt32();

                    if (NameCode == "YW1")
                    {
                        combineconfigReader.Skip(0x04);
                    }
                    else
                    {
                        combineconfigReader.Skip(0x08);
                    }

                    int fusionIndex = fusions.FindIndex(x => x.BaseYokai == baseYokai && x.Material == material);
                    if (fusionIndex != -1)
                    {
                        combineconfigWriter.Seek((uint)position + 0x1C);
                        combineconfigWriter.WriteUInt32(fusions[fusionIndex].EvolveTo);
                    }
                }

                combineconfigReader.Close();
                combineconfigWriter.Close();
            }
        }

        public void RandomizeSoul(bool randomize)
        {
            if (randomize)
            {
                DataReader soulconfigReader = new DataReader(LocalFiles["soulconfig"].Data);
                DataWriter soulconfigWriter = new DataWriter(LocalFiles["soulconfig"].Data);

                // Get All Poition and Copy Souls List
                List<uint> position = soulconfigReader.FindAll(0x577EF718, 0x1C, (uint)soulconfigReader.Length);
                List<uint> yokaiID = Souls.Select(x => x.Key).ToList();

                for (int i = 0; i < position.Count - 1; i++)
                {
                    soulconfigWriter.Seek(position[i]);
                    soulconfigWriter.Skip(0x24);
                    int randomIndex = Seed.Next(0, yokaiID.Count);
                    soulconfigWriter.WriteUInt32(yokaiID[randomIndex]);
                    yokaiID.RemoveAt(randomIndex);
                }

                soulconfigReader.Close();
                soulconfigWriter.Close();
            }
        }

        public void RandomizeYokai(Dictionary<string, Option> options)
        {
            List<Yokai> yokais = GetYokais();
            List<Evolution> evolutions = GetEvolutions(yokais);
            List<Fusion> fusions = GetFusions();

            // Randomize Evolution
            if (options["groupBoxEvolution"].Name == "Random")
            {
                List<Yokai> scoutableYokai = yokais.Where(x => x.IsScoutable == true).ToList();
                scoutableYokai.RemoveAll(x => evolutions.FindIndex(y => y.BaseYokai == x.Charaparam.ParamID) != -1);

                for (int i = 0; i < evolutions.Count; i++)
                {
                    int randomNumber = Seed.Next(0, scoutableYokai.Count);
                    evolutions[i].EvolveTo = scoutableYokai[randomNumber].Charaparam.ParamID;
                    scoutableYokai.RemoveAt(randomNumber);
                }
            }

            // Randomize Fusion
            if (options["groupBoxFusion"].Name == "Random")
            {
                List<Yokai> scoutableYokai = yokais.Where(x => x.IsScoutable == true).ToList();
                scoutableYokai.RemoveAll(x => fusions.FindIndex(y => y.BaseYokai == x.Charaparam.ParamID) != -1);
                scoutableYokai.RemoveAll(x => fusions.FindIndex(y => y.Material == x.Charaparam.ParamID) != -1);

                for (int i = 0; i < fusions.Count; i++)
                {
                    int randomNumber = Seed.Next(0, scoutableYokai.Count);
                    fusions[i].EvolveTo = scoutableYokai[randomNumber].Charaparam.ParamID;
                    scoutableYokai.RemoveAt(randomNumber);
                }
            }

            // Randomize Yokai
            for (int i = 0; i < yokais.Count; i++)
            {
                Yokai currentYokai = yokais[i];

                if (currentYokai.IsScoutable == true)
                {
                    if (options["groupBoxTribe"].Name == "Random")
                    {
                        if (this.NameCode == "YW1")
                        {
                            (currentYokai.Charaparam as YW1Charaparam).Tribe = Seed.Next(1, TribeCount + 1);
                        }
                        else
                        {
                            currentYokai.Charabase.Tribe = Seed.Next(1, TribeCount + 1);
                        }
                    }

                    if (options["groupBoxRank"].Name == "Random")
                    {
                        currentYokai.Charabase.Rank = Seed.Next(0, 6);
                    }

                    if (options["groupBoxRarity"].Name == "Random")
                    {
                        currentYokai.Charabase.IsRare = Convert.ToBoolean(Seed.Probability(new int[] { 80, 20 }));
                    }

                    if (options["groupBoxStrongest"].Name == "Random")
                    {
                        if (!(this.NameCode == "YW3"))
                        {
                            int guardCount = Seed.Next(0, 3);
                            List<int> guardAttribute = Seed.GetNumbers(0, 6, guardCount);

                            int weakCount = Seed.Next(0, 3);
                            List<int> weakdAttribute = Seed.GetNumbers(0, 6, guardCount, guardAttribute);

                            for (int j = 0; j < 6; j++)
                            {
                                if (guardAttribute.Contains(j))
                                {
                                    (currentYokai.Charaparam as YW2Charaparam).AttributeDamage[j] = Seed.Next(2, 9) * 0.10f;
                                }
                                else if (weakdAttribute.Contains(j))
                                {
                                    (currentYokai.Charaparam as YW2Charaparam).AttributeDamage[j] = Seed.Next(12, 19) * 0.10f;
                                }
                                else
                                {
                                    (currentYokai.Charaparam as YW2Charaparam).AttributeDamage[j] = 0x00;
                                }
                            }
                        }
                        else
                        {
                            (currentYokai.Charaparam as YW3Charaparam).Strongest = (byte)Seed.Next(0, 8);
                        }
                    }

                    if (options["groupBoxWeakness"].Name == "Random")
                    {
                        if (this.NameCode == "YW3")
                        {
                            (currentYokai.Charaparam as YW3Charaparam).Weakness = (byte)Seed.Next(0, 8);
                        }
                    }

                    if (options["groupBoxBaseStat"].Name == "Swap")
                    {
                        if (this.NameCode == "YW1")
                        {
                            int[] tempStat = new int[5];
                            Array.Copy(tempStat, currentYokai.Charaparam.MinStat, 5);

                            currentYokai.Charaparam.MinStat[0] = tempStat[3];
                            currentYokai.Charaparam.MinStat[1] = tempStat[2];
                            currentYokai.Charaparam.MinStat[2] = tempStat[1];
                            currentYokai.Charaparam.MinStat[3] = tempStat[4];
                            currentYokai.Charaparam.MinStat[4] = tempStat[0];
                        }
                        else
                        {
                            int[] tempStatMin = new int[5];
                            int[] tempStatMax = new int[5];
                            Array.Copy(tempStatMin, currentYokai.Charaparam.MinStat, 5);
                            Array.Copy(tempStatMax, (currentYokai.Charaparam as YW2Charaparam).MaxStat, 5);

                            currentYokai.Charaparam.MinStat[0] = tempStatMin[3];
                            currentYokai.Charaparam.MinStat[1] = tempStatMin[2];
                            currentYokai.Charaparam.MinStat[2] = tempStatMin[1];
                            currentYokai.Charaparam.MinStat[3] = tempStatMin[4];
                            currentYokai.Charaparam.MinStat[4] = tempStatMin[0];

                            (currentYokai.Charaparam as YW2Charaparam).MaxStat[0] = tempStatMax[3];
                            (currentYokai.Charaparam as YW2Charaparam).MaxStat[1] = tempStatMax[2];
                            (currentYokai.Charaparam as YW2Charaparam).MaxStat[2] = tempStatMax[1];
                            (currentYokai.Charaparam as YW2Charaparam).MaxStat[3] = tempStatMax[4];
                            (currentYokai.Charaparam as YW2Charaparam).MaxStat[4] = tempStatMax[0];
                        }
                    }
                    else if (options["groupBoxBaseStat"].Name == "Random")
                    {
                        if (this.NameCode == "YW1")
                        {
                            for (int m = 0; m < 5; m++)
                            {
                                int baseStat = 0;

                                if (m == 0)
                                {
                                    baseStat = Seed.Next(19, 31);
                                }
                                else
                                {
                                    baseStat = Seed.Next(13, 26);
                                }

                                // Bonus
                                if (currentYokai.Charabase.IsRare && currentYokai.Charabase.IsLegendary == false)
                                {
                                    baseStat += 7;
                                }

                                if (currentYokai.Charabase.IsLegendary)
                                {
                                    baseStat += 14;
                                }

                                baseStat += currentYokai.Charabase.Rank;

                                currentYokai.Charaparam.MinStat[m] = baseStat;
                            }
                        }
                        else
                        {
                            int bonusRank = currentYokai.Charabase.Rank * 10;

                            for (int m = 0; m < 5; m++)
                            {
                                int minStat = 0;
                                int maxStat = 0;

                                if (m == 0)
                                {
                                    minStat = Seed.Next(23, 38);
                                    maxStat = Seed.Next(233, 280);
                                }
                                else
                                {
                                    minStat = Seed.Next(1, 24);
                                    maxStat = Seed.Next(43, 155);
                                }

                                // Bonus
                                if (currentYokai.Charabase.IsRare && currentYokai.Charabase.IsLegendary == false && currentYokai.Charabase.Tribe != 0x09)
                                {
                                    minStat += 7 * minStat / 100;
                                }

                                if (currentYokai.Charabase.IsLegendary && currentYokai.Charabase.Tribe != 0x09)
                                {
                                    minStat += 10 * minStat / 100;
                                }

                                if (currentYokai.Charabase.Tribe == 0x09)
                                {
                                    minStat += 25 * minStat / 100;
                                }

                                minStat += bonusRank * minStat / 100;
                                maxStat += bonusRank * minStat / 100;

                                (currentYokai.Charaparam as YW2Charaparam).MinStat[m] = minStat;
                                (currentYokai.Charaparam as YW2Charaparam).MaxStat[m] = maxStat;
                            }
                        }
                    }

                    if (options["groupBoxExperience"].Name == "Random")
                    {
                        currentYokai.Charaparam.ExperienceCurve = Seed.Next(0, 7); ;
                    }

                    if (options["groupBoxWaitTime"].Name == "Random")
                    {
                        if (this.NameCode == "YW3" && currentYokai.IsScoutable == true)
                        {
                            (currentYokai.Charaparam as YW3Charaparam).WaitTime = Seed.Next(1, 8);
                        }
                    }
                }

                if (options["groupBoxDrop"].Name == "Random")
                {
                    List<int> randomItems = Seed.GetNumbers(0, Items.Count, 2);
                    currentYokai.Charaparam.DropID[0] = Items.ElementAt(randomItems[0]).Key;
                    currentYokai.Charaparam.DropID[1] = Items.ElementAt(randomItems[1]).Key;
                }

                if (options["groupBoxAttack"].Name == "Random")
                {
                    currentYokai.Charaparam.AttackID = Attacks.ElementAt(Seed.Next(Attacks.Count)).Key;
                }
                if (options["groupBoxTechnique"].Name == "Random")
                {
                    currentYokai.Charaparam.TechniqueID = Techniques.ElementAt(Seed.Next(Techniques.Count)).Key;
                }
                if (options["groupBoxInspirit"].Name == "Random")
                {
                    currentYokai.Charaparam.InspiritID = Inspirits.ElementAt(Seed.Next(Inspirits.Count)).Key;
                }
                if (options["groupBoxSoultimateMove"].Name == "Random")
                {
                    currentYokai.Charaparam.SoultimateID = Soultimates.ElementAt(Seed.Next(Soultimates.Count)).Key;
                }
                if (options["groupBoxSkill"].Name == "Random")
                {
                    currentYokai.Charaparam.SkillID = Skills.ElementAt(Seed.Next(Skills.Count)).Key;
                }

                if (options["groupBoxMiscellaneous"].CheckBoxes["checkBoxScaleMoney"].Checked == true)
                {
                    if (currentYokai.IsScoutable == true && currentYokai.Charaparam.Money > 35)
                    {
                        currentYokai.Charaparam.Money = 5 + (currentYokai.Charabase.Rank + 1) * 6;
                    }
                }
                if (options["groupBoxMiscellaneous"].CheckBoxes["checkBoxScaleMoney"].Checked == true)
                {
                    if (currentYokai.IsScoutable == true && currentYokai.Charaparam.Experience > 46)
                    {
                        currentYokai.Charaparam.Experience = 26 + (currentYokai.Charabase.Rank + 1) * 4;
                    }
                }
                if (options["groupBoxMiscellaneous"].CheckBoxes["checkBoxYoCommunity"].Checked == true)
                {
                    if (this.NameCode == "YW3" && currentYokai.IsScoutable == true)
                    {
                        // Reset Community
                        (currentYokai.Charabase as YW3Charabase).IsClassic = false;
                        (currentYokai.Charabase as YW3Charabase).IsMerican = false;
                        (currentYokai.Charabase as YW3Charabase).IsDeva = false;
                        (currentYokai.Charabase as YW3Charabase).IsMystery = false;
                        (currentYokai.Charabase as YW3Charabase).IsTreasure = false;

                        // Get new Community
                        int randomIndex = Seed.Probability(new int[] { 79, 5, 13, 1, 1, 1 });

                        if (randomIndex == 1)
                        {
                            (currentYokai.Charabase as YW3Charabase).IsClassic = true;
                        }
                        else if (randomIndex == 2)
                        {
                            (currentYokai.Charabase as YW3Charabase).IsMerican = true;
                        }
                        else if (randomIndex == 3)
                        {
                            (currentYokai.Charabase as YW3Charabase).IsDeva = true;
                        }
                        else if (randomIndex == 4)
                        {
                            (currentYokai.Charabase as YW3Charabase).IsMystery = true;
                        }
                        else if (randomIndex == 5)
                        {
                            (currentYokai.Charabase as YW3Charabase).IsTreasure = true;
                        }
                    }
                }
            }

            // Fix Evolution & Fusions
            List<object> evofusions = new List<object>();
            evofusions.AddRange(evolutions);
            //evofusions.AddRange(fusions);
            for (int i = 0; i < evofusions.Count; i++)
            {
                Yokai currentYokai = null;
                Yokai oldYokai = null;

                if (evofusions[i] is Evolution)
                {
                    currentYokai = yokais.FirstOrDefault(x => (evofusions[i] as Evolution).EvolveTo == x.Charaparam.ParamID);
                    oldYokai = yokais.FirstOrDefault(x => (evofusions[i] as Evolution).BaseYokai == x.Charaparam.ParamID);
                }
                else
                {
                    currentYokai = yokais.FirstOrDefault(x => (evofusions[i] as Fusion).EvolveTo == x.Charaparam.ParamID);
                    oldYokai = yokais.FirstOrDefault(x => (evofusions[i] as Fusion).BaseYokai == x.Charaparam.ParamID);
                    Yokai materialYokai = yokais.FirstOrDefault(x => (evofusions[i] as Fusion).Material == x.Charaparam.ParamID);

                    if (materialYokai != null)
                    {
                        if (this.NameCode == "YW1")
                        {
                            (oldYokai.Charaparam as YW1Charaparam).Tribe = (materialYokai.Charaparam as YW1Charaparam).Tribe;
                        }
                        else
                        {
                            oldYokai.Charabase.Tribe = materialYokai.Charabase.Tribe;
                        }

                        oldYokai.Charabase.Rank = materialYokai.Charabase.Rank;
                        oldYokai.Charaparam.AttackID = materialYokai.Charaparam.AttackID;
                        oldYokai.Charaparam.InspiritID = materialYokai.Charaparam.InspiritID;
                    }
                }

                if (options["groupBoxTribe"].Name == "Swap")
                {

                    currentYokai.Charabase.Tribe = oldYokai.Charabase.Tribe;
                }

                if (options["groupBoxRank"].Name == "Swap")
                {
                    currentYokai.Charabase.Rank = Seed.Next(oldYokai.Charabase.Rank, 6);
                }

                if (options["groupBoxRarity"].Name == "Swap")
                {
                    currentYokai.Charabase.IsRare = oldYokai.Charabase.IsRare;
                }

                if (options["groupBoxStrongest"].Name == "Swap")
                {
                    if (!(this.NameCode == "YW3"))
                    {
                        (currentYokai.Charaparam as YW2Charaparam).AttributeDamage = (oldYokai.Charaparam as YW2Charaparam).AttributeDamage;
                    }
                    else
                    {
                        (currentYokai.Charaparam as YW3Charaparam).Strongest = (oldYokai.Charaparam as YW3Charaparam).Strongest;
                    }
                }

                if (options["groupBoxWeakness"].Name == "Swap")
                {
                    if (this.NameCode == "YW3")
                    {
                        (currentYokai.Charaparam as YW3Charaparam).Strongest = (oldYokai.Charaparam as YW3Charaparam).Weakness;
                    }
                }

                if (options["groupBoxBaseStat"].Name == "Random")
                {
                    if (currentYokai.IsScoutable == true)
                    {
                        if (this.NameCode == "YW1")
                        {
                            for (int m = 0; m < 5; m++)
                            {
                                int baseStat = oldYokai.Charaparam.MinStat[m];
                                if (m == 0)
                                {
                                    baseStat += Seed.Next(10, 21) + currentYokai.Charabase.Rank;
                                }
                                else
                                {
                                    baseStat += Seed.Next(5, 11) + currentYokai.Charabase.Rank;
                                }

                                currentYokai.Charaparam.MinStat[m] = baseStat;
                            }
                        }
                        else
                        {
                            int bonusRank = currentYokai.Charabase.Rank * 10;

                            for (int m = 0; m < 5; m++)
                            {
                                int minStat = oldYokai.Charaparam.MinStat[m];
                                int maxStat = (oldYokai.Charaparam as YW2Charaparam).MaxStat[m];
                                if (m == 0)
                                {
                                    minStat += Seed.Next(10, 21) + currentYokai.Charabase.Rank;
                                    maxStat += Seed.Next(10, 21) + currentYokai.Charabase.Rank * 5;
                                }
                                else
                                {
                                    minStat += Seed.Next(5, 11) + currentYokai.Charabase.Rank;
                                    maxStat += Seed.Next(10, 21) + currentYokai.Charabase.Rank * 5;
                                }

                                (currentYokai.Charaparam as YW2Charaparam).MinStat[m] = minStat;
                                (currentYokai.Charaparam as YW2Charaparam).MaxStat[m] = maxStat;
                            }
                        }
                    }
                }

                if (options["groupBoxWaitTime"].Name == "Swap")
                {
                    if (this.NameCode == "YW3")
                    {
                        (currentYokai.Charaparam as YW3Charaparam).WaitTime = (oldYokai.Charaparam as YW3Charaparam).WaitTime;
                    }
                }

                if (options["groupBoxAttack"].Name == "Swap")
                {

                    currentYokai.Charaparam.AttackID = oldYokai.Charaparam.AttackID;
                }
                if (options["groupBoxTechnique"].Name == "Swap")
                {

                    currentYokai.Charaparam.TechniqueID = oldYokai.Charaparam.TechniqueID;
                }
                if (options["groupBoxInspirit"].Name == "Swap")
                {

                    currentYokai.Charaparam.InspiritID = oldYokai.Charaparam.InspiritID;
                }
                if (options["groupBoxSoultimateMove"].Name == "Swap")
                {

                    currentYokai.Charaparam.SoultimateID = oldYokai.Charaparam.SoultimateID;
                }
            }

            // Save Yokai
            SaveYokais(yokais);
            SaveEvolutions(evolutions);
            SaveFusions(options["groupBoxFusion"].Name == "Random", fusions);
            RandomizeSoul(options["groupBoxSoul"].Name == "Random");
        }

        public void RandomizeBossStat(decimal percentageLevel)
        {
            if (percentageLevel == 0) return;

            List<Yokai> yokais = GetYokais();
            yokais = yokais.Where(x => x.IsBoss == true || x.IsStatic == true).ToList();

            // Randomize Yokai
            for (int i = 0; i < yokais.Count; i++)
            {
                Yokai currentYokai = yokais[i];

                for (int s = 0; s < 5; s++)
                {
                    currentYokai.Charaparam.MinStat[s] += Convert.ToInt32(percentageLevel * currentYokai.Charaparam.MinStat[s] / 100);

                    if (NameCode != "YW1")
                    {
                        (currentYokai.Charaparam as YW2Charaparam).MaxStat[s] += Convert.ToInt32(percentageLevel * (currentYokai.Charaparam as YW2Charaparam).MaxStat[s] / 100);
                    }
                }
            }

            SaveYokais(yokais);
        }

        public void RandomizeStatic(bool randomize, decimal percentageLevel)
        {
            if (randomize == false) return;

            // Initialize File Reader and File Writer
            DataReader commonencReader = new DataReader(LocalFiles["encounter"].Data);
            DataWriter commonencWriter = new DataWriter(LocalFiles["encounter"].Data);

            uint encounterOffset = commonencReader.FindUInt32BetweenRange(0xE43FA5D2, 0x00, (uint) commonencReader.Length);
            if (encounterOffset < (uint)commonencReader.Length)
            {
                commonencReader.Seek(encounterOffset - 0x04);

                int encounterCount = commonencReader.ReadInt32();
                for (int i = 0; i < encounterCount; i++)
                {
                    commonencReader.Skip(0x08);
                    commonencWriter.Seek((uint)commonencReader.BaseStream.Position);

                    uint yokaiID = commonencReader.ReadUInt32();
                    int yokaiLevel = commonencReader.ReadInt32();

                    // Randomize Static Yo-Kai
                    if (YokaiScoutable.ContainsKey(yokaiID))
                    {
                        commonencWriter.WriteUInt32(YokaiScoutable.ElementAt(Seed.Next(0, YokaiScoutable.Count)).Key);
                    } 
                    else if (YokaiStatic.ContainsKey(yokaiID))
                    {
                        commonencWriter.WriteUInt32(YokaiStatic.ElementAt(Seed.Next(0, YokaiStatic.Count)).Key);
                    } 
                    else
                    {
                        commonencWriter.Skip(0x04);
                    }

                    // Add Percentage Level Multiplicator
                    if (percentageLevel > 0)
                    {
                        if (yokaiLevel > 0)
                        {
                            int newLevel = yokaiLevel + Convert.ToInt32(yokaiLevel * percentageLevel / 100);
                            if (newLevel > 99)
                            {
                                newLevel = 99;
                            }
                            commonencWriter.WriteInt32(newLevel);
                        }
                    }

                    if (NameCode != "YW3")
                    {
                        commonencReader.Skip(0x14);
                    }
                    else
                    {
                        commonencReader.Skip(0x18);
                    }
                }
            }

            commonencReader.Close();
            commonencWriter.Close();
        }

        public virtual void RandomizeWild(bool randomize, decimal percentageLevel)
        {

        }

        public void RandomizeShop(bool randomize)
        {
            if (randomize == false) return;

            // Get All Shop Files
            string[] shopFolder = Directory.GetFiles(RomfsPath + @"\yw2_a_fa\data\res\shop\");

            foreach (string file in shopFolder)
            {
                // Exclude Invalid Shop Files
                if (Path.GetFileName(file).StartsWith("shop_shp"))
                {
                    // Initialize File Reader And File Writer
                    DataReader shopReader = new DataReader(File.ReadAllBytes(file));
                    DataWriter shopWriter = new DataWriter(file);

                    shopReader.Seek(0x1C);
                    int shopCount = shopReader.ReadInt32();
                    for (int i = 0; i < shopCount; i++)
                    {
                        shopReader.Skip(0x0c);
                        shopWriter.Seek((uint)shopReader.BaseStream.Position);

                        if (Items.ContainsKey(shopReader.ReadUInt32()))
                        {
                            shopWriter.WriteUInt32(Items.ElementAt(Seed.Next(0, Items.Count)).Key);
                        }

                        shopReader.Skip(0x24);
                    }

                    // Close Files
                    shopReader.Close();
                    shopWriter.Close();
                }
            }
        }

        public virtual void RandomizeTreasureBox(bool randomize)
        {
            if (randomize == false) return;

            // Find All Files Who Contains Treasure Box Entry
            string[] folders = Directory.GetDirectories(RomfsPath + @"\yw2_a_fa\data\res\map\").Select(Path.GetFileName).ToArray();
            foreach (string folder in folders)
            {
                if (File.Exists(RomfsPath + @"\yw2_a_fa\data\res\map\" + folder + @"\" + folder + "_tbox.cfg.bin"))
                {
                    // Initialize File Reader and File Writer
                    DataReader treasureBoxReader = new DataReader(File.ReadAllBytes(RomfsPath + @"\yw2_a_fa\data\res\map\" + folder + @"\" + folder + "_tbox.cfg.bin"));
                    DataWriter treasureBoxWriter = new DataWriter(RomfsPath + @"\yw2_a_fa\data\res\map\" + folder + @"\" + folder + "_tbox.cfg.bin");

                    // Find Start Byte
                    treasureBoxReader.Seek(0x18);
                    int unknowCount = treasureBoxReader.ReadInt16();
                    treasureBoxReader.Seek((uint)(28 + unknowCount * 24));
                    treasureBoxReader.Skip(0x10);
                    int boxCount = treasureBoxReader.ReadInt32();

                    // Randomize Treasure Box
                    for (int i = 0; i < boxCount; i++)
                    {
                        treasureBoxReader.Skip(8);
                        treasureBoxWriter.Seek((uint)treasureBoxReader.BaseStream.Position);

                        if (Items.ContainsKey(treasureBoxReader.ReadUInt32()))
                        {
                            treasureBoxWriter.WriteUInt32(Items.ElementAt(Seed.Next(0, Items.Count)).Key);
                        }

                        treasureBoxReader.Skip(0x10);
                    }

                    // Close File
                    treasureBoxReader.Close();
                    treasureBoxWriter.Close();
                }
            }
        }

        public void RandomizeCrankKai(bool randomize)
        {
            if (randomize == false) return;

            // Initialize File Reader And File Writer
            DataReader crankKaiReader = new DataReader(LocalFiles["crankai"].Data);
            DataWriter crankKaiWriter = new DataWriter(LocalFiles["crankai"].Data);

            crankKaiReader.Seek(0x18);
            int crankKaiCount = crankKaiReader.ReadInt32();
            for (int i = 0; i < crankKaiCount; i++)
            {
                crankKaiReader.Skip(0x10);
                crankKaiWriter.Seek((uint)crankKaiReader.BaseStream.Position);

                // Check If The ID is Item or Yokai
                UInt32 dropID = crankKaiReader.ReadUInt32();

                if (Items.ContainsKey(dropID))
                {
                    crankKaiWriter.WriteUInt32(Items.ElementAt(Seed.Next(0, Items.Count)).Key);
                }
                else if (YokaiScoutable.ContainsKey(dropID))
                {
                    crankKaiWriter.WriteUInt32(YokaiScoutable.ElementAt(Seed.Next(0, YokaiScoutable.Count)).Key);
                }

                crankKaiReader.Skip(0x20);
            }

            // Close Files
            crankKaiReader.Close();
            crankKaiWriter.Close();
        }

        public void RandomizeLegendary(bool randomize, bool randomizeRequirment)
        {
            if (randomize == false) return;

            // Initialize File Reader And File Writer
            DataReader legendReader = new DataReader(LocalFiles["legendconfig"].Data);
            DataWriter legendWriter = new DataWriter(LocalFiles["legendconfig"].Data);

            // Remove Legendary
            List<Yokai> yokais = GetYokais();
            for (int i = 0; i < yokais.Count; i++)
            {
                if (yokais[i].Charabase.IsLegendary == true)
                {
                    yokais[i].Charabase.IsLegendary = false;
                }
            }

            // Create New Legendary Yokais
            legendReader.Seek(0x18);
            int legendCount = legendReader.ReadInt32();
            legendWriter.Seek((uint)legendReader.BaseStream.Position);
            List<int> legendaryYokaisIndex = Seed.GetNumbers(0, YokaiScoutable.Count, legendCount);

            for (int i = 0; i < legendCount; i++)
            {
                legendWriter.Skip(0x10);
                legendWriter.WriteUInt32(yokais[legendaryYokaisIndex[i]].Charaparam.ParamID);
                yokais[legendaryYokaisIndex[i]].Charabase.IsLegendary = true;

                // Create New Medallium Requirment
                if (randomizeRequirment == true)
                {
                    // Create Requirment Yokai and Exclude Legend Yokai From The List
                    List<int> requirmentYokais = Seed.GetNumbers(0, YokaiScoutable.Count, 8, legendaryYokaisIndex);
                    for (int r = 0; r < 8; r++)
                    {
                        legendWriter.WriteUInt32(YokaiScoutable.ElementAt(requirmentYokais[r]).Key);
                        legendWriter.WriteInt32(0x01);
                    }

                    legendWriter.Skip(0x08);
                } else
                {
                    legendWriter.Skip(0x48);
                }
            }

            SaveYokais(yokais);

            // Close File
            legendReader.Close();
            legendWriter.Close();
        }

        public void RandomizeGiven(bool randomize)
        {
            if (randomize == false) return;

            // Randomize Given Yokais
            foreach (KeyValuePair<string, List<uint>> givenYokai in YokaiGiven)
            {
                for (int i = 0; i < givenYokai.Value.Count; i++)
                {
                    DataWriter givenWriter = new DataWriter(RomfsPath + givenYokai.Key);
                    givenWriter.Seek(givenYokai.Value[i]);
                    givenWriter.WriteUInt32(YokaiScoutable.ElementAt(Seed.Next(0, YokaiScoutable.Count)).Key);
                    givenWriter.Close();
                }
            }
        }

        public virtual void FixStory(bool randomize)
        {

        }
    }
}
