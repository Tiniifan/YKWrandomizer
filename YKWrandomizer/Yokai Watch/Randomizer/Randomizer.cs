using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using YKWrandomizer.Tool;
using YKWrandomizer.Level5.Archive.XPCK;
using YKWrandomizer.Yokai_Watch.Logic;
using YKWrandomizer.Yokai_Watch.Games;
using YKWrandomizer.Yokai_Watch.Games.YW1;
using YKWrandomizer.Yokai_Watch.Games.YW2;
using YKWrandomizer.Yokai_Watch.Games.YW3;

namespace YKWrandomizer.Yokai_Watch.Randomizer
{
    public class Randomizer
    {
        public IGame Game;

        private List<Yokai> Yokais;

        private List<Fusion> Fusions;

        private List<Evolution> Evolutions;

        private RandomNumber Seed = new RandomNumber();

        public Randomizer(IGame game)
        {
            Game = game;
            Yokais = game.GetYokais();
            Fusions = game.GetFusions();
            Evolutions = game.GetEvolutions(Yokais);
        }

        private void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Seed.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
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

        public void RemoveUnscoutableYokai(bool randomize)
        {
            if (randomize == false) return;

            List<Yokai> yokais = Yokais.Where(x => Game.YokaiUnscoutableAutorized.Contains(x.ParamID)).ToList();

            foreach (Yokai yokai in yokais)
            {
                yokai.Statut.IsScoutable = true;

                if (yokai.MedaliumOffset == 0)
                {
                    yokai.MedaliumOffset = 1;

                    if (Game is YW1)
                    {
                        yokai.ScoutableID = 0x13345632;
                    }
                    else if (Game is YW2)
                    {
                        yokai.ScoutableID = 0x00654331;
                    }
                }
            }
        }

        public void RandomizeLegendary(bool randomize, bool lockLegendary, bool randomizeRequirment)
        {
            if (randomize == false) return;

            List<LegendSeal> legendaries = Game.GetLegendaries();

            // Remove Legendary
            foreach (Yokai yokai in Yokais)
            {
                yokai.Statut.IsLegendary = false;
            }

            // Scoutable Yokai
            List<UInt32> scoutableYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true).Select(i => i.ParamID).ToList();

            // Randomize Legendary Yokais
            for (int i = 0; i < legendaries.Count; i ++)
            {
                int randomNumber = Seed.Next(0, scoutableYokaiID.Count);
                uint legendaryID = scoutableYokaiID[randomNumber];
                scoutableYokaiID.RemoveAt(randomNumber);

                legendaries[i].LegendaryParamID = legendaryID;

                // Update Yokai
                Yokai yokai = Yokais.FirstOrDefault(x => x.ParamID == legendaryID);
                yokai.Statut.IsLegendary = true;
                if (lockLegendary)
                {
                    yokai.ScoutableID = 0x00;
                    yokai.Statut.IsScoutable = false;
                }

                List<uint> requirmentYokais = new List<uint>();
                if (randomizeRequirment)
                {
                    // Create new Seal Requirment
                    for (int r = 0; r < 8; r++)
                    {
                        randomNumber = Seed.Next(0, scoutableYokaiID.Count);
                        requirmentYokais.Add(scoutableYokaiID[randomNumber]);
                        scoutableYokaiID.RemoveAt(randomNumber);
                    }

                    legendaries[i].RequirmentParamID = requirmentYokais.ToArray();
                }
            }

            // Save
            Game.SaveLegendaries(legendaries, true);
        }

