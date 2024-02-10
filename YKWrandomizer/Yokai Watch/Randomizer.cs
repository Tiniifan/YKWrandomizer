using System;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Archive.XPCK;
using YKWrandomizer.Level5.Image;
using YKWrandomizer.Level5.Script;
using YKWrandomizer.Yokai_Watch.Logic;
using YKWrandomizer.Yokai_Watch.Games;
using YKWrandomizer.Yokai_Watch.Games.YW1;
using YKW1 = YKWrandomizer.Yokai_Watch.Games.YW1.Logic;
using YKW2 = YKWrandomizer.Yokai_Watch.Games.YW2.Logic;
using YKW3 = YKWrandomizer.Yokai_Watch.Games.YW3.Logic;
using YKWB = YKWrandomizer.Yokai_Watch.Games.YWB.Logic;
using YKWrandomizer.Level5.Text;
using YKWrandomizer.Level5.Text.Logic;

namespace YKWrandomizer.Yokai_Watch
{
    public class Randomizer
    {
        public IGame Game;

        private List<ICharabase> Charabases;

        private List<ICharaparam> Charaparams;

        private List<ICharascale> Charascales;

        private List<IItem> Items;

        private RandomNumber Seed = new RandomNumber();

        private T2bþ Charanames;

        private string[] GetNames(ICharaparam[] charaparams, ICharabase[] charabases)
        {
            HashSet<string> encounteredNames = new HashSet<string>();
            Dictionary<string, int> nameOccurrences = new Dictionary<string, int>();

            return charaparams
                .Select((charaparam, index) =>
                {
                    var searchCharabase = charabases.FirstOrDefault(x => x.BaseHash == charaparam.BaseHash);

                    if (searchCharabase != null && Charanames.Nouns.TryGetValue(searchCharabase.NameHash, out var noun) && noun.Strings.Count > 0)
                    {
                        string name = noun.Strings[0].Text;

                        // The name has already been encountered, update the occurrences and add the count to the end
                        if (encounteredNames.Contains(name))
                        {
                            int occurrences = nameOccurrences.TryGetValue(name, out int count) ? count + 1 : 2;
                            nameOccurrences[name] = occurrences;
                            name += " " + occurrences;
                        }

                        // Add the name to the list of encountered names
                        encounteredNames.Add(name);

                        return name;
                    }

                    return "Param " + index;
                })
                .ToArray();
        }

        public Randomizer(IGame game)
        {
            Game = game;

            Charabases = new List<ICharabase>();
            Charabases.AddRange(Game.GetCharacterbase(false));
            Charabases.AddRange(Game.GetCharacterbase(true));

            Charaparams = new List<ICharaparam>();
            Charaparams.AddRange(Game.GetCharaparam());
            ICharaevolve[] charaevolves = Game.GetCharaevolution();

            Charascales = new List<ICharascale>();
            Charascales.AddRange(Game.GetCharascale());

            // Get Names
            Charanames = new T2bþ(Game.Files["chara_text"].File.Directory.GetFileFromFullPath(Game.Files["chara_text"].Path));

            // Link charaevolve to charaparam
            foreach (ICharaparam charaparamsWithEvolve in Charaparams.Where(x => x.EvolveOffset != -1).ToArray())
            {
                ICharaevolve charaevolve = charaevolves[charaparamsWithEvolve.EvolveOffset];
                charaparamsWithEvolve.EvolveParam = charaevolve.ParamHash;
                charaparamsWithEvolve.EvolveLevel = charaevolve.Level;
                charaparamsWithEvolve.EvolveCost = charaevolve.Cost;
            }

            Items = new List<IItem>();
            Items.AddRange(Game.GetItems("all"));
        }

        private void YokaiInheritance(ICharaparam parentParam, ICharaparam childParam, Dictionary<string, Option> options)
        {
            // Get charabases
            ICharabase parentBase = Charabases.FirstOrDefault(x => x.BaseHash == parentParam.BaseHash);
            ICharabase childBase = Charabases.FirstOrDefault(x => x.BaseHash == childParam.BaseHash);

            if (options["groupBoxTribe"].Name == "Swap")
            {
                childBase.Tribe = parentBase.Tribe;
                childParam.Tribe = parentParam.Tribe;
            }

            if (options["groupBoxRank"].Name == "Swap")
            {
                childBase.Rank = Seed.Next(parentBase.Rank, 6);
            }

            if (options["groupBoxRole"].Name == "Swap")
            {
                childBase.Role = parentBase.Role;
            }

            if (options["groupBoxStatus"].Name == "Swap")
            {
                childBase.IsRare = parentBase.IsRare;
                childBase.IsMerican = parentBase.IsMerican;
                childBase.IsClassic = parentBase.IsClassic;
            }

            if (options["groupBoxFavFood"].Name == "Swap")
            {
                childBase.FavoriteFoodHash = parentBase.FavoriteFoodHash;
            }

            if (options["groupBoxHatedFood"].Name == "Swap")
            {
                childBase.HatedFoodHash = parentBase.HatedFoodHash;
            }

            if (options["groupBoxStrongest"].Name == "Swap")
            {
                childParam.AttributeDamageFire = parentParam.AttributeDamageFire;
                childParam.AttributeDamageIce = parentParam.AttributeDamageIce;
                childParam.AttributeDamageEarth = parentParam.AttributeDamageEarth;
                childParam.AttributeDamageLigthning = parentParam.AttributeDamageLigthning;
                childParam.AttributeDamageWater = parentParam.AttributeDamageWater;
                childParam.AttributeDamageWind = parentParam.AttributeDamageWind;
                childParam.Strongest = parentParam.Strongest;
            }

            if (options["groupBoxWeakness"].Name == "Swap")
            {
                childParam.Strongest = parentParam.Weakness;
            }

            if (options["groupBoxBaseStat"].Name == "Random")
            {
                int[] minStats = new int[5] { parentParam.MinHP, parentParam.MinStrength, parentParam.MinSpirit, parentParam.MinDefense, parentParam.MinSpeed };
                int[] maxStats = new int[5] { parentParam.MaxHP, parentParam.MaxStrength, parentParam.MaxSpirit, parentParam.MaxDefense, parentParam.MaxSpeed };

                for (int m = 0; m < 5; m++)
                {
                    if (Game is YW1)
                    {
                        int minStat = minStats[m];

                        if (m == 0)
                        {
                            minStat += Seed.Next(10, 21) + childBase.Rank;
                        }
                        else
                        {
                            minStat += Seed.Next(5, 11) + childBase.Rank;
                        }
                    }
                    else
                    {
                        int minStat = minStats[m];
                        int maxStat = maxStats[m];

                        if (m == 0)
                        {
                            minStat += Seed.Next(10, 21) + childBase.Rank;
                            maxStat += Seed.Next(10, 21) + childBase.Rank * 5;
                        }
                        else
                        {
                            minStat += Seed.Next(5, 11) + childBase.Rank;
                            maxStat += Seed.Next(5, 11) + childBase.Rank * 5;
                        }
                    }
                }

                childParam.MinHP = minStats[0];
                childParam.MinStrength = minStats[1];
                childParam.MinSpirit = minStats[2];
                childParam.MinDefense = minStats[3];
                childParam.MinSpeed = minStats[4];
                childParam.MaxHP = maxStats[0];
                childParam.MaxStrength = maxStats[1];
                childParam.MaxSpirit = maxStats[2];
                childParam.MaxDefense = maxStats[3];
                childParam.MaxSpeed = maxStats[4];
            }

            if (options["groupBoxWaitTime"].Name == "Swap")
            {
                childParam.WaitTime = parentParam.WaitTime;
            }

            if (options["groupBoxAttack"].Name == "Swap")
            {

                childParam.AttackHash = parentParam.AttackHash;
            }

            if (options["groupBoxTechnique"].Name == "Swap")
            {

                childParam.TechniqueHash = parentParam.TechniqueHash;
            }

            if (options["groupBoxInspirit"].Name == "Swap")
            {

                childParam.InspiritHash = parentParam.InspiritHash;
            }

            if (options["groupBoxSoultimateMove"].Name == "Swap")
            {
                childParam.SoultimateHash = parentParam.SoultimateHash;
            }
        }

