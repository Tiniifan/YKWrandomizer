using System;
using System.IO;
using System.Linq;
using System.Drawing;
using YKWrandomizer.Logic;
using YKWrandomizer.Tools;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace YKWrandomizer.YokaiWatch
{
    public class YW
    {
        public string Name;

        private string Directory;

        private IDictionary<UInt32, Item> Items;

        public IDictionary<UInt32, Yokai> Yokais;

        private IDictionary<UInt32, Move> Skills;

        private IDictionary<UInt32, Move> Attacks;

        private IDictionary<UInt32, Move> Inspirits;

        private IDictionary<UInt32, Move> Techniques;

        private IDictionary<UInt32, Move> Soultimates;

        private List<BossYokai> BossYokais;

        private IDictionary<string, List<uint>> GivenYokais;

        private IDictionary<string, List<uint>> StaticYokais;

        private int LegendaryYokaisNumber;

        private Dictionary<int, Evolution> Evolutions;

        public List<int> MedalliumOrder;

        private List<Tribe> Tribes;

        private List<Rank> Ranks = new List<Rank>() { Rank.E(), Rank.D(), Rank.C(), Rank.B(), Rank.A(), Rank.S() };

        public YW(string _Name, string _Directory)
        {
            switch (_Name)
            {
                case "yw1":
                    Items = Common.Items.YW1;
                    Yokais = Common.Yokais.YW1;
                    Skills = Common.Skills.YW1;
                    Attacks = Common.Attacks.YW1;
                    Inspirits = Common.Inspirits.YW1;
                    Techniques = Common.Techniques.YW1;
                    Soultimates = Common.Soultimates.YW1;
                    BossYokais = Common.BossYokais.YW1;
                    GivenYokais = Common.GivenYokais.YW1;
                    StaticYokais = Common.StaticYokais.YW1;
                    MedalliumOrder = Common.Medalliums.YW1;
                    Tribes = Common.Tribes.YW1;
                    LegendaryYokaisNumber = 5;
                    break;
                default:
                    throw new FormatException("Unrecognized game");
            }

            Name = _Name;
            Directory = _Directory;

            LoadYokais();
        }

        private void LoadYokais()
        {
            // Initialize File Reader
            DataReader charabaseReader = new DataReader(File.ReadAllBytes(Directory + "/" + Name + "_a_fa/data/res/character/chara_base_0.04j.cfg.bin"));
            DataReader charaparamReader = new DataReader(File.ReadAllBytes(Directory + "/" + Name + "_a_fa/data/res/character/chara_param_0.02.cfg.bin"));

            if (Name == "yw1")
            {
                // Create Dictionary Evolution
                charaparamReader.Seek(0x1A0C8);
                charaparamReader.Skip(0x10);
                Evolutions = new Dictionary<int, Evolution>();
                int evolutionCount = charaparamReader.ReadInt32();
                for (int e = 0; e < evolutionCount; e++)
                {
                    charaparamReader.Skip(0x08);
                    int evolutionLevel = charaparamReader.ReadInt32();
                    UInt32 yokaiID = charaparamReader.Reverse(charaparamReader.ReadUInt32());
                    Evolutions.Add(e, new Evolution(Yokais[yokaiID], evolutionLevel));
                }

                // Load Yokais
                charaparamReader.Seek(0x1C);
                for (int i = 0; i < Yokais.Count; i++)
                {
                    charabaseReader.Seek(0x2BE0);
                    charaparamReader.Skip(0x14);
                    UInt32 charaparamID = charaparamReader.Reverse(charaparamReader.ReadUInt32());
                    UInt32 charabaseID = charaparamReader.Reverse(charaparamReader.ReadUInt32());
                    UInt32 tryCharabaseID = 0x0;

                    if (Yokais[charaparamID].Status.Name != "NPC")
                    {
                        while (charabaseID != tryCharabaseID)
                        {
                            charabaseReader.Skip(0x0C);
                            tryCharabaseID = charabaseReader.Reverse(charabaseReader.ReadUInt32());
                            charabaseReader.Skip(0x5C);
                        }

                        charabaseReader.Seek((uint)(charabaseReader.BaseStream.Position - 0x5C));

                        // Tribe
                        switch (charaparamReader.Read())
                        {
                            case 0x01:
                                Yokais[charaparamID].Tribe = Tribe.Brave();
                                break;
                            case 0x02:
                                Yokais[charaparamID].Tribe = Tribe.Mysterious();
                                break;
                            case 0x03:
                                Yokais[charaparamID].Tribe = Tribe.Tough();
                                break;
                            case 0x04:
                                Yokais[charaparamID].Tribe = Tribe.Charming();
                                break;
                            case 0x05:
                                Yokais[charaparamID].Tribe = Tribe.Heartful();
                                break;
                            case 0x06:
                                Yokais[charaparamID].Tribe = Tribe.Shady();
                                break;
                            case 0x07:
                                Yokais[charaparamID].Tribe = Tribe.Eerie();
                                break;
                            case 0x08:
                                Yokais[charaparamID].Tribe = Tribe.Slippery();
                                break;
                            case 0x09:
                                Yokais[charaparamID].Tribe = Tribe.Wicked();
                                break;
                            default:
                                Yokais[charaparamID].Tribe = Tribe.NoTribe();
                                break;
                        }

                        // Base Stat
                        charaparamReader.Skip(0x03);
                        for (int s = 0; s < 5; s++)
                        {
                            Yokais[charaparamID].BaseStat[s] = charaparamReader.ReadInt32();
                        }

                        // Unknown Value Maybe Element?
                        charaparamReader.Skip(0x4);

                        // Moveset
                        for (int m = 0; m < 3; m++)
                        {
                            switch (m)
                            {
                                case 0:
                                    Yokais[charaparamID].Moveset[0] = Attacks[charaparamReader.Reverse(charaparamReader.ReadUInt32())];
                                    break;
                                case 1:
                                    Yokais[charaparamID].Moveset[1] = Techniques[charaparamReader.Reverse(charaparamReader.ReadUInt32())];
                                    break;
                                case 2:
                                    Yokais[charaparamID].Moveset[2] = Inspirits[charaparamReader.Reverse(charaparamReader.ReadUInt32())];
                                    break;
                            }

                            charaparamReader.Skip(0x4);
                        }
                        charaparamReader.Skip(0x24);
                        Yokais[charaparamID].Moveset[3] = Soultimates[charaparamReader.Reverse(charaparamReader.ReadUInt32())];

                        // Unknown Values
                        charaparamReader.Skip(0x0C);

                        // Skill
                        Yokais[charaparamID].Skill = Skills[charaparamReader.Reverse(charaparamReader.ReadUInt32())];

                        // Money And Experience
                        Yokais[charaparamID].Money = Convert.ToInt32(charaparamReader.ReadUInt32());
                        Yokais[charaparamID].Experience = Convert.ToInt32(charaparamReader.ReadUInt32());

                        // Drops
                        for (int d = 0; d < 2; d++)
                        {
                            UInt32 itemID = charaparamReader.Reverse(charaparamReader.ReadUInt32());

                            if (Items.ContainsKey(itemID))
                            {
                                Yokais[charaparamID].Drops[d] = Items[itemID];
                            }
                            else
                            {
                                Yokais[charaparamID].Drops[d] = new Item(" ");
                            }

                            charaparamReader.Skip(0x04);
                        }

                        // Evolution
                        charaparamReader.Skip(0x34);
                        int tryEvolution = charaparamReader.ReadInt32();
                        if (tryEvolution > -1 & tryEvolution < Evolutions.Count)
                        {
                            Yokais[charaparamID].Evolution = Evolutions[tryEvolution];
                        }

                        // Rank
                        charabaseReader.Skip(0x28);
                        switch (charabaseReader.ReadInt32())
                        {
                            case 0x01:
                                Yokais[charaparamID].Rank = Rank.D();
                                break;
                            case 0x02:
                                Yokais[charaparamID].Rank = Rank.C();
                                break;
                            case 0x03:
                                Yokais[charaparamID].Rank = Rank.B();
                                break;
                            case 0x04:
                                Yokais[charaparamID].Rank = Rank.A();
                                break;
                            case 0x05:
                                Yokais[charaparamID].Rank = Rank.S();
                                break;
                            default:
                                Yokais[charaparamID].Rank = Rank.E();
                                break;
                        }

                        // Rarity
                        if (charabaseReader.ReadInt32() == 0x01)
                        {
                            Yokais[charaparamID].Rarity = Rarity.Rare();
                        }
                        else
                        {
                            Yokais[charaparamID].Rarity = Rarity.Normal();
                        }

                        // Legendary
                        Yokais[charaparamID].IsLegendary = charabaseReader.ReadInt32() == 0x01;

                        // Next Yokai
                        charaparamReader.Skip(0x08);
                    } else
                    {
                        charaparamReader.Skip(0xC4);
                    }
                }
            }

            // Close Files
            charabaseReader.Close();
            charaparamReader.Close();
        }

        private void WriteYokais()
        {
            // Initialise Face Icon Map
            Bitmap faceIcon = new Bitmap(Image.FromStream(new ResourceReader(Name + "_face_icon.png").GetResourceStream()));

            // Initialize File Reader and File Writer
            DataReader charabaseReader = new DataReader(File.ReadAllBytes(Directory + "/" + Name + "_a_fa/data/res/character/chara_base_0.04j.cfg.bin"));
            DataReader charaparamReader = new DataReader(File.ReadAllBytes(Directory + "/" + Name + "_a_fa/data/res/character/chara_param_0.02.cfg.bin"));
            DataWriter charabaseWriter = new DataWriter(Directory + "/" + Name + "_a_fa/data/res/character/chara_base_0.04j.cfg.bin");
            DataWriter charaparamWriter = null;

            if (Name == "yw1")
            {
                // Save Evolution
                byte[] evolutionByteArray = new byte[20 + 16 * Evolutions.Count];
                DataWriter evolutionWriter = new DataWriter(evolutionByteArray);
                evolutionWriter.Write(new byte[16] { 0x9D, 0xC9, 0xCA, 0xB7, 0x00, 0xFF, 0xFF, 0xFF, 0xCB,  0x81, 0x61,  0x4C, 0x01, 0x01, 0xFF, 0xFF });
                evolutionWriter.WriteInt32(Evolutions.Count);
                for (int e = 0; e < Evolutions.Count; e++)
                {
                    evolutionWriter.Write(new byte[8] { 0xEF, 0xC0, 0x48, 0x14, 0x02, 0x05, 0xFF, 0xFF });
                    evolutionWriter.WriteInt32(Evolutions[e].Level);
                    evolutionWriter.WriteUInt32(Yokais.FirstOrDefault(x => x.Value == Evolutions[e].EvolutionTo).Key);
                }

                // Create charaparamWriter
                byte[] charaparam = new byte[107708 + evolutionByteArray.Length];
                charaparamWriter = new DataWriter(charaparam);
                charaparamWriter.Write(charaparamReader.GetSection(0x0, 106696));
                charaparamWriter.Write(evolutionByteArray);
                charaparamWriter.Write(charaparamReader.GetSection(0x1A1CC, 1012));
                charaparamWriter.Seek(0x0);
                charaparamWriter.WriteInt32(689 + Evolutions.Count);
                charaparamWriter.WriteInt32(106696 + evolutionByteArray.Length + 436);

                // Save Yokais
                charaparamReader.Seek(0x1C);
                for (int i = 0; i < Yokais.Count; i++)
                {
                    charabaseReader.Seek(0x2BE0);
                    charaparamReader.Skip(0x14);
                    UInt32 charaparamID = charaparamReader.Reverse(charaparamReader.ReadUInt32());
                    UInt32 charabaseID = charaparamReader.Reverse(charaparamReader.ReadUInt32());
                    UInt32 tryCharabaseID = 0x0;

                    if (Yokais[charaparamID].Status.Name != "NPC")
                    {

                        while (charabaseID != tryCharabaseID)
                        {
                            charabaseReader.Skip(0x0C);
                            tryCharabaseID = charabaseReader.Reverse(charabaseReader.ReadUInt32());
                            charabaseReader.Skip(0x5C);
                        }

                        charaparamWriter.Seek((uint)charaparamReader.BaseStream.Position);
                        charabaseWriter.Seek((uint)(charabaseReader.BaseStream.Position - 0x5C));

                        // Tribe
                        charaparamWriter.WriteInt32(Yokais[charaparamID].Tribe.ID);

                        // Base Stat
                        for (int s = 0; s < 5; s++)
                        {
                            charaparamWriter.WriteInt32(Yokais[charaparamID].BaseStat[s]);
                        }

                        // Unknown Value Maybe Element?
                        charaparamWriter.Skip(0x4);

                        // Moveset
                        for (int m = 0; m < 3; m++)
                        {             
                            switch (m)
                            {
                                case 0:
                                    charaparamWriter.WriteUInt32(Attacks.FirstOrDefault(x => x.Value == Yokais[charaparamID].Moveset[0]).Key);
                                    break;
                                case 1:
                                    charaparamWriter.WriteUInt32(Techniques.FirstOrDefault(x => x.Value == Yokais[charaparamID].Moveset[1]).Key);
                                    break;
                                case 2:
                                    charaparamWriter.WriteUInt32(Inspirits.FirstOrDefault(x => x.Value == Yokais[charaparamID].Moveset[2]).Key);
                                    break;
                            }

                            charaparamWriter.Skip(0x4);
                        }

                        // Friendship 1
                        if (i > 223 & Yokais[charaparamID].BeFriend == true)
                        {
                            charaparamWriter.Skip(0x0c);
                            charaparamWriter.Write(new byte[24] { 0x01, 0x00, 0x00, 0x00, 0x66, 0x66, 0xA6, 0x3F, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x33, 0x33, 0x33, 0x3F });
                        } 
                        else
                        {
                            charaparamWriter.Skip(0x24);
                        }

                        // Soultimate
                        charaparamWriter.WriteUInt32(Soultimates.FirstOrDefault(x => x.Value == Yokais[charaparamID].Moveset[3]).Key);

                        // Friendship 2
                        if (i > 223 & Yokais[charaparamID].BeFriend == true)
                        {
                            charaparamWriter.Write(new byte[12] { 0x00, 0x00, 0x00, 0x1C, 0x56, 0xEB, 0xEF, 0x32, 0x67, 0x55, 0x24, 0xEB });
                        } else
                        {
                            charaparamWriter.Skip(0x0C);
                        }

                        // Skill
                        charaparamWriter.WriteUInt32(Skills.FirstOrDefault(x => x.Value == Yokais[charaparamID].Skill).Key);

                        // Money And Experience
                        charaparamWriter.WriteInt32(Yokais[charaparamID].Money);
                        charaparamWriter.WriteInt32(Yokais[charaparamID].Experience);

                        // Drops
                        for (int d = 0; d < 2; d++)
                        {
                            charaparamWriter.WriteUInt32(Items.FirstOrDefault(x => x.Value == Yokais[charaparamID].Drops[d]).Key);
                            charaparamWriter.Skip(0x04);
                        }

                        // Friendship 3
                        if (i > 223 & Yokais[charaparamID].BeFriend == true)
                        {
                            charaparamWriter.Skip(0x20);
                            charaparamWriter.Write(new byte[4] { 0xF7, 0xCD, 0x90, 0xA6 });
                            charaparamWriter.Skip(0x10);
                        }
                        else
                        {
                            charaparamWriter.Skip(0x34);
                        }

                        // Evolution
                        if (Yokais[charaparamID].Evolution != null)
                        {
                            charaparamWriter.WriteInt32(Evolutions.FirstOrDefault(x => x.Value == Yokais[charaparamID].Evolution).Key);
                        } else
                        {
                            charaparamWriter.Write(new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF });
                        }

                        if (Yokais[charaparamID].Status.Name == "Normal")
                        {
                            // Rank
                            charabaseWriter.Skip(0x28);
                            charabaseWriter.WriteInt32(Yokais[charaparamID].Rank.ID);
                            charabaseWriter.WriteInt32(Yokais[charaparamID].Rarity.ID);
                            charabaseWriter.WriteInt32(Convert.ToInt32(Yokais[charaparamID].IsLegendary));

                            // Draw Medals
                            if (i < 223)
                            {
                                Image newMedal = null;

                                if (Yokais[charaparamID].IsLegendary)
                                {
                                    newMedal = Image.FromStream(new ResourceReader("y_medal_lege0" + Yokais[charaparamID].Tribe.ID + ".png").GetResourceStream());
                                }
                                else if (Yokais[charaparamID].Rarity.ID == 0x1)
                                {
                                    newMedal = Image.FromStream(new ResourceReader("y_medal_rare0" + Yokais[charaparamID].Tribe.ID + ".png").GetResourceStream());
                                }
                                else
                                {
                                    newMedal = Image.FromStream(new ResourceReader("y_medal_nml0" + Yokais[charaparamID].Tribe.ID + ".png").GetResourceStream());
                                }

                                faceIcon = Draw.DrawImage(faceIcon, MedalliumOrder[i] % 23 * 44, MedalliumOrder[i] / 23 * 44, newMedal);
                            }
                        }


                    }

                    // Next Yokai
                    charaparamReader.Skip(0xC4);
                }

                // Save charaparam
                File.WriteAllBytes(Directory + "/" + Name + "_a_fa/data/res/character/chara_param_0.02.cfg.bin", charaparam);
                faceIcon.Save(Directory + "/yw1_a_fa/data/menu/face_icon/face_icon.png", ImageFormat.Png);
            }

            // Close Files
            charabaseReader.Close();
            charaparamReader.Close();
            charabaseWriter.Close();
            charaparamWriter.Close();
        }

        private void GenerateYokais(params int[] options)
        {
            // Possible Yokai For The Evolution
            List<Yokai> possibleYokaiEvolve = new List<Yokai>();
            if (options[13] == 0)
            {
                possibleYokaiEvolve = Yokais.Where(x => x.Value.Status.Name == "Normal").Select(x => x.Value).ToList();
            }
            else
            {
                possibleYokaiEvolve = Yokais.Where(x => x.Value.Status.Name != "MiniBoss").Select(x => x.Value).ToList();
            }

            // Generate Random Evolution
            if (options[4] == 1)
            {
                for (int i = 0; i < Yokais.Count; i++)
                {
                    Yokai yokai = Yokais.ElementAt(i).Value;

                    if (yokai.Evolution != null)
                    {
                        yokai.Evolution.EvolutionTo = possibleYokaiEvolve[new RandomNumber(0, possibleYokaiEvolve.Count).GetNumber()];
                    }
                }
            }
            else if (options[4] == 2)
            {
                for (int i = 0; i < Yokais.Count; i++)
                {
                    Yokai yokai = Yokais.ElementAt(i).Value;

                    if (options[13] == 0 && yokai.Status.Name != "Normal") continue;

                    int randomNumber = new RandomNumber(0, 101).GetNumber();
                    if (randomNumber < 20)
                    {
                        yokai.Evolution = new Evolution(possibleYokaiEvolve[new RandomNumber(0, possibleYokaiEvolve.Count).GetNumber()], new RandomNumber(1, 36).GetNumber());
                    }
                    else
                    {
                        yokai.Evolution = null;
                    }
                }
            }

            // Randomize Yokai-Traits
            for (int i = 0; i < Yokais.Count; i++)
            {
                Yokai yokai = Yokais.ElementAt(i).Value;

                if (yokai.Status.Name == "Normal" || yokai.Status.Name == "MiniBoss")
                {
                    // Randomize Tribe
                    if (options[0] != 0)
                    {
                        yokai.Tribe = Tribes[new RandomNumber(0, Tribes.Count).GetNumber()];
                    }

                    // Randomize Rank
                    if (options[1] != 0)
                    {
                        yokai.Rank = Ranks[new RandomNumber(0, Ranks.Count).GetNumber()];
                    }

                    // Randomize Rarity
                    if (options[2] != 0)
                    {
                        if (new RandomNumber(0, 101).GetNumber() < 95)
                        {
                            yokai.Rarity = Rarity.Normal();
                        }
                        else
                        {
                            yokai.Rarity = Rarity.Rare();
                        }
                    }

                    // Randomize Drop
                    if (options[3] == 1)
                    {
                        for (int drop = 0; drop < 2; drop++)
                        {
                            yokai.Drops[drop] = Items.ElementAt(new RandomNumber(0, Items.Count).GetNumber()).Value;
                        }
                    }

                    // Skills
                    if (options[5] == 1)
                    {
                        yokai.Skill = Skills.ElementAt(new RandomNumber(1, Skills.Count).GetNumber()).Value;
                    }

                    // Attacks
                    if (options[6] != 0)
                    {
                        yokai.Moveset[0] = Attacks.ElementAt(new RandomNumber(1, Attacks.Count).GetNumber()).Value;
                    }

                    // Techniques
                    if (options[7] != 0)
                    {
                        yokai.Moveset[1] = Techniques.ElementAt(new RandomNumber(1, Techniques.Count).GetNumber()).Value;
                    }

                    // Inspirit
                    if (options[8] != 0)
                    {
                        yokai.Moveset[2] = Inspirits.ElementAt(new RandomNumber(1, Inspirits.Count).GetNumber()).Value;
                    }

                    // Soultimate
                    if (options[9] != 0)
                    {
                        yokai.Moveset[3] = Soultimates.ElementAt(new RandomNumber(1, Soultimates.Count).GetNumber()).Value;
                    }
                }

                if (yokai.Status.Name == "Normal")
                {
                    // Base Stat
                    if (options[10] != 0)
                    {
                        yokai.NewStat(25, 38);
                    }

                    // Scale Money
                    if (options[11] == 1)
                    {
                        if (yokai.Money > 35)
                        {
                            yokai.Money = 5 + (yokai.Rank.ID + 1) * 6;
                        }
                    }

                    // Scale Experience
                    if (options[12] == 1)
                    {
                        if (yokai.Experience > 46)
                        {
                            yokai.Money = 26 + (yokai.Rank.ID + 1) * 4;
                        }
                    }
                }

                if (yokai.Status.Name == "Normal" || yokai.Status.Name == "Boss")
                {
                    // Enable Friendly
                    if (options[13] == 1)
                    {
                        yokai.BeFriend = true;
                    }
                }
            }

            // Fix Evolution
            for (int i = 0; i < Yokais.Count; i++)
            {
                Yokai yokai = Yokais.ElementAt(i).Value;

                if (yokai.Evolution != null)
                {
                    // Fix Tribe
                    if (options[0] == 1)
                    {
                        yokai.Tribe = yokai.Evolution.EvolutionTo.Tribe;
                    }

                    // Fix Rank
                    if (options[1] == 1)
                    {
                        yokai.Rank = Ranks[new RandomNumber(yokai.Evolution.EvolutionTo.Rank.ID, Ranks.Count).GetNumber()];
                    }

                    // Fix Rarity
                    if (options[2] == 1)
                    {
                        yokai.Rarity = yokai.Evolution.EvolutionTo.Rarity;

                        // Fix Legendary
                        if (yokai.Evolution.EvolutionTo.IsLegendary == true)
                        {
                            yokai.IsLegendary = true;
                        }
                    }

                    // Fix Attack
                    if (options[6] == 1)
                    {
                        yokai.Moveset[0] = yokai.Evolution.EvolutionTo.Moveset[0];
                    }

                    // Fix Technique
                    if (options[7] == 1)
                    {
                        yokai.Moveset[1] = yokai.Evolution.EvolutionTo.Moveset[1];
                    }

                    // Fix Inspirit
                    if (options[8] == 1)
                    {
                        yokai.Moveset[2] = yokai.Evolution.EvolutionTo.Moveset[2];
                    }

                    // Fix Soultimate
                    if (options[9] == 1)
                    {
                        yokai.Moveset[3] = yokai.Evolution.EvolutionTo.Moveset[3];
                    }

                    // Randomize Stat Again Because The Rank Has Changed
                    if (options[10] == 1)
                    {
                        yokai.NewStat(25, 38);
                    }

                }
            }

            // Fix Buhu bug
            Yokais[0x2DD24384].Rank = Rank.E();
        }

        public void RandomizeYokais(params int[] options)
        {
            GenerateYokais(options);
            WriteYokais();
        }

        public void RandomizeWild(bool randomize, bool allowBoss, bool allYokai, decimal percentageLevel)
        {
            if (randomize == false) return;

            // Create Scoutable Yokais Dictionary
            Dictionary<UInt32, Yokai> yokaisScoutable = new Dictionary<UInt32, Yokai>();
            if (allowBoss == true)
            {
                yokaisScoutable = Yokais.Where(x => x.Value.Status.Name == "Normal" || x.Value.Status.Name == "Boss").ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                yokaisScoutable = Yokais.Where(x => x.Value.Status.Name == "Normal").ToDictionary(x => x.Key, x => x.Value);
            }

            // Create Dictionary with E-Rank Yokai
            Dictionary<UInt32, Yokai> poorYokais = yokaisScoutable.Where(x => x.Value.Rank.Name == "E").ToDictionary(x => x.Key, x => x.Value);

            // Find All Files Who Contains Wild Entry
            string[] folders = System.IO.Directory.GetDirectories(Directory + "/" + Name + "_a_fa/data/res/map/").Select(Path.GetFileName).ToArray();
            foreach (string folder in folders)
            {
                if (File.Exists(Directory + "/" + Name + "_a_fa/data/res/map/" + folder + "/" + folder + "_enc_0.02m.cfg.bin"))
                {
                    // Initialize File Reader and File Writer
                    DataReader wildReader = new DataReader(File.ReadAllBytes(Directory + "/" + Name + "_a_fa/data/res/map/" + folder + "/" + folder + "_enc_0.02m.cfg.bin"));
                    DataWriter wildWriter = new DataWriter(Directory + "/" + Name + "_a_fa/data/res/map/" + folder + "/" + folder + "_enc_0.02m.cfg.bin");

                    // Randomize Wild
                    for (int position = 0; position < wildReader.Length; position += 4)
                    {
                        // Check If The Wild Has Yokai
                        if (wildReader.ReadUInt32() == 0xE43FA5D2)
                        {
                            wildReader.Skip(0x0C);
                            int wildCount = wildReader.ReadInt32();

                            // Number Yokais In The Area
                            for (int i = 0; i < wildCount; i++)
                            {
                                // Get Random Yo-Kai
                                wildWriter.Seek((uint)wildReader.BaseStream.Position);
                                wildWriter.Skip(0x08);

                                if (i < 6)
                                {
                                    wildWriter.WriteUInt32(poorYokais.ElementAt(new RandomNumber(0, poorYokais.Count).GetNumber()).Key);
                                }
                                else
                                {
                                    wildWriter.WriteUInt32(yokaisScoutable.ElementAt(new RandomNumber(0, yokaisScoutable.Count).GetNumber()).Key);
                                }

                                // Get Level Of The Yo-Kai
                                wildReader.Skip(0x0C);
                                int level = wildReader.ReadByte();
                                wildReader.Skip(0x17);

                                // Add Percentage Level Multiplicator
                                if (percentageLevel > 0)
                                {
                                    if (level > 0)
                                    {
                                        level = Convert.ToInt32(level + (percentageLevel * level / 100));
                                        if (level > 99) level = 99;
                                        wildWriter.WriteByte(level);
                                    }
                                }
                            }

                            break;
                        }
                    }

                    // Close File
                    wildReader.Close();
                    wildWriter.Close();
                }
            }

            // Unlock All Yokais
            if (allYokai == true)
            {
                using (var fileStream = File.Create(Directory + "/" + Name + "_a_fa/data/res/map/watchmap_common_0.12e.cfg.bin"))
                {
                    new ResourceReader(Name + "_Watchmap.bin").GetResourceStream().CopyTo(fileStream);
                }
            }
        }

        public void RandomizeGiven(bool randomize, bool allowBoss)
        {
            if (randomize == false) return;

            // Create Scoutable Yokais Dictionary
            Dictionary<UInt32, Yokai> yokaisScoutable = new Dictionary<UInt32, Yokai>();
            if (allowBoss == true)
            {
                yokaisScoutable = Yokais.Where(x => x.Value.Status.Name == "Normal" || x.Value.Status.Name == "Boss").ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                yokaisScoutable = Yokais.Where(x => x.Value.Status.Name == "Normal").ToDictionary(x => x.Key, x => x.Value);
            }

            // Randomize Given Yokais
            foreach (KeyValuePair<string, List<uint>> givenYokai in GivenYokais)
            {
                for (int i = 0; i < givenYokai.Value.Count; i++)
                {
                    DataWriter givenWriter = new DataWriter(Directory + givenYokai.Key);
                    givenWriter.Seek(givenYokai.Value[i]);
                    givenWriter.WriteUInt32(yokaisScoutable.ElementAt(new RandomNumber(0, yokaisScoutable.Count).GetNumber()).Key);
                    givenWriter.Close();
                }
            }
        }

        public void RandomizeStatic(bool randomize, decimal percentageLevel)
        {
            if (randomize == false) return;

            // Create Scoutable Yokais Dictionarys
            Dictionary<UInt32, Yokai> yokaisScoutable = Yokais.Where(x => x.Value.Status.Name == "Normal" || x.Value.Status.Name == "MiniBoss").ToDictionary(x => x.Key, x => x.Value);

            // Randomize Static
            foreach (KeyValuePair<string, List<uint>> staticYokai in StaticYokais)
            {
                for (int i = 0; i < staticYokai.Value.Count; i++)
                {
                    // Initialize File Reader and File Writer
                    DataReader staticReader = new DataReader(File.ReadAllBytes(Directory + staticYokai.Key));
                    DataWriter staticWriter = new DataWriter(Directory + staticYokai.Key);

                    staticReader.Seek(staticYokai.Value[i]);
                    staticReader.Skip(0x10);

                    int staticCount = staticReader.ReadInt32();
                    for (int s = 0; s < staticCount; s++)
                    {
                        staticReader.Skip(8);
                        staticWriter.Seek((uint)staticReader.BaseStream.Position);

                        UInt32 yokaiID = staticReader.Reverse(staticReader.ReadUInt32());
                        int level = staticReader.ReadByte();

                        if (yokaisScoutable.ContainsKey(yokaiID))
                        {
                            staticWriter.WriteUInt32(yokaisScoutable.ElementAt(new RandomNumber(0, yokaisScoutable.Count).GetNumber()).Key);

                            // Add Percentage Level Multiplicator
                            if (percentageLevel > 0)
                            {
                                if (level > 0)
                                {
                                    level = Convert.ToInt32(level + (percentageLevel * level / 100));
                                    if (level > 99) level = 99;
                                    staticWriter.WriteByte(level);
                                }
                            }
                        }

                        staticReader.Skip(0x17);
                    }

                    // Close File
                    staticReader.Close();
                    staticWriter.Close();
                }
            }
        }

        public void BossSwap(bool randomize, bool scaleBaseStat)
        {
            if (randomize == false) return;

            // Create Temp Folder
            System.IO.Directory.CreateDirectory("./temp/");
            List<BossYokai> tempBossYokais = new List<BossYokai>(BossYokais);

            // Initialize File Writer
            DataWriter staticWriter = new DataWriter(Directory + StaticYokais.ElementAt(0).Key);

            // Swap Boss
            for (int i = 0; i < BossYokais.Count; i++)
            {
                // Take Random Yokai
                int randomNumber = new RandomNumber(0, tempBossYokais.Count).GetNumber();
                BossYokai randomBoss = tempBossYokais[randomNumber];;

                // Swap Boss And Script File
                staticWriter.Seek(BossYokais[i].PositionInEncounter);
                staticWriter.WriteUInt32(randomBoss.ID);
                File.Move(Directory + "/" + Name + "_a_fa/seq/battle/encount/" + randomBoss.ScriptName, "./temp/" + BossYokais[i].ScriptName);

                // Scale Base Stat Depending Of The Level
                if (scaleBaseStat == true)
                {
                    int oldLevel = randomBoss.Level;
                    int newLevel = BossYokais[i].Level;
                    

                    int startIndex = Yokais.Keys.ToList().IndexOf(randomBoss.ID);
                    for (int b = 0; b < randomBoss.Count; b++)
                    {
                        for (int s = 0; s < 5; s++)
                        {
                            Yokais.ElementAt(startIndex).Value.BaseStat[s] = Convert.ToInt32((double)Yokais.ElementAt(startIndex).Value.BaseStat[s] / oldLevel * newLevel);
                        }
                        startIndex += b;
                    }
                }

                tempBossYokais.RemoveAt(randomNumber);
            }

            // Restore Files
            for (int i = 0; i < BossYokais.Count; i++)
            {
                File.Move("./temp/" + BossYokais[i].ScriptName, Directory + "/" + Name + "_a_fa/seq/battle/encount/" + BossYokais[i].ScriptName);
            }

            // Close File
            staticWriter.Close();
        }

        public void BossBaseStat(decimal percentageBaseStat)
        {
            if (percentageBaseStat == 0) return;

            for (int i = 0; i < Yokais.Count; i++)
            {
                Yokai yokai = Yokais.ElementAt(i).Value;

                if (yokai.Status.Name == "Boss" || yokai.Status.Name == "MiniBoss")
                {
                    for (int s = 0; s < yokai.BaseStat.Count; i++)
                    {
                        yokai.BaseStat[s] = Convert.ToInt32(yokai.BaseStat[s] + (percentageBaseStat * 100 / 100));
                    }
                }
            }
        }

        public void RandomizeShop(bool randomize)
        {
            if (randomize == false) return;

            // Get All Shop Files
            string[] shopFolder = System.IO.Directory.GetFiles(Directory + "/" + Name + "_a_fa/data/res/shop/");

            foreach (string file in shopFolder)
            {
                // Exclude Invalid Shop Files
                if (Path.GetFileName(file) != "combine_config.cfg.bin" && Path.GetFileName(file) != "def_shoplist.cfg.bin")
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

                        if (Items.ContainsKey(shopReader.Reverse(shopReader.ReadUInt32())))
                        {
                            shopWriter.WriteUInt32(Items.ElementAt(new RandomNumber(0, Items.Count).GetNumber()).Key);
                        }

                        shopReader.Skip(0x24);
                    }

                    // Close Files
                    shopReader.Close();
                    shopWriter.Close();
                }
            }
        }

        public void RandomizeTreasureBox(bool randomize)
        {
            if (randomize == false) return;

            // Get All Treasure Box Files
            string[] treasureBoxFolder = System.IO.Directory.GetFiles(Directory + "/" + Name + "_a_fa/data/res/map/");

            // Find All Files Who Contains Treasure Box Entry
            string[] folders = System.IO.Directory.GetDirectories(Directory + "/" + Name + "_a_fa/data/res/map/").Select(Path.GetFileName).ToArray();
            foreach (string folder in folders)
            {
                if (File.Exists(Directory + "/" + Name + "_a_fa/data/res/map/" + folder + "/" + folder + "_tbox.cfg.bin"))
                {
                    // Initialize File Reader and File Writer
                    DataReader treasureBoxReader = new DataReader(File.ReadAllBytes(Directory + "/" + Name + "_a_fa/data/res/map/" + folder + "/" + folder + "_tbox.cfg.bin"));
                    DataWriter treasureBoxWriter = new DataWriter(Directory + "/" + Name + "_a_fa/data/res/map/" + folder + "/" + folder + "_tbox.cfg.bin");

                    // Find Start Byte
                    treasureBoxReader.Seek(0x18);
                    int unknowCount = treasureBoxReader.ReadInt16();
                    treasureBoxReader.Seek((uint)(28 + unknowCount * 24));
                    treasureBoxReader.Skip(0x10);
                    int boxCount = treasureBoxReader.ReadInt32();
                    treasureBoxReader.Skip(0x14);

                    // Randomize Treasure Box
                    for (int i = 0; i < boxCount; i++)
                    {
                        treasureBoxReader.Skip(8);
                        treasureBoxWriter.Seek((uint)treasureBoxReader.BaseStream.Position);

                        if (Items.ContainsKey(treasureBoxReader.Reverse(treasureBoxReader.ReadUInt32())))
                        {
                            treasureBoxWriter.WriteUInt32(Items.ElementAt(new RandomNumber(0, Items.Count).GetNumber()).Key);
                        }

                        treasureBoxReader.Skip(0x10);
                    }

                    // Close File
                    treasureBoxReader.Close();
                    treasureBoxWriter.Close();
                }
            }
        }

        public void RandomizeCrankKai(bool randomize, bool allowBoss)
        {
            if (randomize == false) return;

            // Create Scoutable Yokais Dictionary
            Dictionary<UInt32, Yokai> yokaisScoutable = new Dictionary<UInt32, Yokai>();
            if (allowBoss == true)
            {
                yokaisScoutable = Yokais.Where(x => x.Value.Status.Name == "Normal" || x.Value.Status.Name == "Boss").ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                yokaisScoutable = Yokais.Where(x => x.Value.Status.Name == "Normal").ToDictionary(x => x.Key, x => x.Value);
            }

            // Initialize File Reader And File Writer
            DataReader crankKaiReader = new DataReader(File.ReadAllBytes(Directory + "/" + Name + "_a_fa/data/res/capsule/capsule_config.cfg.bin"));
            DataWriter crankKaiWriter = new DataWriter(Directory + "/" + Name + "_a_fa/data/res/capsule/capsule_config.cfg.bin");

            crankKaiReader.Seek(0x18);
            int crankKaiCount = crankKaiReader.ReadInt32();
            for (int i = 0; i < crankKaiCount; i++)
            {
                crankKaiReader.Skip(0x10);
                crankKaiWriter.Seek((uint)crankKaiReader.BaseStream.Position);

                // Check If The ID is Item or Yokai
                UInt32 dropID = crankKaiReader.Reverse(crankKaiReader.ReadUInt32());

                if (Items.ContainsKey(dropID))
                {
                    crankKaiWriter.WriteUInt32(Items.ElementAt(new RandomNumber(0, Items.Count).GetNumber()).Key);
                } else if (yokaisScoutable.ContainsKey(dropID))
                {
                    crankKaiWriter.WriteUInt32(yokaisScoutable.ElementAt(new RandomNumber(0, yokaisScoutable.Count).GetNumber()).Key);
                }

                crankKaiReader.Skip(0x20);
            }

            // Close Files
            crankKaiReader.Close();
            crankKaiWriter.Close();
        }

        public void RandomizeLegendary(bool randomize, bool allowBoss, bool randomizeRequirment)
        {
            if (randomize == false) return;

            // Initialize File Writer
            DataWriter legendWriter = new DataWriter(Directory + "/" + Name + "_a_fa/data/res/legend/legend_config.cfg.bin");

            // Remove Legendary
            for (int i = 0; i < Yokais.Count; i++)
            {
                KeyValuePair<uint, Yokai> yokaiKeyValuePair = Yokais.ElementAt(i);

                if (yokaiKeyValuePair.Value.IsLegendary == true)
                {
                    Yokais[yokaiKeyValuePair.Key].IsLegendary = false;
                }
            }

            // Create Scoutable Yokais Dictionary
            Dictionary<UInt32, Yokai> yokaisScoutable = new Dictionary<UInt32, Yokai>();
            if (allowBoss == true)
            {
                yokaisScoutable = Yokais.Where(x => x.Value.Status.Name == "Normal" || x.Value.Status.Name == "Boss").ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                yokaisScoutable = Yokais.Where(x => x.Value.Status.Name == "Normal").ToDictionary(x => x.Key, x => x.Value);
            }

            // Create New Legendary Yokais
            legendWriter.Seek(0x1C);
            List<int> legendaryYokaisIndex = new RandomNumber(0, yokaisScoutable.Count).GetNumbers(LegendaryYokaisNumber);
            for (int i = 0; i < LegendaryYokaisNumber; i++)
            {
                KeyValuePair<uint, Yokai> yokaiKeyValuePair = yokaisScoutable.ElementAt(legendaryYokaisIndex[i]);
                yokaisScoutable[yokaiKeyValuePair.Key].IsLegendary = true;

                legendWriter.Skip(0x10);
                legendWriter.WriteUInt32(yokaiKeyValuePair.Key);
                legendWriter.Skip(0x44);
            }

            // Create New Medallium Requirment
            if (randomizeRequirment == true)
            {
                // Randomize Requirement
                legendWriter.Seek(0x1C);
                for (int i = 0; i < LegendaryYokaisNumber; i++)
                {
                    legendWriter.Skip(0x14);

                    // Create Requirment Yokai and Exclude Legend Yokai From The List
                    List<int> requirmentYokais = new RandomNumber(0, yokaisScoutable.Count).GetNumbers(8, legendaryYokaisIndex);

                    // Save Requirment Yokais And Show Yokais In The Medallium
                    for (int r = 0; r < requirmentYokais.Count; r++)
                    {
                        legendWriter.WriteUInt32(yokaisScoutable.ElementAt(requirmentYokais[r]).Key);
                        legendWriter.WriteByte(0x01);
                        legendWriter.Skip(0x03);
                    }

                    legendWriter.Skip(0x04);
                }
            }

            // Close File
            legendWriter.Close();
        }
    }
}