        public void RandomizeYokai(Dictionary<string, Option> options, decimal percentageLevel)
        {
            // Yokai Scoutable and Static
            foreach (Yokai yokai in Yokais)
            {
                yokai.DropID = options["groupBoxDrop"].Name == "Random" ? new uint[] { Game.Items.ElementAt(Seed.GetNumbers(0, Game.Items.Count, 2)[0]).Key, Game.Items.ElementAt(Seed.GetNumbers(0, Game.Items.Count, 2)[1]).Key } : yokai.DropID;

                if (yokai.Statut.IsScoutable == true)
                {
                    yokai.Tribe = options["groupBoxTribe"].Name != "Unchanged" ? Seed.Next(1, Game.Tribes.Count - 1) : yokai.Tribe;
                    yokai.Rank = options["groupBoxRank"].Name != "Unchanged" ? Seed.Next(0, 6) : yokai.Rank;
                    yokai.Statut.IsRare = options["groupBoxRarity"].Name != "Unchanged" ? Convert.ToBoolean(Seed.Probability(new int[] { 80, 20 })) : yokai.Statut.IsRare;
                    yokai.Weakness = options["groupBoxWeakness"].Name != "Unchanged" ? (byte)Seed.Next(0, 8) : yokai.Weakness;
                    yokai.ExperienceCurve = options["groupBoxExperience"].Name == "Random" ? Seed.Next(0, 7) : yokai.ExperienceCurve;
                    yokai.WaitTime = options["groupBoxWaitTime"].Name == "Random" ? Seed.Next(1, 8) : yokai.WaitTime;

                    if (options["groupBoxStrongest"].Name != "Unchanged")
                    {
                        // YKW1 - YKW2
                        int[] attributes = Enumerable.Range(0, 6).ToArray();
                        int guardAttribute = Seed.Next(0, 6);
                        int weakAttribute = Seed.Next(0, 6, guardAttribute);
                        int guardDamage = Seed.Next(3, 8);
                        int weakDamage = 20 - guardDamage;

                        yokai.AttributeDamage = attributes.Select(j =>
                            j == guardAttribute ? guardDamage * 0.10f :
                            j == weakAttribute ? weakDamage * 0.10f : 0x00
                        ).ToArray();

                        // YKW3
                        yokai.Strongest = (byte)Seed.Next(0, 8);
                    }
                 
                    if (options["groupBoxBaseStat"].Name == "Swap")
                    {
                        Shuffle(yokai.MinStat);
                        if (yokai.MaxStat != null)
                        {
                            Shuffle(yokai.MaxStat);
                        }
                    }
                    else if (options["groupBoxBaseStat"].Name == "Random")
                    {
                        int bonusRank = yokai.Rank * 10;

                        for (int m = 0; m < 5; m++)
                        {
                            int minStat = 0;
                            int maxStat = 0;

                            if (Game.Name == "Yo-Kai Watch 1")
                            {
                                if (m == 0)
                                {
                                    minStat = Seed.Next(19, 31);
                                }
                                else
                                {
                                    minStat = Seed.Next(13, 26);
                                }

                                // Bonus
                                if (yokai.Statut.IsRare && !yokai.Statut.IsLegendary)
                                {
                                    minStat += 7;
                                }

                                if (yokai.Statut.IsLegendary)
                                {
                                    minStat += 14;
                                }

                                minStat += yokai.Rank;
                            }
                            else
                            {
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
                                if (yokai.Statut.IsRare && !yokai.Statut.IsLegendary && yokai.Tribe != 0x09)
                                {
                                    minStat += (int)(0.07 * minStat);
                                }

                                if (yokai.Statut.IsLegendary && yokai.Tribe != 0x09)
                                {
                                    minStat += (int)(0.1 * minStat);
                                }

                                if (yokai.Tribe == 0x09)
                                {
                                    minStat += (int)(0.25 * minStat);
                                }

                                minStat += (int)(bonusRank * 0.01 * minStat);
                                maxStat += (int)(bonusRank * 0.01 * minStat);
                            }

                            yokai.MinStat[m] = minStat;
                            yokai.MaxStat[m] = maxStat;
                        }
                    }

                    if (options["groupBoxMiscellaneous"].CheckBoxes["checkBoxYoCommunity"].Checked)
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
                    yokai.AttackID = options["groupBoxAttack"].Name != "Unchanged" ? Game.Attacks.ElementAt(Seed.Next(Game.Attacks.Count)).Key : yokai.AttackID;
                    yokai.TechniqueID = options["groupBoxTechnique"].Name != "Unchanged" ? Game.Techniques.ElementAt(Seed.Next(Game.Techniques.Count)).Key : yokai.TechniqueID;
                    yokai.InspiritID = options["groupBoxInspirit"].Name != "Unchanged" ? Game.Inspirits.ElementAt(Seed.Next(Game.Inspirits.Count)).Key : yokai.InspiritID;
                    yokai.SoultimateID = options["groupBoxSoultimateMove"].Name != "Unchanged" ? Game.Soultimates.ElementAt(Seed.Next(Game.Soultimates.Count)).Key : yokai.SoultimateID;
                    yokai.SkillID = options["groupBoxSkill"].Name == "Random" ? Game.Skills.ElementAt(Seed.Next(Game.Skills.Count)).Key : yokai.SkillID;
                    yokai.Money = options["groupBoxMiscellaneous"].CheckBoxes["checkBoxScaleMoney"].Checked && yokai.Money > 35 ? 5 + (yokai.Rank + 1) * 6 : yokai.Money;
                    yokai.Experience = options["groupBoxMiscellaneous"].CheckBoxes["checkBoxScaleMoney"].Checked && yokai.Experience > 46 ? 26 + (yokai.Rank + 1) * 4 : yokai.Experience;
                }
            }

            // Prevent bug from the story
            Game.FixYokai(Yokais);

            // Randomize Evolution
            if (options["groupBoxEvolution"].Name == "Random")
            {
                List<uint> scoutableYokaiID = null;

                if (options["groupBoxEvolution"].CheckBoxes["checkBoxCreateNewEvolution"].Checked == true)
                {
                    // Create new Evolution
                    int yokaiScoutableNumber = Yokais.Where(x => x.Statut.IsScoutable == true).Count();
                    int newEvolutionCount = Seed.Next(0, 10 * yokaiScoutableNumber / 100);

                    scoutableYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true && Evolutions.FindIndex(y => y.BaseYokai == x.ParamID) == -1).Select(x => x.ParamID).ToList();

                    for (int i = 0; i < newEvolutionCount; i++)
                    {
                        Evolution evolution = new Evolution();

                        int randomNumber = Seed.Next(0, scoutableYokaiID.Count);
                        evolution.BaseYokai = scoutableYokaiID[randomNumber];
                        evolution.Level = Seed.Next(1, 99);
                        scoutableYokaiID.RemoveAt(randomNumber);

                        Evolutions.Add(evolution);
                    }
                }

                if (scoutableYokaiID == null)
                {
                    scoutableYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true && Evolutions.FindIndex(y => y.BaseYokai == x.ParamID) == -1).Select(x => x.ParamID).ToList();
                }