        public void RandomizeYokai(Dictionary<string, Option> options, decimal percentageLevel)
        {
            // Get all fusions
            List<ICombineConfig> fusions = Game.GetFusions().ToList();

            // Swap yokai models
            if (options["groupBoxBaseMiscellaneous"].CheckBoxes["checkBoxSwapModel"].Checked == true)
            {
                // Get only yokais
                List<ICharabase> yokaiCharabases = Charabases.Where(x => x.IsYokai == true).ToList();

                foreach (ICharabase yokai in Charabases.Where(x => x.IsYokai == true))
                {
                    // Get random yokai
                    int randomIndex = Seed.Next(yokaiCharabases.Count);
                    ICharabase randomYokaiCharabase = yokaiCharabases[randomIndex];

                    // Swap some informations and model
                    yokai.NameHash = randomYokaiCharabase.NameHash;
                    yokai.DescriptionHash = randomYokaiCharabase.DescriptionHash;
                    yokai.MedalPosX = randomYokaiCharabase.MedalPosX;
                    yokai.MedalPosY = randomYokaiCharabase.MedalPosY;
                    yokai.FileNameNumber = randomYokaiCharabase.FileNameNumber;
                    yokai.FileNamePrefix = randomYokaiCharabase.FileNamePrefix;
                    yokai.FileNameVariant = randomYokaiCharabase.FileNameVariant;

                    // Remove the used yokai
                    yokaiCharabases.RemoveAt(randomIndex);
                }

                Console.WriteLine("YES");
            }

            // Randomize yokai informations
            foreach (ICharaparam charaparam in Charaparams)
            {
                // Get charabase
                ICharabase charabase = Charabases.FirstOrDefault(x => x.BaseHash == charaparam.BaseHash);

                // Get tribe name
                string tribeName = "";
                if (Game.Name == "Yo-Kai Watch 1")
                {
                    tribeName = Game.Tribes[charaparam.Tribe];
                }
                else
                {
                    tribeName = Game.Tribes[charabase.Tribe];
                }

                // Randomize drop item
                if (options["groupBoxDrop"].Name == "Random")
                {
                    // Prevent to randomize key items
                    if (Items.Any(x => x.ItemHash == charaparam.Drop1Hash) || charaparam.Drop1Hash == 0x0)
                    {
                        charaparam.Drop1Hash = Items.ElementAt(Seed.GetNumbers(0, Items.Count, 2)[0]).ItemHash;
                    }

                    if (Items.Any(x => x.ItemHash == charaparam.Drop2Hash) || charaparam.Drop2Hash == 0x0)
                    {
                        charaparam.Drop2Hash = Items.ElementAt(Seed.GetNumbers(0, Items.Count, 2)[0]).ItemHash;
                    }
                }

                // Playable yokai
                if ((charaparam.ScoutableHash != 0x00 && charaparam.ShowInMedalium == true) || (charaparam.ScoutableHash == 0x00 && charaparam.ShowInMedalium == true && tribeName != "Boss" && tribeName != "Untribe"))
                {
                    // Randomize tribe
                    if (options["groupBoxTribe"].Name != "Unchanged")
                    {
                        int tribe = Seed.Next(1, Game.Tribes.Count);

                        while (Game.Tribes[tribe] == "Boss")
                        {
                            tribe = Seed.Next(1, Game.Tribes.Count);
                        }

                        charaparam.Tribe = tribe;
                        charabase.Tribe = tribe;
                    }

                    // Randomize rank
                    if (options["groupBoxRank"].Name != "Unchanged")
                    {
                        charabase.Rank = Seed.Next(0, 6);
                    }

                    // Randomize role
                    if (options["groupBoxRole"].Name != "Unchanged")
                    {
                        charabase.Rank = Seed.Next(1, 5);
                    }

                    // Randomize status
                    if (options["groupBoxStatus"].Name != "Unchanged")
                    {
                        charabase.IsRare = Convert.ToBoolean(Seed.Probability(new int[] { 90, 10 }));
                        charabase.IsMerican = Convert.ToBoolean(Seed.Probability(new int[] { 95, 5 }));
                        charabase.IsClassic = Convert.ToBoolean(Seed.Probability(new int[] { 99, 1 }));
                    }

                    // Randomize favorite food
                    if (options["groupBoxFavFood"].Name != "Unchanged")
                    {
                        charabase.FavoriteFoodHash = Seed.Next(Game.FoodsType.Count);
                    }

                    // Randomize hated food
                    if (options["groupBoxHatedFood"].Name != "Unchanged")
                    {
                        charabase.HatedFoodHash = Seed.Next(0, Game.FoodsType.Count, charabase.FavoriteFoodHash);
                    }

                    // Randomize resistance
                    if (options["groupBoxStrongest"].Name != "Unchanged")
                    {
                        // YKW1 - YKW2
                        float[] attributeDamage = new float[6] { 1f, 1f, 1f, 1f, 1f, 1f };

                        bool weaked = Convert.ToBoolean(Seed.Probability(new int[] { 10, 90 }));

                        if (weaked)
                        {
                            // Weak type
                            int weakCount = Seed.Next(0, 4);
                            List<int> weakIndices = new List<int>();

                            if (weakCount > 0)
                            {
                                weakIndices = Seed.GetNumbers(0, 6, weakCount);
                                for (int i = 0; i < weakIndices.Count; i++)
                                {
                                    float weakDamage = (float)(Seed.Next(11, 23) * 0.1);
                                    attributeDamage[weakIndices[i]] = weakDamage;
                                }
                            }

                            int guardCount = Seed.Next(0, 2);
                            if (guardCount > 0)
                            {
                                int guardIndice = 0;

                                if (weakIndices.Count > 0)
                                {
                                    guardIndice = Seed.GetNumbers(0, 6, 1, weakIndices).First();
                                }
                                else
                                {
                                    guardIndice = Seed.Next(0, 6);
                                }

                                attributeDamage[guardIndice] = (float)(Seed.Next(1, 11) * 0.1);
                            }
                        }
                        else
                        {
                            // Guard type
                            int guardCount = Seed.Next(0, 4);
                            List<int> guardIndices = new List<int>();

                            if (guardCount > 0)
                            {
                                guardIndices = Seed.GetNumbers(0, 6, guardCount);
                                for (int i = 0; i < guardIndices.Count; i++)
                                {
                                    float guardDamage = (float)(Seed.Next(1, 11) * 0.1);
                                    attributeDamage[guardIndices[i]] = guardDamage;
                                }
                            }

                            int weakCount = Seed.Next(0, 2);
                            if (weakCount > 0)
                            {
                                int weakIndice = 0;

                                if (guardIndices.Count > 0)
                                {
                                    weakIndice = Seed.GetNumbers(0, 6, 1, guardIndices).First();
                                }
                                else
                                {
                                    weakIndice = Seed.Next(0, 6);
                                }

                                attributeDamage[weakIndice] = (float)(Seed.Next(1, 11) * 0.1);
                            }
                        }

                        charaparam.AttributeDamageFire = attributeDamage[0];
                        charaparam.AttributeDamageIce = attributeDamage[1];
                        charaparam.AttributeDamageEarth = attributeDamage[2];
                        charaparam.AttributeDamageLigthning = attributeDamage[3];
                        charaparam.AttributeDamageWater = attributeDamage[4];
                        charaparam.AttributeDamageWind = attributeDamage[5];

                        // YKW3
                        charaparam.Strongest = (byte)Seed.Next(0, 7);
                    }

                    // Randomize weakness
                    if (options["groupBoxWeakness"].Name != "Unchanged")
                    {
                        charaparam.Weakness = Seed.Next(0, 7);
                    }

                    // Randomize stat
                    if (options["groupBoxBaseStat"].Name == "Swap")
                    {
                        int[] minStat = new int[5] { charaparam.MinHP, charaparam.MinStrength, charaparam.MinSpirit, charaparam.MinDefense, charaparam.MinSpeed };
                        int[] maxStat = new int[5] { charaparam.MaxHP, charaparam.MaxStrength, charaparam.MaxSpirit, charaparam.MaxDefense, charaparam.MaxSpeed };

                        int[] randomMinStat = Seed.GetNumbers(0, 6, 5).ToArray();
                        int[] randomMaxStat = Seed.GetNumbers(0, 6, 5).ToArray();

                        charaparam.MinHP = minStat[randomMinStat[0]];
                        charaparam.MinStrength = minStat[randomMinStat[1]];
                        charaparam.MinSpirit = minStat[randomMinStat[2]];
                        charaparam.MinDefense = minStat[randomMinStat[3]];
                        charaparam.MinSpeed = minStat[randomMinStat[4]];

                        charaparam.MaxHP = maxStat[randomMaxStat[0]];
                        charaparam.MaxStrength = maxStat[randomMaxStat[1]];
                        charaparam.MaxSpirit = maxStat[randomMaxStat[2]];
                        charaparam.MaxDefense = maxStat[randomMaxStat[3]];
                        charaparam.MaxSpeed = maxStat[randomMaxStat[4]];
                    }
                    else if (options["groupBoxBaseStat"].Name == "Random")
                    {
                        int[] minStats = new int[5];
                        int[] maxStats = new int[5];

                        int bonusRank = charabase.Rank * 10;

                        for (int m = 0; m < 5; m++)
                        {
                            int minStat = 0;
                            int maxStat = 0;

                            if (Game is YW1)
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
                                if (charabase.IsRare)
                                {
                                    minStat += 7;
                                }

                                if (charabase.IsLegend)
                                {
                                    minStat += 14;
                                }

                                minStat += charabase.Rank;
                                minStats[m] = minStat;
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
                                if (charabase.IsRare || charabase.IsCommandant || charabase.IsDeva || charabase.IsTreasure)
                                {
                                    minStat += (int)(0.07 * minStat);
                                }

                                if (charabase.IsLegend || charabase.IsLegendaryMystery || charabase.IsPionner)
                                {
                                    minStat += (int)(0.1 * minStat);
                                }

                                minStat += (int)(bonusRank * 0.01 * minStat);
                                maxStat += (int)(bonusRank * 0.01 * minStat);
                                minStats[m] = minStat;
                                maxStats[m] = maxStat;
                            }
                        }

                        int[] randomMinStat = Seed.GetNumbers(1, 5, 4).ToArray();
                        int[] randomMaxStat = Seed.GetNumbers(1, 5, 4).ToArray();

                        charaparam.MinHP = minStats[0];
                        charaparam.MinStrength = minStats[randomMinStat[0]];
                        charaparam.MinSpirit = minStats[randomMinStat[1]];
                        charaparam.MinDefense = minStats[randomMinStat[2]];
                        charaparam.MinSpeed = minStats[randomMinStat[3]];

                        charaparam.MaxHP = maxStats[0];
                        charaparam.MaxStrength = maxStats[randomMaxStat[0]];
                        charaparam.MaxSpirit = maxStats[randomMaxStat[1]];
                        charaparam.MaxDefense = maxStats[randomMaxStat[2]];
                        charaparam.MaxSpeed = maxStats[randomMaxStat[3]];
                    }

                    // Randomize experience curve
                    if (options["groupBoxExperience"].Name == "Random")
                    {
                        charaparam.ExperienceCurve = Seed.Next(0, 7);
                    }

                    // Randomize Wait time
                    if (options["groupBoxWaitTime"].Name == "Random")
                    {
                        charaparam.WaitTime = Seed.Next(0, 7);
                    }
                }
                else
                {
                    if (percentageLevel > 0)
                    {
                        charaparam.MinHP += Convert.ToInt32(percentageLevel * charaparam.MinHP / 100);
                        charaparam.MinStrength += Convert.ToInt32(percentageLevel * charaparam.MinStrength / 100);
                        charaparam.MinSpirit += Convert.ToInt32(percentageLevel * charaparam.MinSpirit / 100);
                        charaparam.MinDefense += Convert.ToInt32(percentageLevel * charaparam.MinDefense / 100);
                        charaparam.MinSpeed += Convert.ToInt32(percentageLevel * charaparam.MinSpeed / 100);
                        charaparam.MaxHP += Convert.ToInt32(percentageLevel * charaparam.MaxHP / 100);
                        charaparam.MaxStrength += Convert.ToInt32(percentageLevel * charaparam.MaxStrength / 100);
                        charaparam.MaxSpirit += Convert.ToInt32(percentageLevel * charaparam.MaxSpirit / 100);
                        charaparam.MaxDefense += Convert.ToInt32(percentageLevel * charaparam.MaxDefense / 100);
                        charaparam.MaxSpeed += Convert.ToInt32(percentageLevel * charaparam.MaxSpeed / 100);
                    }
                }

                // Playable yokai and mini-boss yokai
                if (tribeName != "Boss" && tribeName != "Untribe")
                {
                    // Randomize attack
                    if (options["groupBoxAttack"].Name != "Unchanged")
                    {
                        charaparam.AttackHash = (int)Game.Attacks.ElementAt(Seed.Next(Game.Attacks.Count)).Key;
                    }

                    // Randomize technique
                    if (options["groupBoxTechnique"].Name != "Unchanged")
                    {
                        charaparam.TechniqueHash = (int)Game.Techniques.ElementAt(Seed.Next(Game.Techniques.Count)).Key;
                    }

                    // Randomize inspirit
                    if (options["groupBoxInspirit"].Name != "Unchanged")
                    {
                        charaparam.InspiritHash = (int)Game.Inspirits.ElementAt(Seed.Next(Game.Inspirits.Count)).Key;
                    }

                    // Randomize soultimate
                    if (options["groupBoxSoultimateMove"].Name != "Unchanged")
                    {
                        charaparam.SoultimateHash = (int)Game.Soultimates.ElementAt(Seed.Next(Game.Soultimates.Count)).Key;
                    }

                    // Randomize ability
                    if (options["groupBoxSkill"].Name != "Unchanged")
                    {
                        charaparam.AbilityHash = (int)Game.Skills.ElementAt(Seed.Next(Game.Skills.Count)).Key;
                    }

                    // Fix money
                    if (options["groupBoxMiscellaneous"].CheckBoxes["checkBoxScaleMoney"].Checked)
                    {
                        if (charaparam.Money > 35)
                        {
                            charaparam.Money = 5 + (charabase.Rank + 1) * 6;
                        }
                    }

                    // Fix experience
                    if (options["groupBoxMiscellaneous"].CheckBoxes["checkBoxScaleMoney"].Checked)
                    {
                        if (charaparam.Experience > 46)
                        {
                            charaparam.Experience = 26 + (charabase.Rank + 1) * 4;
                        }
                    }
                }
            }

