using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using YKWrandomizer.Tool;
using YKWrandomizer.Level5.Text;
using YKWrandomizer.Yokai_Watch.Logic;
using YKWrandomizer.Yokai_Watch.Common;

namespace YKWrandomizer.Yokai_Watch.Games.YW1
{
    public class YW1 : Randomizer
    {
        public YW1(string romfsPath, string language)
        {
            Name = "Yo-Kai Watch 1";

            Attacks = Common.Attacks.YW1;
            Techniques = Common.Techniques.YW1;
            Inspirits = Common.Inspirits.YW1;
            Soultimates = Common.Soultimates.YW1;
            Skills = Common.Skills.YW1;
            Items = Common.Items.YW1;
            Tribes = Common.Tribes.YW1;
            YokaiGiven = Common.GivenYokais.YW1;

            RomfsPath = romfsPath;

            FilesPath = new Dictionary<string, string>();
            FilesPath.Add("charatext", RomfsPath + @"\yw1_a_fa\data\res\text\chara_text_" + language + ".cfg.bin");
            FilesPath.Add("charabase", RomfsPath + @"\yw1_a_fa\data\res\character\chara_base_0.04j.cfg.bin");
            FilesPath.Add("charaparam", RomfsPath + @"\yw1_a_fa\data\res\character\chara_param_0.02.cfg.bin");
            FilesPath.Add("combineconfig", RomfsPath + @"\yw1_a_fa\data\res\shop\combine_config.cfg.bin");
            FilesPath.Add("shop", RomfsPath + @"\yw1_a_fa\data\res\shop\");
            FilesPath.Add("face_icon", RomfsPath + @"\yw1_a_fa\data\menu\face_icon\face_icon.png");
            FilesPath.Add("legendconfig", RomfsPath + @"\yw1_a_fa\data\res\legend\legend_config.cfg.bin");
            FilesPath.Add("encounter", RomfsPath + @"\yw1_a_fa\data\res\battle\common_enc_0.02m.cfg.bin");
            FilesPath.Add("crankai", RomfsPath + @"\yw1_a_fa\data\res\capsule\capsule_config.cfg.bin");
            FilesPath.Add("wild", RomfsPath + @"\yw1_a_fa\data\res\map\");

            Yokais = LoadYokai();
            Fusions = LoadFusion();
            Evolutions = LoadEvolution();
        }

        private Dictionary<UInt32, Yokai> LoadYokai()
        {
            Level5_Text charatext = new Level5_Text(File.ReadAllBytes(FilesPath["charatext"]));

            Dictionary<UInt32, Yokai> yokais = new Dictionary<uint, Yokai>();

            DataReader charabaseReader = new DataReader(File.ReadAllBytes(FilesPath["charabase"]));
            DataReader charaparamReader = new DataReader(File.ReadAllBytes(FilesPath["charaparam"]));

            charaparamReader.Seek(0x18);
            int yokaiCount = charaparamReader.ReadInt32();
            for (int i = 0; i < yokaiCount; i++)
            {
                YW1Charabase charabase = null;
                YW1Charaparam charaparam = null;

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

                if (charabase != null & charaparam != null)
                {
                    Yokai yokai = new Yokai
                    {
                        Name = charatext.GetNounText(charabase.NameID, 0),
                        Rank = charabase.Rank,
                        Tribe = charaparam.Tribe,
                        MinStat = charaparam.MinStat,
                        AttributeDamage = charaparam.AttributeDamage,
                        AttackID = charaparam.AttackID,
                        TechniqueID = charaparam.TechniqueID,
                        InspiritID = charaparam.InspiritID,
                        SoultimateID = charaparam.SoultimateID,
                        FoodID = charaparam.FoodID,
                        SkillID = charaparam.SkillID,
                        Money = charaparam.Money,
                        Experience = charaparam.Experience,
                        DropID = charaparam.DropID,
                        DropRate = charaparam.DropRate,
                        ExperienceCurve = charaparam.ExperienceCurve,
                        EvolveOffset = charaparam.EvolveOffset,
                        MedaliumOffset = charaparam.MedaliumOffset,
                        Medal = charabase.Medal,
                        ScoutableID = charaparam.ScoutableID,
                        Statut = new Statut
                        {
                            IsLegendary = charabase.IsLegendary,
                            IsRare = charabase.IsRare,
                            IsBoss = charaparam.Tribe == 0x09 || charaparam.Tribe == 0x00,
                            IsScoutable = charaparam.Scoutable,
                            IsStatic = !charaparam.Scoutable && (charaparam.Tribe == 0x09 || charaparam.Tribe == 0x00)
                        },
                    };

                    yokais.Add(charaparam.ParamID, yokai);
                }
            }

            charabaseReader.Close();
            charaparamReader.Close();

            yokais.Remove(0x8B97B90D);

            return yokais;
        }

        public override void SaveYokai()
        {
            // Initialise Face Icon Map
            Bitmap faceIcon = new Bitmap(Image.FromStream(new ResourceReader("yw1_face_icon.png").GetResourceStream()));

            DataReader charabaseReader = new DataReader(File.ReadAllBytes(FilesPath["charabase"]));
            DataReader charaparamReader = new DataReader(File.ReadAllBytes(FilesPath["charaparam"]));
            DataWriter charabaseWriter = new DataWriter(FilesPath["charabase"]);
            DataWriter charaparamWriter = new DataWriter(FilesPath["charaparam"]);

            charaparamReader.Seek(0x18);
            int yokaiCount = charaparamReader.ReadInt32();
            for (int i = 0; i < yokaiCount; i++)
            {
                YW1Charabase charabase = null;
                YW1Charaparam charaparam = new YW1Charaparam();

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

                if (charabase != null)
                {
                    Yokai yokai = Yokais.FirstOrDefault(x => x.Key == charaparam.ParamID).Value;

                    if (yokai != null)
                    {
                        charabase.Rank = yokai.Rank;
                        charaparam.Tribe = yokai.Tribe;
                        charaparam.MinStat = yokai.MinStat;
                        charaparam.AttributeDamage = yokai.AttributeDamage;
                        charaparam.AttackID = yokai.AttackID;
                        charaparam.TechniqueID = yokai.TechniqueID;
                        charaparam.InspiritID = yokai.InspiritID;
                        charaparam.SoultimateID = yokai.SoultimateID;
                        charaparam.SkillID = yokai.SkillID;
                        charaparam.FoodID = yokai.FoodID;
                        charaparam.Money = yokai.Money;
                        charaparam.Experience = yokai.Experience;
                        charaparam.DropID = yokai.DropID;
                        charaparam.DropRate = yokai.DropRate;
                        charaparam.ExperienceCurve = yokai.ExperienceCurve;
                        charaparam.EvolveOffset = yokai.EvolveOffset;
                        charaparam.MedaliumOffset = yokai.MedaliumOffset;
                        charabase.Medal = yokai.Medal;
                        charaparam.MedaliumOffset = yokai.MedaliumOffset;
                        charabase.IsLegendary = yokai.Statut.IsLegendary;
                        charabase.IsRare = yokai.Statut.IsRare;
                        charaparam.ScoutableID = yokai.ScoutableID;

                        charabase.Write(charabaseWriter);
                        charaparam.Write(charaparamWriter);

                        if (yokai.Statut.IsScoutable == true && yokai.Statut.IsBoss == false)
                        {
                            if (yokai.Tribe != 0x09 && yokai.Tribe != 0x00)
                            {
                                Image newMedal = null;

                                if (yokai.Statut.IsRare)
                                {
                                    newMedal = Image.FromStream(new ResourceReader("y_medal_rare0" + yokai.Tribe + ".png").GetResourceStream());
                                }
                                else if (yokai.Statut.IsLegendary)
                                {
                                    newMedal = Image.FromStream(new ResourceReader("y_medal_lege0" + yokai.Tribe + ".png").GetResourceStream());
                                }
                                else
                                {
                                    newMedal = Image.FromStream(new ResourceReader("y_medal_nml0" + yokai.Tribe + ".png").GetResourceStream());
                                }

                                faceIcon = Draw.DrawImage(faceIcon, yokai.Medal.X * 44, yokai.Medal.Y * 44, newMedal);
                            }
                        }
                    }
                }
            }

            charabaseReader.Close();
            charaparamReader.Close();

            charabaseWriter.Close();
            charaparamWriter.Close();

            faceIcon.Save(FilesPath["face_icon"], ImageFormat.Png);
        }

        public override string PrintYokai()
        {
            string text = "--Yokai Traits--\n";

            foreach (Yokai yokai in Yokais.Values)
            {
                string yokaiText = string.Join("\n", new String[]
                {
                    yokai.Name,
                    "- Rank: " + Ranks.Value[(byte)yokai.Rank],
                    "- Tribe: " + Tribes[(byte)yokai.Tribe],
                    "- Attack: " + Attacks.FirstOrDefault(x => x.Key == yokai.AttackID).Value,
                    "- Technique: "+ Techniques.FirstOrDefault(x => x.Key == yokai.TechniqueID).Value,
                    "- Inspirit: "+ Inspirits.FirstOrDefault(x => x.Key == yokai.InspiritID).Value,
                    "- Soultimate: "+ Soultimates.FirstOrDefault(x => x.Key == yokai.SoultimateID).Value,
                    "- SkillID: "+ Skills.FirstOrDefault(x => x.Key == yokai.SkillID).Value,
                    "- Base Stat: [ HP: " + yokai.MinStat[0] + ", Strength: " + yokai.MinStat[1] + ", Spirit: " + yokai.MinStat[2] + ", Defense: " + yokai.MinStat[3]+ ", Speed: " + yokai.MinStat[4] + " ]",
                    "- Attribute Damage: [ Fire: " + yokai.AttributeDamage[0].ToString("0.00") + ", Ice: " + yokai.AttributeDamage[1].ToString("0.00") + ", Earth: " + yokai.AttributeDamage[2].ToString("0.00") + ", Ligthning: " + yokai.AttributeDamage[3].ToString("0.00") + ", Water: " + yokai.AttributeDamage[4].ToString("0.00") + ", Wind: " + yokai.AttributeDamage[5].ToString("0.00") + " ]",
                    "- Money: "+ yokai.Money,
                    "- Experience: "+ yokai.Experience,
                    "- Drop: [ (" + Items.FirstOrDefault(x => x.Key == yokai.DropID[0]).Value + ": " + yokai.DropRate[0] + "%)" + ", (" +  Items.FirstOrDefault(x => x.Key == yokai.DropID[1]).Value + ": " + yokai.DropRate[1] + "%) ]",
                    "- ExperienceCurve: "+ ExperienceCurves.Value[(byte)yokai.ExperienceCurve],
                    "- Statut: [ IsRare: " + yokai.Statut.IsRare + ", IsLegendary: " + yokai.Statut.IsLegendary + ", IsScoutable: " + yokai.Statut.IsScoutable + " ]",
                });

                text += yokaiText + "\n";
                
            }

            return text;
        }

        public override void RandomizeWild(bool randomize, decimal percentageLevel)
        {
            if (randomize == false) return;

            Dictionary<UInt32, Yokai> yokaisScoutable = Yokais.Where(x => x.Value.Statut.IsScoutable == true).ToDictionary(i => i.Key, i => i.Value);
            Dictionary<UInt32, Yokai> poorYokais = yokaisScoutable.Where(x => x.Value.Rank == 0).ToDictionary(x => x.Key, x => x.Value);

            // Find All Files Who Contains Wild Entry
            string[] folders = Directory.GetDirectories(FilesPath["wild"]).Select(Path.GetFileName).ToArray();
            foreach (string folder in folders)
            {
                if (File.Exists(FilesPath["wild"] + folder + "/" + folder + "_enc_0.02m.cfg.bin"))
                {
                    // Initialize File Reader and File Writer
                    DataReader wildReader = new DataReader(File.ReadAllBytes(FilesPath["wild"] + folder + "/" + folder + "_enc_0.02m.cfg.bin"));
                    DataWriter wildWriter = new DataWriter(FilesPath["wild"] + folder + "/" + folder + "_enc_0.02m.cfg.bin");

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
                                    wildWriter.WriteUInt32(poorYokais.ElementAt(Seed.Next(0, poorYokais.Count)).Key);
                                }
                                else
                                {
                                    wildWriter.WriteUInt32(yokaisScoutable.ElementAt(Seed.Next(0, yokaisScoutable.Count)).Key);
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
        }

        public override void RandomizeTreasureBox(bool randomize)
        {
            if (randomize == false) return;

            List<UInt32> itemsID = Items.Select(i => i.Key).ToList();

            // Find All Files Who Contains Treasure Box Entry
            string[] folders = Directory.GetDirectories(FilesPath["wild"]).Select(Path.GetFileName).ToArray();
            foreach (string folder in folders)
            {
                if (File.Exists(FilesPath["wild"] + folder + "/" + folder + "_tbox.cfg.bin"))
                {
                    // Initialize File Reader and File Writer
                    DataReader treasureBoxReader = new DataReader(File.ReadAllBytes(FilesPath["wild"] + folder + "/" + folder + "_tbox.cfg.bin"));
                    DataWriter treasureBoxWriter = new DataWriter(FilesPath["wild"] + folder + "/" + folder + "_tbox.cfg.bin");

                    uint treasureboxOffset = treasureBoxReader.FindUInt32BetweenRange(0x890AA6BE, 0x00, (uint)treasureBoxReader.Length);
                    if (treasureboxOffset < (uint)treasureBoxReader.Length)
                    {
                        treasureBoxReader.Seek(treasureboxOffset);
                        treasureBoxReader.Skip(0x08);

                        int boxCount = treasureBoxReader.ReadInt32();
                        for (int i = 0; i < boxCount; i++)
                        {
                            treasureBoxReader.Skip(8);
                            treasureBoxWriter.Seek((uint)treasureBoxReader.BaseStream.Position);

                            if (Items.ContainsKey(treasureBoxReader.ReadUInt32()))
                            {
                                treasureBoxWriter.WriteUInt32(itemsID[Seed.Next(0, itemsID.Count)]);
                            }

                            treasureBoxReader.Skip(0x10);
                        }
                    }

                    // Close File
                    treasureBoxReader.Close();
                    treasureBoxWriter.Close();
                }
            }
        }

        public override void FixStory(bool randomize)
        {
            if (randomize == false) return;

            // Buhu - Mochismo - Dulluma
            Yokais[0x8443D22D].Rank = 0;
            Yokais[0x86056C74].Rank = 0;
            Yokais[0xAEACEBD9].Rank = 0;

            // Fusion Quest
            DataWriter mochismoMapWriter = new DataWriter(File.ReadAllBytes(RomfsPath + "/yw1_a_fa/data/res/map/t103d11/t103d11_enc_0.02m.cfg.bin"));
            mochismoMapWriter.Seek(0x528);
            mochismoMapWriter.WriteUInt32(0x86056C74);
            mochismoMapWriter.Seek(0x578);
            mochismoMapWriter.WriteUInt32(0x86056C74);
            mochismoMapWriter.Seek(0x5B8);
            mochismoMapWriter.WriteUInt32(0x86056C74);
            mochismoMapWriter.Seek(0x648);
            mochismoMapWriter.WriteUInt32(0x86056C74);

            DataWriter dullumaMapWriter = new DataWriter(File.ReadAllBytes(RomfsPath + "/yw1_a_fa/data/res/map/t101d03/t101d03_enc_0.02m.cfg.bin"));
            dullumaMapWriter.Seek(0x2BC);
            dullumaMapWriter.WriteUInt32(0xAEACEBD9);
            dullumaMapWriter.Seek(0x328);
            dullumaMapWriter.WriteUInt32(0xAEACEBD9);
            dullumaMapWriter.Seek(0x394);
            dullumaMapWriter.WriteUInt32(0xAEACEBD9);
            dullumaMapWriter.Seek(0x3B8);
            dullumaMapWriter.WriteUInt32(0xAEACEBD9);

            mochismoMapWriter.Close();
            dullumaMapWriter.Close();
        }
    }
}
