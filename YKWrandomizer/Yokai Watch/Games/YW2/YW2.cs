using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using YKWrandomizer.Tool;
using YKWrandomizer.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW2
{
    public class YW2 : Randomizer
    {
        public YW2(string romfsPath)
        {
            Name = "Yo-Kai Watch 2";
            NameCode = "YW2";
            TribeCount = 9;

            YokaiScoutable = Common.YokaiScoutable.YW2;
            YokaiStatic = Common.YokaiStatic.YW2;
            YokaiBoss = Common.YokaiBoss.YW2;

            Attacks = Common.Attacks.YW2;
            Techniques = Common.Techniques.YW2;
            Inspirits = Common.Inspirits.YW2;
            Soultimates = Common.Soultimates.YW2;
            Skills = Common.Skills.YW2;
            Items = Common.Items.YW2;
            Souls = Common.Souls.YW2;

            RomfsPath = romfsPath;
            FaceIconFolder = "/yw2_a_fa/data/menu/face_icon/face_icon.png";

            LocalFiles = new Dictionary<string, LocalFile>();
            LocalFiles.Add("charabase", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\character\chara_base_0.04c.cfg.bin"));
            LocalFiles.Add("charaparam", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\character\chara_param_0.03a.cfg.bin"));
            LocalFiles.Add("combineconfig", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\shop\combine_config.cfg.bin"));
            LocalFiles.Add("soulconfig", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\skill\soul_config_0.04b.cfg.bin"));
            LocalFiles.Add("encounter", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\battle\common_enc_0.03a.cfg.bin"));
            LocalFiles.Add("crankai", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\capsule\capsule_config_0.03.cfg.bin"));
            LocalFiles.Add("legendconfig", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\character\legend_config_0.01b.cfg.bin"));
            LocalFiles.Add("questconfig", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\quest\quest_config_0.06b.cfg.bin"));
            LocalFiles.Add("baffleboard", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\map\territory_0.01j.cfg.bin"));
        }

        public override void RandomizeWild(bool randomize, decimal percentageLevel)
        {
            if (randomize == false) return;

            // Find All Files Who Contains Wild Entry
            string[] folders = Directory.GetDirectories(RomfsPath + "/yw2_a_fa/data/res/map/").Select(Path.GetFileName).ToArray();
            foreach (string folder in folders)
            {
                if (File.Exists(RomfsPath + "/yw2_a_fa/data/res/map/" + folder + "/" + folder + ".pck"))
                {
                    // Initialize File Reader and File Writer
                    DataReader wildReader = new DataReader(File.ReadAllBytes(RomfsPath + "/yw2_a_fa/data/res/map/" + folder + "/" + folder + ".pck"));
                    DataWriter wildWriter = new DataWriter(RomfsPath + "/yw2_a_fa/data/res/map/" + folder + "/" + folder + ".pck");

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
                                wildWriter.WriteUInt32(YokaiScoutable.ElementAt(Seed.Next(0, YokaiScoutable.Count)).Key);

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

            // Find All Files Who Contains Wild Entry
            string[] folders = Directory.GetDirectories(RomfsPath + "/yw2_a_fa/data/res/map/").Select(Path.GetFileName).ToArray();
            foreach (string folder in folders)
            {
                if (File.Exists(RomfsPath + "/yw2_a_fa/data/res/map/" + folder + "/" + folder + ".pck"))
                {
                    // Initialize File Reader and File Writer
                    DataReader treasureboxReader = new DataReader(File.ReadAllBytes(RomfsPath + "/yw2_a_fa/data/res/map/" + folder + "/" + folder + ".pck"));
                    DataWriter treasureboxWriter = new DataWriter(RomfsPath + "/yw2_a_fa/data/res/map/" + folder + "/" + folder + ".pck");

                    while (treasureboxReader.BaseStream.Position < treasureboxReader.Length - 8)
                    {
                        uint pos = (uint)treasureboxReader.BaseStream.Position;

                        // Search Wild Entry In The .pck
                        if (treasureboxReader.ReadUInt32() == 0x8592EF19)
                        {
                            treasureboxReader.Skip(0x04);
                            int treasureCount = treasureboxReader.ReadInt32();

                            // For Each Treasure Box In The Area
                            for (int i = 0; i < treasureCount; i++)
                            {
                                treasureboxReader.Skip(0x08);
                                treasureboxWriter.Seek((uint)treasureboxReader.BaseStream.Position);

                                UInt32 itemID = treasureboxReader.ReadUInt32();
                                if (Items.ContainsKey(itemID)) {
                                    treasureboxWriter.WriteUInt32(Items.ElementAt(Seed.Next(0, Items.Count)).Key);
                                }

                                treasureboxReader.Skip(0x0C);
                            }

                            break;
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
            DataWriter questconfigWriter = new DataWriter(LocalFiles["questconfig"].Data);
            questconfigWriter.Seek(0x10084);
            questconfigWriter.WriteUInt32(0xC5AD7A9D);
            questconfigWriter.Seek(0x100AC);
            questconfigWriter.WriteUInt32(0xC5AD7A9D);

            // Jibanyse Baffleboard
            DataWriter baffleboardWriter = new DataWriter(LocalFiles["baffleboard"].Data);
            baffleboardWriter.Seek(0x23C);
            baffleboardWriter.WriteUInt32(0xC5AD7A9D);
            baffleboardWriter.Seek(0x258);
            baffleboardWriter.WriteUInt32(0xC5AD7A9D);
            baffleboardWriter.Seek(0x278);
            baffleboardWriter.WriteUInt32(0xC5AD7A9D);

            // Fix Fidgephant bug
            DataWriter fidgephantMapWriter = new DataWriter(File.ReadAllBytes(RomfsPath + "/yw2_a_fa/data/res/map/t131g00/t131g00.pck"));
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

            questconfigWriter.Close();
            baffleboardWriter.Close();
            fidgephantMapWriter.Close();
        }
    }
}
