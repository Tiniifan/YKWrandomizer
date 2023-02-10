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

namespace YKWrandomizer.Yokai_Watch.Games.YW2
{
    public class YW2 : Randomizer
    {
        public YW2(string romfsPath, string language)
        {
            Name = "Yo-Kai Watch 2";

            Attacks = Common.Attacks.YW2;
            Techniques = Common.Techniques.YW2;
            Inspirits = Common.Inspirits.YW2;
            Soultimates = Common.Soultimates.YW2;
            Skills = Common.Skills.YW2;
            Items = Common.Items.YW2;
            Tribes = Common.Tribes.YW2;

            RomfsPath = romfsPath;

            FilesPath = new Dictionary<string, string>();
            FilesPath.Add("charatext", RomfsPath + @"\yw2_lg_" + language + @"_fa\data\res\text\chara_text_" + language + ".cfg.bin");
            FilesPath.Add("charabase", RomfsPath + @"\yw2_a_fa\data\res\character\chara_base_0.04c.cfg.bin");
            FilesPath.Add("charaparam", RomfsPath + @"\yw2_a_fa\data\res\character\chara_param_0.03a.cfg.bin");
            FilesPath.Add("combineconfig", RomfsPath + @"\yw2_a_fa\data\res\shop\combine_config.cfg.bin");
            FilesPath.Add("shop", RomfsPath + @"\yw2_a_fa\data\res\shop\");
            FilesPath.Add("face_icon", RomfsPath + @"\yw2_a_fa\data\menu\face_icon\face_icon.png");
            FilesPath.Add("legendconfig", RomfsPath + @"\yw2_a_fa\data\res\character\legend_config_0.01b.cfg.bin");
            FilesPath.Add("encounter", RomfsPath + @"\yw2_a_fa\data\res\battle\common_enc_0.03a.cfg.bin");
            FilesPath.Add("crankai", RomfsPath + @"\yw2_a_fa\data\res\capsule\capsule_config_0.03.cfg.bin");
            FilesPath.Add("wild", RomfsPath + @"\yw2_a_fa\data\res\map\");
            FilesPath.Add("questconfig", RomfsPath + @"\yw2_a_fa\data\res\quest\quest_config_0.06b.cfg.bin");
            FilesPath.Add("baffleboard", RomfsPath + @"\yw2_a_fa\data\res\map\territory_0.01j.cfg.bin");

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
                YW2Charabase charabase = null;
                YW2Charaparam charaparam = null;

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

                if (charabase != null & charaparam != null)
                {
                    Yokai yokai = new Yokai
                    {
                        Name = charatext.GetNounText(charabase.NameID, 0),
                        Rank = charabase.Rank,
                        Tribe = charabase.Tribe,
                        MinStat = charaparam.MinStat,
                        MaxStat = charaparam.MaxStat,
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
                        Statut = new Statut
                        {
                            IsLegendary = charabase.IsLegendary,
                            IsRare = charabase.IsRare,
                            IsBoss = charabase.Tribe == 0x0A || charabase.Tribe == 0x00,
                            IsScoutable = charaparam.Scoutable,
                            IsStatic = !charaparam.Scoutable && (charabase.Tribe == 0x0A || charabase.Tribe == 0x00)
                        },
                    };

                    yokais.Add(charaparam.ParamID, yokai);
                }
            }

            charabaseReader.Close();
            charaparamReader.Close();

            yokais.Remove(0x7101BA13);

            return yokais;
        }

        public override void SaveYokai()
        {
            // Initialise Face Icon Map
            Bitmap faceIcon = new Bitmap(Image.FromStream(new ResourceReader("yw2_face_icon.png").GetResourceStream()));

            DataReader charabaseReader = new DataReader(File.ReadAllBytes(FilesPath["charabase"]));
            DataReader charaparamReader = new DataReader(File.ReadAllBytes(FilesPath["charaparam"]));
            DataWriter charabaseWriter = new DataWriter(FilesPath["charabase"]);
            DataWriter charaparamWriter = new DataWriter(FilesPath["charaparam"]);

            charaparamReader.Seek(0x18);
            int yokaiCount = charaparamReader.ReadInt32();
            for (int i = 0; i < yokaiCount; i++)
            {
                YW2Charabase charabase = null;
                YW2Charaparam charaparam = null;

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

                if (charabase != null)
                {
                    Yokai yokai = Yokais.FirstOrDefault(x => x.Key == charaparam.ParamID).Value;

                    if (yokai != null)
                    {
                        charabase.Rank = yokai.Rank;
                        charabase.Tribe = yokai.Tribe;
                        charaparam.MinStat = yokai.MinStat;
                        charaparam.MaxStat = yokai.MaxStat;
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

                        charabase.Write(charabaseWriter);
                        charaparam.Write(charaparamWriter);

                        if (yokai.Statut.IsScoutable == true && yokai.Statut.IsBoss == false)
                        {
                            if (yokai.Tribe != 0x0A && yokai.Tribe != 0x00)
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
                    "- Min Stat: [ HP: " + yokai.MinStat[0] + ", Strength: " + yokai.MinStat[1] + ", Spirit: " + yokai.MinStat[2] + ", Defense: " + yokai.MinStat[3]+ ", Speed: " + yokai.MinStat[4] + " ]",
                    "- Max Stat: [ HP: " + yokai.MaxStat[0] + ", Strength: " + yokai.MaxStat[1] + ", Spirit: " + yokai.MaxStat[2] + ", Defense: " + yokai.MaxStat[3]+ ", Speed: " + yokai.MaxStat[4] + " ]",
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
                if (File.Exists(FilesPath["wild"] + folder + "/" + folder + ".pck"))
                {
                    // Initialize File Reader and File Writer
                    DataReader wildReader = new DataReader(File.ReadAllBytes(FilesPath["wild"] + folder + "/" + folder + ".pck"));
                    DataWriter wildWriter = new DataWriter(FilesPath["wild"] + folder + "/" + folder + ".pck");

                    while (wildReader.BaseStream.Position < wildReader.Length - 8)
                    {
                        uint pos = (uint)wildReader.BaseStream.Position;

                        // Search Wild Entry In The .pck
                        if (wildReader.ReadUInt32() == 0xE43FA5D2)
                        {
                            wildReader.Skip(0x0C);
                            int wildCount = wildReader.ReadInt32();

                            // For Each Yokai In The Area
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
                                int yokaiLevel = wildReader.ReadByte();

                                // Add Percentage Level Multiplicator
                                if (percentageLevel > 0)
                                {
                                    if (yokaiLevel > 0)
                                    {
                                        int newLevel = yokaiLevel + yokaiLevel * 20 / 100;
                                        if (newLevel > 99)
                                        {
                                            newLevel = 99;
                                        }
                                        wildWriter.WriteByte(newLevel);
                                    }
                                }

                                // Next Yokai
                                wildReader.Skip(0x17);
                            }

                            break;
                        }
                    }

                    wildReader.Close();
                    wildWriter.Close();
                }
            }
        }

        public override void RandomizeTreasureBox(bool randomize)
        {
            if (randomize == false) return;

            List<UInt32> itemsID = Items.Select(i => i.Key).ToList();

            // Find All Files Who Contains Wild Entry
            string[] folders = Directory.GetDirectories(FilesPath["wild"]).Select(Path.GetFileName).ToArray();
            foreach (string folder in folders)
            {
                if (File.Exists(FilesPath["wild"] + folder + "/" + folder + ".pck"))
                {
                    // Initialize File Reader and File Writer
                    DataReader treasureboxReader = new DataReader(File.ReadAllBytes(FilesPath["wild"] + folder + "/" + folder + ".pck"));
                    DataWriter treasureboxWriter = new DataWriter(FilesPath["wild"] + folder + "/" + folder + ".pck");

                    uint treasureboxOffset = treasureboxReader.FindUInt32BetweenRange(0x890AA6BE, 0x00, (uint)treasureboxReader.Length);
                    if (treasureboxOffset < (uint)treasureboxReader.Length)
                    {
                        treasureboxReader.Seek(treasureboxOffset);
                        treasureboxReader.Skip(0x08);

                        int boxCount = treasureboxReader.ReadInt32();
                        for (int i = 0; i < boxCount; i++)
                        {
                            treasureboxReader.Skip(8);
                            treasureboxWriter.Seek((uint)treasureboxReader.BaseStream.Position);

                            if (Items.ContainsKey(treasureboxReader.ReadUInt32()))
                            {
                                treasureboxWriter.WriteUInt32(itemsID[Seed.Next(0, itemsID.Count)]);
                            }

                            treasureboxReader.Skip(0x10);
                        }
                    }

                    treasureboxReader.Close();
                    treasureboxWriter.Close();
                }
            }
        }

        public override void FixStory(bool randomize)
        {
            if (randomize == false) return;

            // Jibanyse Quest
            DataWriter questconfigWriter = new DataWriter(FilesPath["questconfig"]);
            questconfigWriter.Seek(0x10084);
            questconfigWriter.WriteUInt32(0xC5AD7A9D);
            questconfigWriter.Seek(0x100AC);
            questconfigWriter.WriteUInt32(0xC5AD7A9D);

            // Jibanyse Baffleboard
            DataWriter baffleboardWriter = new DataWriter(FilesPath["baffleboard"]);
            baffleboardWriter.Seek(0x23C);
            baffleboardWriter.WriteUInt32(0xC5AD7A9D);
            baffleboardWriter.Seek(0x258);
            baffleboardWriter.WriteUInt32(0xC5AD7A9D);
            baffleboardWriter.Seek(0x278);
            baffleboardWriter.WriteUInt32(0xC5AD7A9D);

            // Fix Fidgephant bug
            DataWriter fidgephantMapWriter = new DataWriter(RomfsPath + "/yw2_a_fa/data/res/map/t131g00/t131g00.pck");
            fidgephantMapWriter.Seek(0x1F74);
            fidgephantMapWriter.WriteUInt32(0xA4AF6DC1);
            fidgephantMapWriter.Seek(0x1F98);
            fidgephantMapWriter.WriteUInt32(0xA4AF6DC1);
            fidgephantMapWriter.Seek(0x1FBC);
            fidgephantMapWriter.WriteUInt32(0xA4AF6DC1);
            fidgephantMapWriter.Seek(0x1FE0);
            fidgephantMapWriter.WriteUInt32(0xA4AF6DC1);
            fidgephantMapWriter.Seek(0x267C);
            fidgephantMapWriter.WriteUInt32(0xA4AF6DC1);
            fidgephantMapWriter.Seek(0x26A0);
            fidgephantMapWriter.WriteUInt32(0xA4AF6DC1);
            fidgephantMapWriter.Seek(0x2778);
            fidgephantMapWriter.WriteUInt32(0xA4AF6DC1);

            // Bamboo Quest
            DataWriter bamboogWriter1 = new DataWriter(RomfsPath + "/yw2_a_fa/data/res/shop/shop_shpN024.cfg.bin");
            DataWriter bamboogWriter2 = new DataWriter(RomfsPath + "/yw2_a_fa/data/res/shop/shop_shpN024_0.01n.cfg.bin");
            bamboogWriter1.Seek(0x94);
            bamboogWriter1.WriteUInt32(0x3CF5AC04);
            bamboogWriter2.Seek(0xFC);
            bamboogWriter2.WriteUInt32(0x3CF5AC04);

            questconfigWriter.Close();
            baffleboardWriter.Close();
            fidgephantMapWriter.Close();
            bamboogWriter1.Close();
            bamboogWriter2.Close();
        }
    }
}