            // Fix Yokais
            Game.FixYokai(Charabases);

            // Randomize evolution
            if (options["groupBoxEvolution"].Name == "Random")
            {
                // Get possible yokai
                List<int> yokaiParamHashes = Charaparams
                    .Where(x => x.ScoutableHash != 0x00
                                && x.ShowInMedalium == true
                                && x.EvolveOffset == -1
                                && (Charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash) is var charaBase)
                                && !charaBase.IsLegend
                                && !charaBase.IsPionner
                                && !charaBase.IsLegendaryMystery)
                    .Select(x => x.ParamHash)
                    .ToList();

                foreach (ICharaparam charaparamWithEvolve in Charaparams.Where(x => x.EvolveParam != 0x00).ToArray())
                {
                    int randomIndex = Seed.Next(0, yokaiParamHashes.Count);
                    int paramHash = yokaiParamHashes[randomIndex];
                    charaparamWithEvolve.EvolveParam = paramHash;
                    yokaiParamHashes.RemoveAt(randomIndex);
                }

                if (options["groupBoxEvolution"].CheckBoxes["checkBoxCreateNewEvolution"].Checked == true)
                {
                    // Create new Evolution
                    if (yokaiParamHashes.Count > 0)
                    {
                        int minEvolutionCount = (20 * yokaiParamHashes.Count()) / 100;
                        int maxEvolutionCount = (30 * yokaiParamHashes.Count()) / 100;
                        int newEvolutionCount = Seed.Next(minEvolutionCount, maxEvolutionCount) / 2;

                        for (int i = 0; i < newEvolutionCount; i++)
                        {
                            if (yokaiParamHashes.Count == 0)
                            {
                                break;
                            }
                            else
                            {
                                int randomIndex1 = Seed.Next(0, yokaiParamHashes.Count);
                                int randomIndex2 = Seed.Next(0, yokaiParamHashes.Count, randomIndex1);

                                int paramHash1 = yokaiParamHashes[randomIndex1];
                                int paramHash2 = yokaiParamHashes[randomIndex2];

                                ICharaparam yokai = Charaparams.FirstOrDefault(x => x.ParamHash == paramHash1);
                                yokai.EvolveParam = paramHash2;
                                yokai.EvolveLevel = Seed.Next(1, 99);
                                yokai.EvolveCost = Seed.Next(1, 11) * 1000;

                                yokaiParamHashes.Remove(paramHash1);
                                yokaiParamHashes.Remove(paramHash2);
                            }
                        }
                    }
                }
            }