                for (int i = 0; i < Evolutions.Count; i++)
                {
                    // Randomize Evolution
                    int randomNumber = Seed.Next(0, scoutableYokaiID.Count);
                    Evolutions[i].EvolveTo = scoutableYokaiID[randomNumber];
                    scoutableYokaiID.RemoveAt(randomNumber);
                }
            }

            // Randomize Fusion
            if (options["groupBoxFusion"].Name == "Random")
            {
                List<UInt32> scoutableYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true
                                                                && Fusions.FindIndex(y => y.BaseYokai == x.ParamID) == -1
                                                                && Fusions.FindIndex(y => y.Material == x.ParamID) == -1
                                                                ).Select(x => x.ParamID).ToList();

                foreach (Fusion fusion in Fusions.Where(x => x.BaseIsItem == false & x.EvolveToIsItem == false))
                {
                    int randomNumber = Seed.Next(0, scoutableYokaiID.Count);
                    fusion.EvolveTo = scoutableYokaiID[randomNumber];
                    scoutableYokaiID.RemoveAt(randomNumber);
                }
            }

            // Transmits some data of the base yokai to the new evolution
            for (int i = 0; i < Evolutions.Count; i++)
            {
                Yokai oldYokai = Yokais.FirstOrDefault(x => x.ParamID == Evolutions[i].BaseYokai);
                Yokai newYokai = Yokais.FirstOrDefault(x => x.ParamID == Evolutions[i].EvolveTo);
                YokaiInheritance(oldYokai, newYokai, options);
            }

            // Transmits some data of the base yokai to the new fusion
            foreach (Fusion fusion in Fusions.Where(x => x.BaseIsItem == false & x.EvolveToIsItem == false))
            {
                YokaiInheritance(Yokais.FirstOrDefault(x => fusion.BaseYokai == x.ParamID), Yokais.FirstOrDefault(x => fusion.EvolveTo == x.ParamID), options);
            }

            // Boss Stat
            if (percentageLevel > 0)
            {
                List<Yokai> bosses = Yokais.Where(x => x.Statut.IsScoutable == false).ToList();

                foreach (Yokai boss in bosses)
                {
                    for (int s = 0; s < 5; s++)
                    {
                        boss.MinStat[s] += Convert.ToInt32(percentageLevel * boss.MinStat[s] / 100);

                        if (boss.MaxStat != null)
                        {
                            boss.MaxStat[s] += Convert.ToInt32(percentageLevel * boss.MaxStat[s] / 100);
                        }
                    }
                }
            }

            // Save
            Game.SaveYokais(Yokais, Evolutions);
            Game.SaveFusions(Fusions);
        }

        public void RandomizeYokaiSoul(bool randomize)
        {
            if (randomize == false) return;

            List<uint> souls = Game.GetSouls();
            List<uint> soulsTemp = souls.ToList();

            for (int i = 0; i < souls.Count; i++)
            {
                int randomIndex = Seed.Next(0, soulsTemp.Count);
                souls[i] = soulsTemp[randomIndex];
                soulsTemp.RemoveAt(randomIndex);
            }

            // Save
            Game.SaveSouls(souls);
        }

        public void RandomizeStatic(bool randomize, decimal percentageLevel)
        {
            if (randomize == false) return;

            List<(uint, int)> encounters = Game.GetEncounter();
            List<UInt32> scoutableYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true).Select(i => i.ParamID).ToList();
            List<UInt32> staticYokaiID = Yokais.Where(x => x.Statut.IsStatic == true).Select(i => i.ParamID).ToList();

            // Randomize Encounter
            for (int i = 0; i < encounters.Count; i++)
            {
                uint yokaiParamID = encounters[i].Item1;
                int level = encounters[i].Item2;

                // Get Yokai
                Yokai yokai = Yokais.FirstOrDefault(x => x.ParamID == yokaiParamID);

                if (yokai != null)
                {
                    if (yokai.Statut.IsScoutable) {
                        yokaiParamID = scoutableYokaiID[Seed.Next(0, scoutableYokaiID.Count)];
                    } else if (yokai.Statut.IsStatic)
                    {
                        yokaiParamID = staticYokaiID[Seed.Next(0, staticYokaiID.Count)];
                    }

                    // Add Percentage Level Multiplicator
                    if (percentageLevel > 0)
                    {
                        if (level > 0)
                        {
                            int newLevel = level + Convert.ToInt32(level * percentageLevel / 100);
                            if (newLevel > 99)
                            {
                                newLevel = 99;
                            }
                        }
                    }
                }

                encounters[i] = (yokaiParamID, level);
            }

            // Save
            Game.SaveEncounter(encounters);
        }

        public void RandomizeWild(bool randomize, decimal percentageLevel)
        {
            if (randomize == false) return;

            List<UInt32> scoutableYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true).Select(i => i.ParamID).ToList();
            List<UInt32> poorYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true && x.Rank == 0).Select(i => i.ParamID).ToList();
            List<UInt32> staticYokaiID = Yokais.Where(x => x.Statut.IsStatic == true).Select(i => i.ParamID).ToList();

            string encounterFileName = "";

            if (Game is YW1)
            {
                encounterFileName = "_enc_0.02m.cfg.bin";
            } else if (Game is YW2)
            {
                encounterFileName = "_enc_0.03a.cfg.bin";
            } else if (Game is YW3)
            {
                encounterFileName = "_enc_0.01.cfg.bin";
            }

            foreach (VirtualDirectory directory in Game.Game.Directory.GetFolderFromFullPath("/data/res/map").Folders)
            {
                byte[] randomizedMap = null;
                XPCK mapData = null;

                // Get encounter file
                if (Game is YW2 || Game is YW3)
                {
                    // Folder contains data map
                    if (directory.Files.ContainsKey(directory.Name + ".pck"))
                    {
                        if (directory.Files[directory.Name + ".pck"].ByteContent == null)
                        {
                            directory.Files[directory.Name + ".pck"].Read();
                        }
                        

                        mapData = new XPCK(directory.Files[directory.Name + ".pck"].ByteContent);

                        if (mapData.Directory.Files.ContainsKey(directory.Name + encounterFileName))
                        {
                            mapData.Directory.Files[directory.Name + encounterFileName].Read();
                            randomizedMap = mapData.Directory.Files[directory.Name + encounterFileName].ByteContent;
                        }
                    }
                } else
                {
                    if (directory.Files.ContainsKey(directory.Name + encounterFileName))
                    {
                        directory.Files[directory.Name + encounterFileName].Read();
                        randomizedMap = directory.Files[directory.Name + encounterFileName].ByteContent;
                    }
                }

                // Encounter can be randomize
                if (randomizedMap != null)
                {
                    List<(uint, int)> encounters = Game.GetWorldEncounter(randomizedMap);

                    if (encounters.Count > 0)
                    {
                        // Randomize Encounter
                        for (int i = 0; i < encounters.Count; i++)
                        {
                            uint yokaiParamID = encounters[i].Item1;
                            int level = encounters[i].Item2;

                            // Get Yokai
                            Yokai yokai = Yokais.FirstOrDefault(x => x.ParamID == yokaiParamID);

                            if (yokai != null)
                            {
                                if (yokai.Statut.IsScoutable)
                                {
                                    if (i < 6)
                                    {
                                        yokaiParamID = poorYokaiID[Seed.Next(0, poorYokaiID.Count)];
                                    }
                                    else
                                    {
                                        yokaiParamID = scoutableYokaiID[Seed.Next(0, scoutableYokaiID.Count)];
                                    }
                                }
                                else if (yokai.Statut.IsStatic)
                                {
                                    yokaiParamID = staticYokaiID[Seed.Next(0, staticYokaiID.Count)];
                                }

                                // Add Percentage Level Multiplicator
                                if (percentageLevel > 0)
                                {
                                    if (level > 0)
                                    {
                                        int newLevel = level + Convert.ToInt32(level * percentageLevel / 100);
                                        if (newLevel > 99)
                                        {
                                            newLevel = 99;
                                        }
                                    }
                                }
                            }

                            encounters[i] = (yokaiParamID, level);
                        }

                        // Save
                        randomizedMap = Game.SaveWorldEncounter(encounters, randomizedMap);

                        // Replace file
                        if (Game is YW2 || Game is YW3)
                        {
                            mapData.Directory.Files[directory.Name + encounterFileName].ByteContent = randomizedMap;
                            mapData.Save();

                            using (BinaryDataReader reader = new BinaryDataReader(mapData.BaseStream))
                            {
                                directory.Files[directory.Name + ".pck"].ByteContent = reader.GetSection(0, (int)reader.Length);
                            }

                            mapData.Close();

                        }
                        else
                        {
                            directory.Files[directory.Name + encounterFileName].ByteContent = randomizedMap;
                        }
                    }
                }
            }
        }

        public void RandomizeShop(bool randomize)
        {
            if (randomize == false) return;

            foreach(KeyValuePair<string, SubMemoryStream> file in Game.Game.Directory.GetFolderFromFullPath("/data/res/shop").Files.Where(x => x.Key.StartsWith("shop_shp")))
            {
                List<uint> shops = Game.GetShop(file.Key);

                if (shops.Count > 0)
                {
                    for (int i = 0; i < shops.Count; i++)
                    {
                        if (Game.Items.ContainsKey(shops[i]))
                        {
                            shops[i] = Game.Items.ElementAt(Seed.Next(0, Game.Items.Count)).Key;
                        }
                    }

                    // Save
                    Game.SaveShop(shops, file.Key);
                }
            }
        }

        public void RandomizeTreasureBox(bool randomize)
        {
            if (randomize == false) return;

            List<UInt32> scoutableYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true).Select(i => i.ParamID).ToList();
            List<UInt32> poorYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true && x.Rank == 0).Select(i => i.ParamID).ToList();
            List<UInt32> staticYokaiID = Yokais.Where(x => x.Statut.IsStatic == true).Select(i => i.ParamID).ToList();

            string encounterFileName = "";

            if (Game is YW1)
            {
                encounterFileName = "_tbox.cfg.bin";
            }
            else if (Game is YW2)
            {
                encounterFileName = "_tbox.cfg.bin";
            }
            else if (Game is YW3)
            {
                encounterFileName = "_tbox_0.01r.cfg.bin";
            }

            foreach (VirtualDirectory directory in Game.Game.Directory.GetFolderFromFullPath("/data/res/map").Folders)
            {
                byte[] randomizedMap = null;
                XPCK mapData = null;

                // Get treasurebox file
                if (Game is YW2 || Game is YW3)
                {
                    // Folder contains data map
                    if (directory.Files.ContainsKey(directory.Name + ".pck"))
                    {
                        if (directory.Files[directory.Name + ".pck"].ByteContent == null)
                        {
                            directory.Files[directory.Name + ".pck"].Read();
                        }

                        mapData = new XPCK(directory.Files[directory.Name + ".pck"].ByteContent);

                        if (mapData.Directory.Files.ContainsKey(directory.Name + encounterFileName))
                        {
                            mapData.Directory.Files[directory.Name + encounterFileName].Read();
                            randomizedMap = mapData.Directory.Files[directory.Name + encounterFileName].ByteContent;
                        }
                    }
                }
                else
                {
                    if (directory.Files.ContainsKey(directory.Name + encounterFileName))
                    {
                        directory.Files[directory.Name + encounterFileName].Read();
                        randomizedMap = directory.Files[directory.Name + encounterFileName].ByteContent;
                    }
                }

                // treasurebox can be randomize
                if (randomizedMap != null)
                {
                    List<uint> treasures = Game.GetTreasureBox(randomizedMap);

                    if (treasures.Count > 0)
                    {
                        // Randomize treasure
                        for (int i = 0; i < treasures.Count; i++)
                        {
                            if (Game.Items.ContainsKey(treasures[i]))
                            {
                                treasures[i] = Game.Items.ElementAt(Seed.Next(0, Game.Items.Count)).Key;
                            }
                        }

                        // Save
                        randomizedMap = Game.SaveTreasureBox(treasures, randomizedMap);

                        if (Game is YW2 || Game is YW3)
                        {
                            mapData.Directory.Files[directory.Name + encounterFileName].ByteContent = randomizedMap;
                            mapData.Save();

                            using (BinaryDataReader reader = new BinaryDataReader(mapData.BaseStream))
                            {
                                directory.Files[directory.Name + ".pck"].ByteContent = reader.GetSection(0, (int)reader.Length);
                            }

                            mapData.Close();

                        }
                        else
                        {
                            directory.Files[directory.Name + encounterFileName].ByteContent = randomizedMap;
                        }
                    }
                }
            }
        }

        public void RandomizeCrankKai(bool randomize)
        {
            if (randomize == false) return;

            List<uint> capsules = Game.GetCapsule();
            List<UInt32> scoutableYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true).Select(i => i.ParamID).ToList();

            for (int i = 0; i < capsules.Count; i++)
            {
                if (scoutableYokaiID.IndexOf(capsules[i]) != -1)
                {
                    // Replace capsule reward by yokai
                    capsules[i] = scoutableYokaiID[Seed.Next(0, scoutableYokaiID.Count)];
                } else if (Game.Items.ContainsKey(capsules[i]))
                {
                    // Replace capsule reward by item
                    capsules[i] = Game.Items.ElementAt(Seed.Next(0, Game.Items.Count)).Key;
                }
            }

            // Save
            Game.SaveCapsule(capsules);
        }

        public void RandomizeGiven(bool randomize)
        {
            if (randomize == false) return;

            // Scoutable Yokai
            List<UInt32> scoutableYokaiID = Yokais.Where(x => x.Statut.IsScoutable == true).Select(i => i.ParamID).ToList();

            // Randomize Given Yokais
            foreach (KeyValuePair<string, List<uint>> givenYokai in Game.YokaiGiven)
            {
                using (BinaryDataWriter givenWriter = new BinaryDataWriter(Game.Game.Directory.GetFileFromFullPath(givenYokai.Key)))
                {
                    for (int i = 0; i < givenYokai.Value.Count; i++)
                    {
                        int randomIndex = Seed.Next(0, scoutableYokaiID.Count);
                        givenWriter.Seek(givenYokai.Value[i]);
                        givenWriter.Write(scoutableYokaiID[randomIndex]);
                        scoutableYokaiID.RemoveAt(randomIndex);
                    }
                }         
            }
        }

        public virtual void FixStory(bool randomize)
        {
            if (randomize == false) return;

            Game.FixStory();
        }

        private string PrintYokai(Yokai yokai)
        {
            Type objectType = yokai.GetType();
            PropertyInfo[] properties = objectType.GetProperties();

            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(yokai);

                if (value != null)
                {
                    string valueString = "";

                    switch (property.Name)
                    {
                        case "Rank":
                            string tryRank = Common.Ranks.Value.TryGetValue((byte)(int)value, out tryRank) ? tryRank : " ";
                            valueString = tryRank;
                            break;
                        case "Tribe":
                            string tryTribe = Game.Tribes.TryGetValue((byte)(int)value, out tryTribe) ? tryTribe : " ";
                            valueString = tryTribe;
                            break;
                        case "Strongest":
                            string tryStrongest = Common.Attribute.Value.TryGetValue((byte)value, out tryStrongest) ? tryStrongest : " ";
                            valueString = tryStrongest;
                            break;
                        case "Weakness":
                            string tryWeakness = Common.Attribute.Value.TryGetValue((byte)value, out tryWeakness) ? tryWeakness : " ";
                            valueString = tryWeakness;
                            break;
                        case "AttackID":
                            string tryAttack = Game.Attacks.TryGetValue((uint)value, out tryAttack) ? tryAttack : " ";
                            valueString = tryAttack;
                            break;
                        case "TechniqueID":
                            string tryTechnique = Game.Techniques.TryGetValue((uint)value, out tryTechnique) ? tryTechnique : " ";
                            valueString = tryTechnique;
                            break;
                        case "InspiritID":
                            string tryInspirit = Game.Inspirits.TryGetValue((uint)value, out tryInspirit) ? tryInspirit : " ";
                            valueString = tryInspirit;
                            break;
                        case "SoultimateID":
                            string trySoultimate = Game.Soultimates.TryGetValue((uint)value, out trySoultimate) ? trySoultimate : " ";
                            valueString = trySoultimate;
                            break;
                        case "SkillID":
                            string trySkill = Game.Skills.TryGetValue((uint)value, out trySkill) ? trySkill : " ";
                            valueString = trySkill;
                            break;
                        case "ScoutableID":
                            valueString = ((uint)value).ToString("X8");
                            break;
                        case "BaseID":
                            valueString = ((uint)value).ToString("X8");
                            break;
                        case "ParamID":
                            valueString = ((uint)value).ToString("X8");
                            break;
                        default:
                            valueString = value is IList ? string.Join(", ", ((IEnumerable)value).Cast<object>()) : value.ToString();
                            break;

                    }

                    sb.AppendLine($"- {property.Name}: {valueString}");
                }
            }

            return sb.ToString();
        }

        public string PrintRandom()
        {
            StringBuilder sb = new StringBuilder("Random seed: " + Seed.Seed + "\n");

            sb.AppendLine("Yo-kai Information:\n");
            foreach (Yokai yokai in Yokais)
            {
                sb.AppendLine(PrintYokai(yokai));
            }

            return sb.ToString();
        }
    }
}
