using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using YKWrandomizer.Tool;
using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games
{
    public class Randomizer
    {
        public string Name;

        public string RomfsPath;

        public Dictionary<byte, string> Tribes;

        public Dictionary<uint, Yokai> Yokais;

        public List<Fusion> Fusions;

        public List<Evolution> Evolutions;

        public Dictionary<uint, string> Attacks;

        public Dictionary<uint, string> Techniques;

        public Dictionary<uint, string> Inspirits;

        public Dictionary<uint, string> Soultimates;

        public Dictionary<uint, string> Skills;

        public Dictionary<uint, string> Items;

        public Dictionary<uint, string> Souls;

        public Dictionary<string, List<uint>> YokaiGiven;

        public Dictionary<string, string> FilesPath;

        public RandomNumber Seed = new RandomNumber();

        public List<Fusion> LoadFusion()
        {
            List<Fusion> output = new List<Fusion>();

            DataReader combineconfigReader = new DataReader(File.ReadAllBytes(FilesPath["combineconfig"]));

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

                if (Name == "Yo-Kai Watch 1")
                {
                    combineconfigReader.Skip(0x04);
                }
                else
                {
                    combineconfigReader.Skip(0x08);
                }

                // Valid fusion
                if (Yokais.ContainsKey(baseYokai))
                {
                    output.Add(new Fusion(baseYokai, material, final));
                }
            }

            combineconfigReader.Close();

            return output;
        }

        public List<Evolution> LoadEvolution()
        {
            List<Evolution> output = new List<Evolution>();

            DataReader charaparamReader = new DataReader(File.ReadAllBytes(FilesPath["charaparam"]));
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
                    UInt32 baseYokai = Yokais.FirstOrDefault(x => x.Value.EvolveOffset == i).Key;

                    output.Add(new Evolution(baseYokai, evolveTo, level));
                }
            }

            charaparamReader.Close();

            return output;
        }

        private void YokaiInheritance(Yokai oldYokai, Yokai newYokai, Dictionary<string, Option> options)
        {
            if (options["groupBoxTribe"].Name == "Swap")
            {
                newYokai.Tribe = oldYokai.Tribe;
            }

            if (options["groupBoxRank"].Name == "Swap")
            {
                newYokai.Rank = Seed.Next(oldYokai.Rank, 6);
            }

            if (options["groupBoxRarity"].Name == "Swap")
            {
                newYokai.Statut = oldYokai.Statut;
            }

            if (options["groupBoxStrongest"].Name == "Swap")
            {
                newYokai.Strongest = oldYokai.Strongest;
                newYokai.AttributeDamage = oldYokai.AttributeDamage;
            }

            if (options["groupBoxWeakness"].Name == "Swap")
            {
                newYokai.Weakness = oldYokai.Weakness;
            }

            if (options["groupBoxBaseStat"].Name == "Random")
            {
                for (int m = 0; m < 5; m++)
                {
                    int minStat = oldYokai.MinStat[m];
                    int maxStat = oldYokai.MaxStat[m];

                    if (m == 0)
                    {
                        minStat += Seed.Next(10, 21) + newYokai.Rank;
                        maxStat += Seed.Next(10, 21) + newYokai.Rank * 5;
                    }
                    else
                    {
                        minStat += Seed.Next(5, 11) + newYokai.Rank;
                        maxStat += Seed.Next(5, 11) + newYokai.Rank * 5;
                    }

                    newYokai.MinStat[m] = minStat;
                    newYokai.MaxStat[m] = maxStat;
                }
            }

            if (options["groupBoxWaitTime"].Name == "Swap")
            {
                newYokai.WaitTime = oldYokai.WaitTime;
            }

            if (options["groupBoxAttack"].Name == "Swap")
            {

                newYokai.AttackID = oldYokai.AttackID;
            }
            if (options["groupBoxTechnique"].Name == "Swap")
            {

                newYokai.TechniqueID = oldYokai.TechniqueID;
            }
            if (options["groupBoxInspirit"].Name == "Swap")
            {

                newYokai.InspiritID = oldYokai.InspiritID;
            }
            if (options["groupBoxSoultimateMove"].Name == "Swap")
            {
                newYokai.SoultimateID = oldYokai.SoultimateID;
            }
        }

        public void RandomizeYokai(Dictionary<string, Option> options)
        {
            foreach (Yokai yokai in Yokais.Values)
            {
                if (yokai.Statut.IsScoutable == true)
                {
                    if (options["groupBoxTribe"].Name != "Unchanged")
                    {
                        yokai.Tribe = Seed.Next(1, Tribes.Count-1);
                    }

                    if (options["groupBoxRank"].Name != "Unchanged")
                    {
                        yokai.Rank = Seed.Next(0, 6);
                    }

                    if (options["groupBoxRarity"].Name != "Unchanged")
                    {
                        yokai.Statut.IsRare = Convert.ToBoolean(Seed.Probability(new int[] { 80, 20 }));
                    }

                    if (options["groupBoxStrongest"].Name != "Unchanged")
                    {
                        // YKW1 - YKW2
                        int guardAttribute = Seed.Next(0, 6);
                        int weakAttribute = Seed.Next(0, 6, guardAttribute);

                        int guardDamage = Seed.Next(3, 8);
                        int weakDamage = 20 - guardDamage;

                        for (int j = 0; j < 6; j++)
                        {
                            if (j == guardAttribute)
                            {
                                yokai.AttributeDamage[j] = guardDamage * 0.10f;
                            }
                            else if (j == weakAttribute)
                            {
                                yokai.AttributeDamage[j] = weakDamage * 0.10f;
                            }
                            else
                            {
                                yokai.AttributeDamage[j] = 0x00;
                            }
                        }

                        // YKW3
                        yokai.Strongest = (byte)Seed.Next(0, 8);
                    }

                    if (options["groupBoxWeakness"].Name != "Unchanged")
                    {
                        yokai.Weakness = (byte)Seed.Next(0, 8);
                    }

                    if (options["groupBoxBaseStat"].Name == "Swap")
                    {
                        int[] tempStatMin = new int[5];
                        int[] tempStatMax = new int[5];
                        Array.Copy(tempStatMin, yokai.MinStat, 5);
                        Array.Copy(tempStatMax, yokai.MaxStat, 5);

                        List<int> shuffle = Seed.GetNumbers(0, 6, 5);
                        for (int i = 0; i < 5; i++)
                        {
                            yokai.MinStat[i] = tempStatMin[shuffle[i]];
                            yokai.MaxStat[i] = tempStatMax[shuffle[i]];
                        }
                    }
                    else if (options["groupBoxBaseStat"].Name == "Random")
                    {
                        if (this.Name == "Yo-Kai Watch 1")
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
                                if (yokai.Statut.IsRare && yokai.Statut.IsLegendary == false)
                                {
                                    baseStat += 7;
                                }

                                if (yokai.Statut.IsLegendary)
                                {
                                    baseStat += 14;
                                }

                                baseStat += yokai.Rank;

                                yokai.MinStat[m] = baseStat;
                            }
                        }
                        else
                        {
                            int bonusRank = yokai.Rank * 10;

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
                                if (yokai.Statut.IsRare && yokai.Statut.IsLegendary == false && yokai.Tribe != 0x09)
                                {
                                    minStat += 7 * minStat / 100;
                                }

                                if (yokai.Statut.IsLegendary && yokai.Tribe != 0x09)
                                {
                                    minStat += 10 * minStat / 100;
                                }

                                if (yokai.Tribe == 0x09)
                                {
                                    minStat += 25 * minStat / 100;
                                }

                                minStat += bonusRank * minStat / 100;
                                maxStat += bonusRank * minStat / 100;

                                yokai.MinStat[m] = minStat;
                                yokai.MaxStat[m] = maxStat;
                            }
                        }
                    }

                    if (options["groupBoxExperience"].Name == "Random")
                    {
                        yokai.ExperienceCurve = Seed.Next(0, 7); ;
                    }

                    if (options["groupBoxWaitTime"].Name == "Random")
                    {
                        yokai.WaitTime = Seed.Next(1, 8);
                    }

                    if (options["groupBoxMiscellaneous"].CheckBoxes["checkBoxYoCommunity"].Checked == true)
                    {
                        // Reset Community
                        yokai.Statut.IsClassic = false;
                        yokai.Statut.IsMerican = false;
                        yokai.Statut.IsDeva = false;
                        yokai.Statut.IsMystery = false;
                        yokai.Statut.IsTreasure = false;

                        // Get new Community
                        int randomIndex = Seed.Probability(new int[] { 79, 5, 13, 1, 1, 1 });

                        switch (randomIndex)
                        {
                            case 1:
                                yokai.Statut.IsClassic = true;
                                break;
                            case 2:
                                yokai.Statut.IsMerican = true;
                                break;
                            case 3:
                                yokai.Statut.IsDeva = true;
                                break;
                            case 4:
                                yokai.Statut.IsMystery = true;
                                break;
                            case 5:
                                yokai.Statut.IsTreasure = true;
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (yokai.Statut.IsScoutable == true || yokai.Statut.IsStatic == true)
                {
                    if (options["groupBoxAttack"].Name != "Unchanged")
                    {
                        yokai.AttackID = Attacks.ElementAt(Seed.Next(Attacks.Count)).Key;
                    }
                    if (options["groupBoxTechnique"].Name != "Unchanged")
                    {
                        yokai.TechniqueID = Techniques.ElementAt(Seed.Next(Techniques.Count)).Key;
                    }
                    if (options["groupBoxInspirit"].Name != "Unchanged")
                    {
                        yokai.InspiritID = Inspirits.ElementAt(Seed.Next(Inspirits.Count)).Key;
                    }
                    if (options["groupBoxSoultimateMove"].Name != "Unchanged")
                    {
                        yokai.SoultimateID = Soultimates.ElementAt(Seed.Next(Soultimates.Count)).Key;
                    }
                    if (options["groupBoxSkill"].Name == "Random")
                    {
                        yokai.SkillID = Skills.ElementAt(Seed.Next(Skills.Count)).Key;
                    }

                    if (options["groupBoxMiscellaneous"].CheckBoxes["checkBoxScaleMoney"].Checked == true)
                    {
                        if (yokai.Money > 35)
                        {
                            yokai.Money = 5 + (yokai.Rank + 1) * 6;
                        }
                    }
                    if (options["groupBoxMiscellaneous"].CheckBoxes["checkBoxScaleMoney"].Checked == true)
                    {
                        if (yokai.Experience > 46)
                        {
                            yokai.Experience = 26 + (yokai.Rank + 1) * 4;
                        }
                    }
                }

                if (options["groupBoxDrop"].Name == "Random")
                {
                    List<int> randomItems = Seed.GetNumbers(0, Items.Count, 2);
                    yokai.DropID[0] = Items.ElementAt(randomItems[0]).Key;
                    yokai.DropID[1] = Items.ElementAt(randomItems[1]).Key;
                }
            }
        }

        public void RandomizeFusion(Dictionary<string, Option> options)
        {
            if (options["groupBoxFusion"].Name == "Random")
            {
                List<UInt32> scoutableYokaiID = Yokais.Where(x => x.Value.Statut.IsScoutable == true 
                                                                && Fusions.FindIndex(y => y.BaseYokai == x.Key) == -1
                                                                && Fusions.FindIndex(y => y.Material == x.Key) == -1
                                                                ).Select(x => x.Key).ToList();

                for (int i = 0; i < Fusions.Count; i++)
                {
                    // Randomize Evolution
                    int randomNumber = Seed.Next(0, scoutableYokaiID.Count);
                    Fusions[i].EvolveTo = scoutableYokaiID[randomNumber];
                    scoutableYokaiID.RemoveAt(randomNumber);
                }
            }

            // Transmits some data of the base yokai to the new evolution
            for (int i = 0; i < Fusions.Count; i++)
            {  
                YokaiInheritance(Yokais[Fusions[i].BaseYokai], Yokais[Fusions[i].EvolveTo], options);
            }

            // Save Fusion
            DataReader combineconfigReader = new DataReader(File.ReadAllBytes(FilesPath["combineconfig"]));
            DataWriter combineconfigWriter = new DataWriter(FilesPath["combineconfig"]);

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

                if (Name == "Yo-Kai Watch 1")
                {
                    combineconfigReader.Skip(0x04);
                }
                else
                {
                    combineconfigReader.Skip(0x08);
                }

                int fusionIndex = Fusions.FindIndex(x => x.BaseYokai == baseYokai && x.Material == material);
                if (fusionIndex != -1)
                {
                    combineconfigWriter.Seek((uint)position + 0x1C);
                    combineconfigWriter.WriteUInt32(Fusions[fusionIndex].EvolveTo);
                }
            }

            combineconfigReader.Close();
            combineconfigWriter.Close();
        }

        public void RandomizeEvolution(Dictionary<string, Option> options)
        {
            if (options["groupBoxEvolution"].Name == "Random")
            {
                List<UInt32> scoutableYokaiID = Yokais.Where(x => x.Value.Statut.IsScoutable == true && Evolutions.FindIndex(y => y.BaseYokai == x.Key) == -1).Select(x => x.Key).ToList();

                for (int i = 0; i < Evolutions.Count; i++)
                {
                    // Randomize Evolution
                    int randomNumber = Seed.Next(0, scoutableYokaiID.Count);
                    Evolutions[i].EvolveTo = scoutableYokaiID[randomNumber];
                    scoutableYokaiID.RemoveAt(randomNumber);
                }
            }

            // Transmits some data of the base yokai to the new evolution
            for (int i = 0; i < Evolutions.Count; i++)
            {
                Yokai oldYokai = Yokais[Evolutions[i].BaseYokai];
                Yokai newYokai = Yokais[Evolutions[i].EvolveTo];
                YokaiInheritance(Yokais[Evolutions[i].BaseYokai], Yokais[Evolutions[i].EvolveTo], options);
            }

            // Save Evolution
            DataReader charaparamReader = new DataReader(File.ReadAllBytes(FilesPath["charaparam"]));
            DataWriter charaparamWriter = new DataWriter(FilesPath["charaparam"]);

            uint evolutionOffset = charaparamReader.FindUInt32BetweenRange(0x1448C0EF, 0x00, (uint)charaparamReader.Length);

            if (evolutionOffset < (uint)charaparamReader.Length)
            {
                charaparamReader.Seek(evolutionOffset - 0x04);
                int evolutionCount = charaparamReader.ReadInt32();
                charaparamWriter.Seek((uint)charaparamReader.BaseStream.Position);

                for (int i = 0; i < evolutionCount; i++)
                {
                    charaparamWriter.Skip(0x0C);
                    charaparamWriter.WriteUInt32(Evolutions[i].EvolveTo);
                }
            }

            charaparamReader.Close();
            charaparamWriter.Close();
        }

        public void RandomizeSoul(bool randomize)
        {
            if (randomize)
            {
                DataReader soulconfigReader = new DataReader(File.ReadAllBytes(FilesPath["soulconfig"]));
                DataWriter soulconfigWriter = new DataWriter(FilesPath["soulconfig"]);

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

        public void RandomizeBossStat(decimal percentageLevel)
        {
            if (percentageLevel == 0) return;

            Dictionary<UInt32, Yokai> yokais = Yokais.Where(x => x.Value.Statut.IsScoutable == false).ToDictionary(i => i.Key, i => i.Value);

            foreach (Yokai yokai in yokais.Values)
            {
                for (int s = 0; s < 5; s++)
                {
                    yokai.MinStat[s] += Convert.ToInt32(percentageLevel * yokai.MinStat[s] / 100);
                    yokai.MaxStat[s] += Convert.ToInt32(percentageLevel * yokai.MaxStat[s] / 100);
                }
            }
        }

        public void RandomizeStatic(bool randomize, decimal percentageLevel)
        {
            if (randomize == false) return;

            // Initialize File Reader and File Writer
            DataReader commonencReader = new DataReader(File.ReadAllBytes(FilesPath["encounter"]));
            DataWriter commonencWriter = new DataWriter(FilesPath["encounter"]);

            List<UInt32> scoutableYokai = Yokais.Where(x => x.Value.Statut.IsScoutable == true).Select(i => i.Key).ToList();
            List<UInt32> staticYokai = Yokais.Where(x => x.Value.Statut.IsStatic == true).Select(i => i.Key).ToList();

            uint encounterOffset = commonencReader.FindUInt32BetweenRange(0xE43FA5D2, 0x00, (uint)commonencReader.Length);
            if (encounterOffset < (uint)commonencReader.Length)
            {
                commonencReader.Seek(encounterOffset);
                commonencReader.Skip(0x10);

                int encounterCount = commonencReader.ReadInt32();
                for (int i = 0; i < encounterCount; i++)
                {
                    commonencReader.Skip(0x08);
                    commonencWriter.Seek((uint)commonencReader.BaseStream.Position);

                    uint yokaiID = commonencReader.ReadUInt32();
                    int yokaiLevel = commonencReader.ReadInt32();

                    // Randomize Static Yo-Kai
                    if (scoutableYokai.Contains(yokaiID))
                    {
                        commonencWriter.WriteUInt32(scoutableYokai[Seed.Next(0, scoutableYokai.Count)]);
                    }
                    else if (staticYokai.Contains(yokaiID))
                    {
                        commonencWriter.WriteUInt32(staticYokai[Seed.Next(0, staticYokai.Count)]);
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

                    if (Name != "Yo-Kai Watch 3")
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
            string[] shopFolder = Directory.GetFiles(FilesPath["shop"]);

            foreach (string file in shopFolder)
            {
                // Exclude Invalid Shop Files
                if (Path.GetFileName(file).StartsWith("shop_shp"))
                {
                    List<UInt32> itemsID = Items.Select(i => i.Key).ToList();

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
                            int randomIndex = Seed.Next(0, itemsID.Count);
                            shopWriter.WriteUInt32(itemsID[randomIndex]);
                            itemsID.RemoveAt(randomIndex);
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

        }

        public void RandomizeCrankKai(bool randomize)
        {
            if (randomize == false) return;

            // Initialize File Reader And File Writer
            DataReader crankKaiReader = new DataReader(File.ReadAllBytes(FilesPath["crankai"]));
            DataWriter crankKaiWriter = new DataWriter(FilesPath["crankai"]);

            List<UInt32> scoutableYokai = Yokais.Where(x => x.Value.Statut.IsScoutable == true).Select(i => i.Key).ToList();

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
                else if (scoutableYokai.Contains(dropID))
                {
                    crankKaiWriter.WriteUInt32(scoutableYokai[Seed.Next(0, scoutableYokai.Count)]);
                } else
                {
                    crankKaiWriter.Skip(0x04);
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
            DataReader legendReader = new DataReader(File.ReadAllBytes(FilesPath["legendconfig"]));
            DataWriter legendWriter = new DataWriter(FilesPath["legendconfig"]);

            // Remove Legendary
            foreach (Yokai yokai in Yokais.Values)
            {
                yokai.Statut.IsLegendary = false;
            }

            // Scoutable Yokai
            List<UInt32> scoutableYokai = Yokais.Where(x => x.Value.Statut.IsScoutable == true).Select(i => i.Key).ToList();

            // Create New Legendary Yokais
            legendReader.Seek(0x18);
            int legendCount = legendReader.ReadInt32();
            legendWriter.Seek((uint)legendReader.BaseStream.Position);
            List<int> legendaryYokaisIndex = Seed.GetNumbers(0, scoutableYokai.Count, legendCount);

            for (int i = 0; i < legendCount; i++)
            {
                legendWriter.Skip(0x10);
                legendWriter.WriteUInt32(scoutableYokai[legendaryYokaisIndex[i]]);
                Yokais[scoutableYokai[legendaryYokaisIndex[i]]].Statut.IsLegendary = true;

                // Create New Medallium Requirment
                if (randomizeRequirment == true)
                {
                    // Create Requirment Yokai and Exclude Legend Yokai From The List
                    List<int> requirmentYokais = Seed.GetNumbers(0, scoutableYokai.Count, 8, legendaryYokaisIndex);
                    for (int r = 0; r < 8; r++)
                    {
                        legendWriter.WriteUInt32(scoutableYokai[requirmentYokais[r]]);
                        legendWriter.WriteInt32(0x01);
                    }

                    if (Name == "Yo-Kai Watch 1")
                    {
                        legendWriter.Skip(0x04);
                    }
                    else
                    {
                        legendWriter.Skip(0x08);
                    }
                }
                else
                {
                    if (Name == "Yo-Kai Watch 1")
                    {
                        legendWriter.Skip(0x44);
                    } else
                    {
                        legendWriter.Skip(0x48);
                    }
                }
            }

            // Close File
            legendReader.Close();
            legendWriter.Close();
        }

        public void RandomizeGiven(bool randomize)
        {
            if (randomize == false) return;

            // Scoutable Yokai
            List<UInt32> scoutableYokai = Yokais.Where(x => x.Value.Statut.IsScoutable == true).Select(i => i.Key).ToList();

            // Randomize Given Yokais
            foreach (KeyValuePair<string, List<uint>> givenYokai in YokaiGiven)
            {
                for (int i = 0; i < givenYokai.Value.Count; i++)
                {
                    DataWriter givenWriter = new DataWriter(RomfsPath + givenYokai.Key);
                    givenWriter.Seek(givenYokai.Value[i]);
                    givenWriter.WriteUInt32(scoutableYokai[Seed.Next(0, scoutableYokai.Count)]);
                    givenWriter.Close();
                }
            }
        }

        public virtual void FixStory(bool randomize)
        {

        }

        public virtual void SaveYokai()
        {

        }

        public virtual string PrintYokai()
        {
            return "";
        }
    }
}