            // Randomize Fusion
            if (options["groupBoxFusion"].Name == "Random")
            {
                // Get possible yokai
                List<int> yokaiParamHashes = Charaparams
                    .Where(x => x.ScoutableHash != 0x00
                                && x.ShowInMedalium == true
                                && x.EvolveOffset == -1
                                && Charaparams.Any(y => y.EvolveParam == x.ParamHash) == false
                                && fusions.Any(y => y.BaseHash == x.ParamHash) == false
                                && fusions.Any(y => y.MaterialHash == x.ParamHash) == false
                                && (Charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash) is var charaBase)
                                && !charaBase.IsLegend
                                && !charaBase.IsPionner
                                && !charaBase.IsLegendaryMystery)
                    .Select(x => x.ParamHash)
                    .ToList();

                foreach (ICombineConfig fusion in fusions.Where(x => x.EvolveToIsItem == false))
                {
                    if (yokaiParamHashes.Count > 0)
                    {
                        // Get random param hash
                        int randomIndex = Seed.Next(yokaiParamHashes.Count);
                        int randomParamHash = yokaiParamHashes[randomIndex];

                        // Randomize fusion
                        fusion.EvolveToHash = randomParamHash;

                        // Remove selected index
                        yokaiParamHashes.RemoveAt(randomIndex);
                    }
                }
            }

            // loop on all fusions that result in a yokai
            foreach (ICombineConfig fusion in fusions.Where(x => x.EvolveToIsItem == false))
            {
                ICharaparam parent = null;
                ICharaparam child = Charaparams.FirstOrDefault(x => x.ParamHash == fusion.EvolveToHash);

                if (fusion.BaseIsItem == false && fusion.MaterialIsItem == true)
                {
                    // Fusion with yokai and item
                    parent = Charaparams.FirstOrDefault(x => x.ParamHash == fusion.BaseHash);
                }
                else if (fusion.BaseIsItem == true && fusion.MaterialIsItem == false)
                {
                    // Fusion with item and yokai
                    parent = Charaparams.FirstOrDefault(x => x.ParamHash == fusion.MaterialHash);
                }
                else if (fusion.BaseIsItem == false && fusion.MaterialIsItem == false)
                {
                    // Fusion with yokai and yokai

                    // Get random parent
                    if (Seed.Next(0, 100) < 50)
                    {
                        parent = Charaparams.FirstOrDefault(x => x.ParamHash == fusion.BaseHash);
                    }
                    else
                    {
                        parent = Charaparams.FirstOrDefault(x => x.ParamHash == fusion.MaterialHash);
                    }
                }

                if (parent != null && child != null)
                {
                    // Transmits some data of the base yokai to the new fusion
                    YokaiInheritance(parent, child, options);
                }
            }

            // Generate charaevolves array
            List<ICharaevolve> charaevolves = new List<ICharaevolve>() { };

            // Loop through Charaparams with non-zero ParamHash
            foreach (ICharaparam charaparamWithEvolve in Charaparams.Where(x => x.EvolveParam != 0x00).ToArray())
            {
                // Get the evolution and transmits some data of the base yokai to the new evolution
                ICharaparam charaparamEvolve = Charaparams.FirstOrDefault(x => x.ParamHash == charaparamWithEvolve.ParamHash);
                YokaiInheritance(charaparamWithEvolve, charaparamEvolve, options);

                ICharaevolve charaevolve = null;

                // Determine the game type and get the corresponding Charaevolve logic
                switch (Game.Name)
                {
                    case "Yo-Kai Watch 1":
                        charaevolve = GameSupport.GetLogic<YKW1.Charaevolve>();
                        break;
                    case "Yo-Kai Watch 2":
                        charaevolve = GameSupport.GetLogic<YKW2.Charaevolve>();
                        break;
                    case "Yo-Kai Watch 3":
                        charaevolve = GameSupport.GetLogic<YKW3.Charaevolve>();
                        break;
                    case "Yo-Kai Watch Blaster":
                        charaevolve = GameSupport.GetLogic<YKWB.Charaevolve>();
                        break;
                }

                // Assign values to charaevolve properties
                charaevolve.ParamHash = charaparamWithEvolve.EvolveParam;
                charaevolve.Level = charaparamWithEvolve.EvolveLevel;
                charaevolve.Cost = charaparamWithEvolve.EvolveCost;
                charaparamWithEvolve.EvolveOffset = charaevolves.Count();

                // Add charaevolve to the charaevolves list
                charaevolves.Add(charaevolve);
            }

            // Save
            Game.SaveFusions(fusions.ToArray());
            Game.SaveCharascale(Charascales.ToArray());
            Game.SaveCharaBase(Charabases.ToArray());
            Game.SaveCharaparam(Charaparams.ToArray());
            Game.SaveCharaevolution(charaevolves.ToArray());
            //Game.SaveBattleCharaparam(BattleCharaparams.ToArray());
            //Game.SaveHackslashCharaparam(HackslashCharaparams.ToArray());

            // Get medallium icons
            byte[] faceIconData = Game.Files["face_icon"].File.Directory.GetFileFromFullPath(Game.Files["face_icon"].Path + "/face_icon.xi");
            Bitmap faceIcon = IMGC.ToBitmap(faceIconData);

            // Get the size of the medal
            int medalSize = 0;
            if (Game.Name == "Yo-Kai Watch 1")
            {
                medalSize = 44;
            }
            else if (Game.Name == "Yo-Kai Watch 2")
            {
                medalSize = 44;
            }
            else if (Game.Name == "Yo-Kai Watch 3")
            {
                medalSize = 32;
            }
            else if (Game.Name == "Yo-Kai Watch Blaster")
            {
                medalSize = 44;
            }

            // Fix medallium icons
            foreach(ICharaparam charaparam in Charaparams)
            {
                // Get charabase
                ICharabase charabase = Charabases.FirstOrDefault(x => x.BaseHash == charaparam.BaseHash);

                // Get tribe name
                string tribeName = "";
                if (Game.Name == "Yo-Kai Watch 1")
                {
                    tribeName = Game.Tribes[charaparam.Tribe];
                }
                else
                {
                    tribeName = Game.Tribes[charabase.Tribe];
                }
                
                if (charaparam.ShowInMedalium == true && charaparam.MedaliumOffset > 0 && tribeName != "Boss" && tribeName != "Untribe")
                {
                    if (charabase.MedalPosX > -1 && charabase.MedalPosY > -1)
                    {
                        // Get tribe
                        int tribe = 0;
                        if (Game.Name == "Yo-Kai Watch 1")
                        {
                            tribe = charaparam.Tribe;
                        } else
                        {
                            tribe = charabase.Tribe;
                        }

                        Image newMedal = null;

                        if (charabase.IsLegend)
                        {
                            newMedal = Image.FromStream(new ResourceReader("y_medal_lege0" + tribe + ".png").GetResourceStream());
                        }
                        else if (charabase.IsRare)
                        {
                            newMedal = Image.FromStream(new ResourceReader("y_medal_rare0" + tribe + ".png").GetResourceStream());
                        }
                        else
                        {
                            newMedal = Image.FromStream(new ResourceReader("y_medal_nml0" + tribe + ".png").GetResourceStream());
                        }

                        faceIcon = Draw.DrawImage(faceIcon, charabase.MedalPosX * medalSize, charabase.MedalPosY * medalSize, newMedal);
                    }
                }   
            }

            // Save new medallium icons
            Game.Files["face_icon"].File.Directory.GetFolderFromFullPath(Game.Files["face_icon"].Path).Files["face_icon.xi"].ByteContent = IMGC.ToIMGC(faceIcon, new RGBA8());
        }

        public void RandomizeStatic(bool randomize, decimal percentageLevel)
        {
            if (randomize == false) return;

            if (Game.Name == "Yo-Kai Watch Blasters")
            {

            }
            else
            {
                // Get encounters
                (IEncountTable[], IEncountChara[]) staticEncountersData = Game.GetStaticEncounters();
                IEncountTable[] encountTables = staticEncountersData.Item1;
                IEncountChara[] encountCharas = staticEncountersData.Item2;

                // Get scoutable yokai
                List<int> scoutableParamHashes = Charaparams
                    .Where(x => x.ScoutableHash != 0x00
                                && x.ShowInMedalium == true
                                && (Charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash) is var charaBase)
                                && !charaBase.IsLegend
                                && !charaBase.IsPionner
                                && !charaBase.IsLegendaryMystery)
                    .Select(x => x.ParamHash)
                    .ToList();

                // Get static yokai
                List<int> staticParamHashes = Charaparams
                    .Where(charaparam =>
                    {
                        ICharabase charabase = Charabases.FirstOrDefault(x => x.BaseHash == charaparam.BaseHash);
                        string tribeName = (Game.Name == "Yo-Kai Watch 1") ? Game.Tribes[charaparam.Tribe] : Game.Tribes[charabase.Tribe];
                        return (charaparam.ShowInMedalium == false && tribeName != "Boss" && tribeName != "Untribe");
                    })
                    .Select(x => x.ParamHash)
                    .ToList();

                // Randomize static encounters
                foreach (IEncountChara encountChara in encountCharas)
                {
                    if (scoutableParamHashes.Contains(encountChara.ParamHash))
                    {
                        // Get random param hash
                        int randomIndex = Seed.Next(scoutableParamHashes.Count);
                        int randomParamHash = scoutableParamHashes[randomIndex];
                        encountChara.ParamHash = randomParamHash;
                    }
                    else if (staticParamHashes.Contains(encountChara.ParamHash))
                    {
                        // Get random param hash
                        int randomIndex = Seed.Next(staticParamHashes.Count);
                        int randomParamHash = staticParamHashes[randomIndex];
                        encountChara.ParamHash = randomParamHash;
                    }

                    // Add Percentage Level Multiplicator
                    if (percentageLevel > 0)
                    {
                        if (encountChara.Level > 0)
                        {
                            int newLevel = encountChara.Level + Convert.ToInt32(encountChara.Level * percentageLevel / 100);

                            if (newLevel > 99)
                            {
                                newLevel = 99;
                            }

                            encountChara.Level = newLevel;
                        }
                    }
                }

                // Save static encounters
                Game.SaveStaticEncounters(encountTables, encountCharas);
            }
        }

        public void SwapBosses(bool randomize, bool scaleStat)
        {
            if (randomize == false) return;

            if (Game.Name == "Yo-Kai Watch Blasters")
            {

            }
            else
            {
                // Get encounters
                (IEncountTable[], IEncountChara[]) staticEncountersData = Game.GetStaticEncounters();
                IEncountTable[] encountTables = staticEncountersData.Item1;
                IEncountChara[] encountCharas = staticEncountersData.Item2;

                // Get tables containing only boss battles
                List<(IEncountTable, int)> bossEncountTables = Game.BossBattles
                    .Select(bossBattle =>
                    {
                        // Join table and supposed boss level
                        IEncountTable encountTable = (IEncountTable)encountTables.FirstOrDefault(x => x.EncountConfigHash == bossBattle.Key).Clone();
                        return (encountTable, bossBattle.Value);
                    })
                    .Where(tuple => tuple.encountTable != null)
                    .ToList();

                int[] randomOrder = Seed.GetNumbers(0, bossEncountTables.Count, bossEncountTables.Count).ToArray();

                for (int i = 0; i < bossEncountTables.Count(); i++)
                {
                    KeyValuePair<int, int> bossBattle = Game.BossBattles.ElementAt(i);

                    // Get boss encount table
                    IEncountTable encountTable = encountTables.FirstOrDefault(x => x.EncountConfigHash == bossBattle.Key);

                    if (encountTable != null)
                    {
                        // Get random boss battle
                        (IEncountTable, int) bossBattleRandom = bossEncountTables[randomOrder[i]];

                        // Swap
                        encountTable.EncountConfigHash = bossBattleRandom.Item1.EncountConfigHash;

                        if (scaleStat)
                        {
                            // Adapt the stats of all bosses from the current table
                            for (int j = 0; j < encountTable.EncountOffsets.Length; j++)
                            {
                                if (encountTable.EncountOffsets[j] != -1)
                                {
                                    // Get charaparam of the boss
                                    ICharaparam bossCharaparam = Charaparams.FirstOrDefault(x => x.ParamHash == encountCharas[encountTable.EncountOffsets[j]].ParamHash);

                                    if (bossCharaparam != null)
                                    {
                                        // Set levels
                                        int oldLevel = bossBattle.Value;
                                        int newLevel = bossBattleRandom.Item2;
                                        double ratioLevel = (double)newLevel / oldLevel;

                                        if (oldLevel != newLevel)
                                        {
                                            // Get stats
                                            int[] minStat = new int[5] { bossCharaparam.MinHP, bossCharaparam.MinStrength, bossCharaparam.MinSpirit, bossCharaparam.MinDefense, bossCharaparam.MinSpeed };
                                            int[] maxStat = new int[5] { bossCharaparam.MaxHP, bossCharaparam.MaxStrength, bossCharaparam.MaxSpirit, bossCharaparam.MaxDefense, bossCharaparam.MaxSpeed };

                                            for (int s = 0; s < 5; s++)
                                            {
                                                minStat[s] = (int)(minStat[s] * ratioLevel);
                                                maxStat[s] = (int)(maxStat[s] * ratioLevel);
                                            }

                                            // Save
                                            bossCharaparam.MinHP = minStat[0];
                                            bossCharaparam.MinStrength = minStat[1];
                                            bossCharaparam.MinSpirit = minStat[2];
                                            bossCharaparam.MinDefense = minStat[3];
                                            bossCharaparam.MinSpeed = minStat[4];
                                            bossCharaparam.MaxHP = maxStat[0];
                                            bossCharaparam.MaxStrength = maxStat[1];
                                            bossCharaparam.MaxSpirit = maxStat[2];
                                            bossCharaparam.MaxDefense = maxStat[3];
                                            bossCharaparam.MaxSpeed = maxStat[4];
                                            bossCharaparam.Experience = (int)(bossCharaparam.Experience * ratioLevel);
                                            bossCharaparam.Experience = (int)(bossCharaparam.Money * ratioLevel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Save static encounters
                Game.SaveStaticEncounters(encountTables, encountCharas);
            }
        }

        public void RandomizeWild(bool randomize, decimal percentageLevel)
        {
            if (randomize == false) return;

            if (Game.Name == "Yo-Kai Watch Blasters")
            {

            }
            else
            {
                // Get scoutable yokai
                List<(ICharaparam, ICharabase)> scoutableYokai = Charaparams
                    .Where(x => x.ScoutableHash != 0x00
                                && x.ShowInMedalium == true
                                && Charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash) is ICharabase charaBase
                                && !charaBase.IsLegend
                                && !charaBase.IsPionner
                                && !charaBase.IsLegendaryMystery)
                    .Select(x => (x, Charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash)))
                    .Where(tuple => tuple.Item2 != null)
                    .ToList();

                // Get static yokai
                List<int> staticParamHashes = Charaparams
                    .Where(charaparam =>
                    {
                        ICharabase charabase = Charabases.FirstOrDefault(x => x.BaseHash == charaparam.BaseHash);
                        string tribeName = (Game.Name == "Yo-Kai Watch 1") ? Game.Tribes[charaparam.Tribe] : Game.Tribes[charabase.Tribe];
                        return (charaparam.ShowInMedalium == false && tribeName != "Boss" && tribeName != "Untribe");
                    })
                    .Select(x => x.ParamHash)
                    .ToList();

                // Set area dictionary
                Dictionary<string, (int, int)> areas = new Dictionary<string, (int, int)>();

                // Browse folders containing map encounter data 
                foreach (string directoryName in Game.GetMapWhoContainsEncounter())
                {
                    // Count the total number of yokai from the area
                    int scoutableCount = 0;
                    int staticCount = 0;

                    // Get encounters
                    (IEncountTable[], IEncountChara[]) staticEncountersData = Game.GetMapEncounter(directoryName);
                    IEncountTable[] encountTables = staticEncountersData.Item1;
                    IEncountChara[] encountCharas = staticEncountersData.Item2;

                    foreach (IEncountChara encountChara in encountCharas)
                    {
                        if (scoutableYokai.Any(x => x.Item1.ParamHash == encountChara.ParamHash))
                        {
                            scoutableCount++;
                        }
                        else if (staticParamHashes.Contains(encountChara.ParamHash))
                        {
                            staticCount++;
                        }
                    }

                    areas.Add(directoryName, (scoutableCount, staticCount));
                }


                Dictionary<string, (List<int>, List<int>)> randomAreas = new Dictionary<string, (List<int>, List<int>)>();
                Dictionary<int, bool> usedParamHashes = new Dictionary<int, bool>();

                foreach (KeyValuePair<string, (int, int)> area in areas)
                {
                    List<int> scoutableHashes = new List<int>();
                    List<int> staticHashes = new List<int>();

                    // Generate random scoutable list
                    for (int i = 0; i < area.Value.Item1; i++)
                    {
                        int getProbability = Seed.Probability(new int[] { 25, 25, 20, 15, 9, 5, 1 });
                        List<(ICharaparam, ICharabase)> filtredScoutableYokai = new List<(ICharaparam, ICharabase)>();

                        if (getProbability == 0)
                        {
                            // E rank
                            filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 0).ToList();
                        }
                        else if (getProbability == 1)
                        {
                            // D rank
                            filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 1).ToList();
                        }
                        else if (getProbability == 2)
                        {
                            // C rank
                            filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 2).ToList();
                        }
                        else if (getProbability == 3)
                        {
                            // B rank
                            filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 3).ToList();
                        }
                        else if (getProbability == 4)
                        {
                            // A rank
                            filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 4).ToList();
                        }
                        else if (getProbability == 5)
                        {
                            // S rank
                            filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 5).ToList();
                        }
                        else
                        {
                            // Rare yokai
                            filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.IsRare == true).ToList();
                        }

                        int paramHash = 0x0;

                        // Select random yokai
                        if (filtredScoutableYokai.Count != 0)
                        {
                            paramHash = filtredScoutableYokai[Seed.Next(filtredScoutableYokai.Count())].Item1.ParamHash;
                        }
                        else
                        {
                            paramHash = scoutableYokai[Seed.Next(scoutableYokai.Count())].Item1.ParamHash;
                        }

                        if (paramHash != 0x00 && usedParamHashes.ContainsKey(paramHash) == false)
                        {
                            usedParamHashes.Add(paramHash, true);
                        }

                        scoutableHashes.Add(paramHash);
                    }

                    // Generate random static list
                    for (int i = 0; i < area.Value.Item2; i++)
                    {
                        staticHashes.Add(staticParamHashes[Seed.Next(staticParamHashes.Count)]);
                    }

                    randomAreas.Add(area.Key, (scoutableHashes, staticHashes));
                }

                // Some yokais are not used, so force them to be used
                if (usedParamHashes.Count() != scoutableYokai.Count())
                {
                    // Set banned indexes
                    Dictionary<string, List<int>> bannedIndexes = new Dictionary<string, List<int>>();

                    // Get a list of all unused yokais   
                    List<(ICharaparam, ICharabase)> unusedYokais = scoutableYokai.Where(x => usedParamHashes.ContainsKey(x.Item1.ParamHash) == false).ToList();

                    // get all areas who contains scoutable yokais
                    string[] areaNames = areas.Where(x => x.Value.Item1 > 0).Select(x => x.Key).ToArray();

                    for (int i = 0; i < unusedYokais.Count; i++)
                    {
                        // Get random area
                        string areaRandom = areaNames[Seed.Next(areaNames.Length)];

                        while (bannedIndexes.ContainsKey(areaRandom) && bannedIndexes[areaRandom].Count == randomAreas[areaRandom].Item1.Count)
                        {
                            areaRandom = areaNames[Seed.Next(areaNames.Length)];
                        }

                        // Get random index
                        int randomIndex = 0;
                        if (bannedIndexes.ContainsKey(areaRandom))
                        {
                            randomIndex = Seed.GetNumbers(0, randomAreas[areaRandom].Item1.Count, 1, bannedIndexes[areaRandom]).First();
                        }
                        else
                        {
                            randomIndex = Seed.Next(0, randomAreas[areaRandom].Item1.Count);
                            bannedIndexes.Add(areaRandom, new List<int>());
                        }

                        bannedIndexes[areaRandom].Add(randomIndex);

                        // Put the unused yokai
                        randomAreas[areaRandom].Item1[randomIndex] = unusedYokais[i].Item1.ParamHash;
                        usedParamHashes.Add(unusedYokais[i].Item1.ParamHash, true);
                    }
                }

                // Fix Area to prevent soft lock
                Game.FixArea(randomAreas);

                // Save randomized area
                foreach (KeyValuePair<string, (List<int>, List<int>)> randomArea in randomAreas)
                {
                    // Set index
                    int scoutableIndex = 0;
                    int staticIndex = 0;

                    // Get encounters
                    (IEncountTable[], IEncountChara[]) staticEncountersData = Game.GetMapEncounter(randomArea.Key);
                    IEncountTable[] encountTables = staticEncountersData.Item1;
                    IEncountChara[] encountCharas = staticEncountersData.Item2;

                    foreach (IEncountChara encountChara in encountCharas)
                    {
                        if (scoutableYokai.Any(x => x.Item1.ParamHash == encountChara.ParamHash))
                        {
                            encountChara.ParamHash = randomArea.Value.Item1[scoutableIndex];
                            scoutableIndex++;
                        }
                        else if (staticParamHashes.Contains(encountChara.ParamHash))
                        {
                            encountChara.ParamHash = randomArea.Value.Item2[staticIndex];
                            staticIndex++;
                        }

                        // Add Percentage Level Multiplicator
                        if (percentageLevel > 0)
                        {
                            if (encountChara.Level > 0)
                            {
                                int newLevel = encountChara.Level + Convert.ToInt32(encountChara.Level * percentageLevel / 100);

                                if (newLevel > 99)
                                {
                                    newLevel = 99;
                                }

                                encountChara.Level = newLevel;
                            }
                        }
                    }

                    // Save file
                    Game.SaveMapEncounter(randomArea.Key, encountTables, encountCharas);
                }
            }
        }

        public void RandomizeShop(bool randomize)
        {
            if (randomize == false) return;

            // Get valid items
            IItem[] autorizedItems = Items.Where(x => x.PurchasePrice > 0).ToArray();

            // Get all files starting with "shop_shp" from the specified directory
            var files = Game.Game.Directory.GetFolderFromFullPath(Game.Files["shop"].Path).Files.Keys;

            foreach (string shopFile in files.Where(x => x.StartsWith("shop_shp")))
            {
                (IShopConfig[], IShopValidCondition[]) shopData = Game.GetShop(shopFile);
                IShopConfig[] shopConfigs = shopData.Item1;
                IShopValidCondition[] validConditions = null;

                if (shopData.Item2 != null && shopData.Item2.Length > 0)
                {
                    validConditions = shopData.Item2;
                }

                // Generate random items
                int[] randomItemsIndex = Seed.GetNumbers(0, autorizedItems.Length, shopConfigs.Length).ToArray();

                for (int i = 0; i < shopConfigs.Length; i++)
                {
                    // Prevent to randomize key items
                    if (Items.Any(x => x.ItemHash == shopConfigs[i].ItemHash))
                    {
                        IItem randomItem = autorizedItems[randomItemsIndex[i]];
                        shopConfigs[i].ItemHash = randomItem.ItemHash;

                        if (shopConfigs[i].Price > 0)
                        {
                            shopConfigs[i].Price = randomItem.PurchasePrice;
                        }

                        if (validConditions != null && shopConfigs[i].ShopValidConditionIndex != -1)
                        {
                            validConditions[shopConfigs[i].ShopValidConditionIndex].Price = randomItem.PurchasePrice;
                        }
                    }
                }

                // Save
                Game.SaveShop(shopFile, shopConfigs, validConditions);
            }

            Game.FixShop();
        }

        public void RandomizeTreasureBox(bool randomize)
        {
            if (randomize == false) return;

            // Browse folders containing map encounter data 
            foreach (string directoryName in Game.GetMapWhoContainsTreasureBoxes())
            {
                IItableDataMore[] itableDataMores = Game.GetTreasureBox(directoryName);

                if (itableDataMores.Length < Items.Count && itableDataMores.Length < 10)
                {
                    // No dupe

                    int[] randomItemsIndex = Seed.GetNumbers(0, Items.Count, itableDataMores.Length).ToArray();

                    for (int i = 0; i < itableDataMores.Length; i++)
                    {
                        // Prevent to randomize key items
                        if (Items.Any(x => x.ItemHash == itableDataMores[i].ItemHash))
                        {
                            IItem randomItem = Items[randomItemsIndex[i]];
                            itableDataMores[i].ItemHash = randomItem.ItemHash;
                        }
                    }
                }
                else
                {
                    // Autorize dupe

                    for (int i = 0; i < itableDataMores.Length; i++)
                    {
                        // Prevent to randomize key items
                        if (Items.Any(x => x.ItemHash == itableDataMores[i].ItemHash))
                        {
                            IItem randomItem = Items[Seed.Next(0, Items.Count)];
                            itableDataMores[i].ItemHash = randomItem.ItemHash;
                        }
                    }
                }

                Game.SaveTreasureBox(directoryName, itableDataMores);
            }
        }

        public void RandomizeCrankKai(bool randomize)
        {
            if (randomize == false) return;

            // Get scoutable yokai
            List<(ICharaparam, ICharabase)> scoutableYokai = Charaparams
                .Where(x => x.ScoutableHash != 0x00
                            && x.ShowInMedalium == true
                            && Charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash) is ICharabase charaBase
                            && !charaBase.IsLegend
                            && !charaBase.IsPionner
                            && !charaBase.IsLegendaryMystery)
                .Select(x => (x, Charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash)))
                .Where(tuple => tuple.Item2 != null)
                .ToList();

            ICapsuleConfig[] capsuleConfigs = Game.GetCapsuleConfigs();

            for (int i = 0; i < capsuleConfigs.Length; i++)
            {
                ICapsuleConfig capsuleConfig = capsuleConfigs[i];

                if (Items.Any(x => x.ItemHash == capsuleConfig.ItemOrYokaiHash)) 
                {
                    capsuleConfig.ItemOrYokaiHash = Items[Seed.Next(Items.Count)].ItemHash;
                } else if (scoutableYokai.Any(x => x.Item1.ParamHash == capsuleConfig.ItemOrYokaiHash))
                {
                    int getProbability = Seed.Probability(new int[] { 25, 25, 20, 15, 9, 5, 1 });
                    List<(ICharaparam, ICharabase)> filtredScoutableYokai = new List<(ICharaparam, ICharabase)>();

                    if (getProbability == 0)
                    {
                        // E rank
                        filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 0).ToList();
                    }
                    else if (getProbability == 1)
                    {
                        // D rank
                        filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 1).ToList();
                    }
                    else if (getProbability == 2)
                    {
                        // C rank
                        filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 2).ToList();
                    }
                    else if (getProbability == 3)
                    {
                        // B rank
                        filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 3).ToList();
                    }
                    else if (getProbability == 4)
                    {
                        // A rank
                        filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 4).ToList();
                    }
                    else if (getProbability == 5)
                    {
                        // S rank
                        filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.Rank == 5).ToList();
                    }
                    else
                    {
                        // Rare yokai
                        filtredScoutableYokai = scoutableYokai.Where(x => x.Item2.IsRare == true).ToList();
                    }

                    // Select random yokai
                    if (filtredScoutableYokai.Count != 0)
                    {
                        capsuleConfig.ItemOrYokaiHash = filtredScoutableYokai[Seed.Next(filtredScoutableYokai.Count())].Item1.ParamHash;
                    }
                    else
                    {
                        capsuleConfig.ItemOrYokaiHash = scoutableYokai[Seed.Next(scoutableYokai.Count())].Item1.ParamHash;
                    }
                }
            }

            // Save
            Game.SaveCapsuleConfigs(capsuleConfigs);
        }

        public void RandomizeLegendary(bool randomize, bool randomizeRequirments)
        {
            if (randomize == false) return;

            // Get scoutable param hash
            List<int> charaparamHashes = Charaparams
                .Where(x => x.ScoutableHash != 0x00
                            && x.ShowInMedalium == true
                            && x.EvolveOffset == -1)
                .Select(x => x.ParamHash)
                .ToList();

            // Reset Scout
            foreach (ICharaparam charaparam in Charaparams
                .Where(x => {
                    ICharabase charabase = Charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash);
                    return charabase != null && (charabase.IsLegend || charabase.IsPionner || charabase.IsLegendaryMystery);
                }))
            {
                if (Game.Name == "Yo-Kai Watch 1")
                {
                    charaparam.ScoutableHash = 0x13345632;
                } else if (Game.Name == "Yo-Kai Watch 2")
                {
                    charaparam.ScoutableHash = 0x00654331;
                }
            }

            // Reset legends
            foreach (ICharabase charabase in Charabases)
            {
                charabase.IsLegend = false;
                charabase.IsLegendaryMystery = false;
                charabase.IsPionner = false;
            }

            ILegendDataConfig[] legendDataConfigs = Game.GetLegends();

            foreach(ILegendDataConfig legendDataConfig in legendDataConfigs)
            {
                // Get random yokai
                int index = Seed.Next(charaparamHashes.Count);
                ICharaparam randomCharaparam = Charaparams.FirstOrDefault(x => x.ParamHash == charaparamHashes[index]);
                ICharabase randomCharabase = Charabases.FirstOrDefault(x => x.BaseHash == randomCharaparam.BaseHash);

                // Edit legend statut
                if (Game.Name == "Yo-Kai Watch 3" || Game.Name == "Yo-Kai Watch Busters 2")
                {
                    int getProbability = Seed.Probability(new int[] { 70, 20, 10 });
                    List<(ICharaparam, ICharabase)> filtredScoutableYokai = new List<(ICharaparam, ICharabase)>();

                    if (getProbability == 0)
                    {
                        randomCharabase.IsLegend = true;
                    }
                    else if (getProbability == 1)
                    {
                        randomCharabase.IsLegendaryMystery = true;
                    }
                    else if (getProbability == 2)
                    {
                        randomCharabase.IsPionner = true;
                    }
                } 
                else
                {
                    randomCharabase.IsLegend = true;
                }

                // Remove rare status
                randomCharabase.IsRare = false;

                // Make unscoutable the yokai
                randomCharaparam.ScoutableHash = 0x0;

                legendDataConfig.YokaiHash = randomCharaparam.ParamHash;

                // Remove
                charaparamHashes.RemoveAt(index);

                if (randomizeRequirments)
                {
                    int[] randomRequirementParamHashes = Seed.GetNumbers(0, charaparamHashes.Count, 8).Select(x => charaparamHashes[x]).ToArray();

                    // Randomize requirments
                    legendDataConfig.Seal1Hash = randomRequirementParamHashes[0];
                    legendDataConfig.Seal2Hash = randomRequirementParamHashes[1];
                    legendDataConfig.Seal3Hash = randomRequirementParamHashes[2];
                    legendDataConfig.Seal4Hash = randomRequirementParamHashes[3];
                    legendDataConfig.Seal5Hash = randomRequirementParamHashes[4];
                    legendDataConfig.Seal6Hash = randomRequirementParamHashes[5];
                    legendDataConfig.Seal7Hash = randomRequirementParamHashes[6];
                    legendDataConfig.Seal8Hash = randomRequirementParamHashes[7];

                    // Enable spoil
                    legendDataConfig.SpoilSeal1 = true;
                    legendDataConfig.SpoilSeal2 = true;
                    legendDataConfig.SpoilSeal3 = true;
                    legendDataConfig.SpoilSeal4 = true;
                    legendDataConfig.SpoilSeal5 = true;
                    legendDataConfig.SpoilSeal6 = true;
                    legendDataConfig.SpoilSeal7 = true;
                    legendDataConfig.SpoilSeal8 = true;

                    // Remove used hashes
                    for (int i = 0; i < 8; i++)
                    {
                        charaparamHashes.Remove(randomRequirementParamHashes[i]);
                    }
                }
            }

            Game.SaveLegends(legendDataConfigs);
        }

        public void RandomizeGiven(bool randomize, int[] starters)
        {
            if (randomize == false) return;

            // Get scoutable yokai
            List<int> scoutableYokai = Charaparams
                .Where(x => x.ScoutableHash != 0x00
                            && x.ShowInMedalium == true
                            && Charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash) is ICharabase charaBase
                            && !charaBase.IsLegend
                            && !charaBase.IsPionner
                            && !charaBase.IsLegendaryMystery)
                .Select(x => x.ParamHash)
                .ToList();

            // get all yokais
            int[] allYokais = Charaparams
             .Where(charaparam =>
             {
                 ICharabase charabase = Charabases.FirstOrDefault(x => x.BaseHash == charaparam.BaseHash);
                 string tribeName = (Game.Name == "Yo-Kai Watch 1") ? Game.Tribes[charaparam.Tribe] : Game.Tribes[charabase.Tribe];
                 return (charaparam.ScoutableHash != 0x00 && charaparam.ShowInMedalium == true)
                        || (charaparam.ScoutableHash == 0x00 && charaparam.ShowInMedalium == true && tribeName != "Boss" && tribeName != "Untribe");
             })
             .Select(x => x.ParamHash)
             .ToArray();

            int fileCount = 0;

            foreach (KeyValuePair<string, SubMemoryStream> file in Game.Game.Directory.GetFolderFromFullPath("/seq/phs").Files)
            {
                int yokaiCount = 0;

                if (file.Value.ByteContent == null)
                {
                    file.Value.Read();
                }

                XQuerry xQuerry = new XQuerry(file.Value.ByteContent);

                byte[] output = new byte[xQuerry.Content["Argument"].Length];
                using (BinaryDataReader reader = new BinaryDataReader(xQuerry.Content["Argument"]))
                using (BinaryDataWriter writer = new BinaryDataWriter(output))
                {
                    while (reader.Position < reader.Length)
                    {
                        int bytesInt = reader.ReadValue<int>();

                        // Check if the bytes int read is a charaparam hash
                        if (Charaparams.Any(x => x.ParamHash == bytesInt))
                        {
                            if ((Game.Name == "Yo-Kai Watch 1" || Game.Name == "Yo-Kai Watch 2") && fileCount == 0 && yokaiCount == 0 && starters[0] != -1)
                            {
                                // Change the starter
                                bytesInt = allYokais[starters[0]];
                                if (scoutableYokai.Contains(bytesInt))
                                {
                                    scoutableYokai.Remove(bytesInt);
                                }
                            }
                            else if (Game.Name == "Yo-Kai Watch 1" && fileCount == 1 && yokaiCount == 0)
                            {
                                // Don't swap jibanyan
                                if (scoutableYokai.Contains(bytesInt))
                                {
                                    scoutableYokai.Remove(bytesInt);
                                }
                            }
                            else
                            {
                                // Change the bytes read by a new yokai
                                int randomIndex = Seed.Next(scoutableYokai.Count);
                                bytesInt = scoutableYokai[randomIndex];
                                scoutableYokai.RemoveAt(randomIndex);
                            }
                            yokaiCount++;
                        }

                        writer.Write(bytesInt);
                    }
                }

                // Replace
                xQuerry.Content["Argument"] = output;

                // Save file
                file.Value.ByteContent = xQuerry.Save();

                fileCount++;
            }
        }

        public void RemoveUnscoutableYokai(bool randomize)
        {
            if (randomize == false) return;

            if (Game.Name == "Yo-Kai Watch 1" || Game.Name == "Yo-Kai Watch 2")
            {
                Game.UnlockUnscoutableYokai(Charaparams, Charabases, Charascales);
            }

        }

        public string[] GetPlayableYokai(bool autorizeUnscoutableYokai)
        {
            List<ICharabase> charabasesCloned = Charabases.Select(charaBase => (ICharabase)charaBase.Clone()).ToList();
            List<ICharaparam> charaparamsCloned = Charaparams.Select(charaParam => (ICharaparam)charaParam.Clone()).ToList();
            List<ICharascale> charascalesCloned = Charascales.Select(charaScale => (ICharascale)charaScale.Clone()).ToList();

            if (autorizeUnscoutableYokai)
            {
                Game.UnlockUnscoutableYokai(charaparamsCloned, charabasesCloned, charascalesCloned, null, null);
            }

            return GetNames(charaparamsCloned
                .Where(charaparam =>
                {
                    ICharabase charabase = charabasesCloned.FirstOrDefault(x => x.BaseHash == charaparam.BaseHash);
                    string tribeName = (Game.Name == "Yo-Kai Watch 1") ? Game.Tribes[charaparam.Tribe] : Game.Tribes[charabase.Tribe];
                    return (charaparam.ScoutableHash != 0x00 && charaparam.ShowInMedalium == true)
                           || (charaparam.ScoutableHash == 0x00 && charaparam.ShowInMedalium == true && tribeName != "Boss" && tribeName != "Untribe");
                })
                .ToArray(), charabasesCloned.ToArray());
        }
    }
}
