using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YKWrandomizer.Tool;
using YKWrandomizer.Level5.Text;
using YKWrandomizer.Yokai_Watch.Logic;
using YKWrandomizer.Level5.Archive.ARC0;

namespace YKWrandomizer.Yokai_Watch.Games.YW3
{
    public class YW3 : IGame
    {
        public string Name => "Yokai Watch 3";

        public Dictionary<uint, string> Attacks => Common.Attacks.YW3;

        public Dictionary<uint, string> Techniques => Common.Techniques.YW3;

        public Dictionary<uint, string> Inspirits => Common.Inspirits.YW3;

        public Dictionary<uint, string> Soultimates => Common.Soultimates.YW3;

        public Dictionary<uint, string> Skills => Common.Skills.YW3;

        public Dictionary<uint, string> Items => Common.Items.YW3;

        public Dictionary<int, string> Tribes => Common.Tribes.YW3;

        public Dictionary<string, List<uint>> YokaiGiven => Common.GivenYokais.YW3;

        public List<uint> YokaiUnscoutableAutorized => Common.YokaiUnscoutableAutorized.YW3;

        public ARC0 Game { get; set; }

        private ARC0 Language { get; set; }

        public string LanguageCode { get; set; }

        public YW3(string gamePath, string languagePath)
        {
            Game = new ARC0(new FileStream(gamePath, FileMode.Open));
            Language = new ARC0(new FileStream(languagePath, FileMode.Open));
            LanguageCode = Regex.Match(languagePath, @"lg_(.*?)\.fa").Groups[1].Value;
        }

        public List<Yokai> GetYokais()
        {
            List<Yokai> yokais;

            T2bþ yokaiNames = new T2bþ(Language.Directory.GetFileFromFullPath("/data/res/text/chara_text_" + LanguageCode + ".cfg.bin"));

            using (BinaryDataReader charaparam = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.03.25.cfg.bin")))
            {
                charaparam.SeekOf<uint>(0xA2D54CD5, 0x10);
                charaparam.Skip(0x08);
                YW3Support.Charaparam[] yokaiParams = charaparam.ReadMultipleStruct<YW3Support.Charaparam>(charaparam.ReadValue<int>());

                // Charabase
                using (BinaryDataReader charabase = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_base_0.03.25.cfg.bin")))
                {
                    charabase.SeekOf<uint>(0x76687850, 0x10);
                    charabase.Skip(0x08);
                    YW3Support.Charabase[] yokaiBases = charabase.ReadMultipleStruct<YW3Support.Charabase>(charabase.ReadValue<int>());

                    // Link
                    Dictionary<YW3Support.Charaparam, YW3Support.Charabase> yokaiConfigs = yokaiParams
                        .Join(yokaiBases, x => x.BaseID, y => y.BaseID, (x, y) => new { Charaparam = x, Charabase = y })
                        .ToDictionary(z => z.Charaparam, z => z.Charabase);

                    // Create Yokai Object List
                    yokais = yokaiConfigs.Select(yokaiConfig => new Yokai
                    {
                        Name = yokaiNames.GetNounText(yokaiConfig.Value.NameID, 0),
                        ModelName = yokaiConfig.Value.Model.GetText(),
                        Rank = yokaiConfig.Value.Rank,
                        Tribe = yokaiConfig.Value.Tribe,
                        MinStat = new int[] { yokaiConfig.Key.MinStat.HP, yokaiConfig.Key.MinStat.Strength, yokaiConfig.Key.MinStat.Spirit, yokaiConfig.Key.MinStat.Defense, yokaiConfig.Key.MinStat.Speed },
                        MaxStat = new int[] { yokaiConfig.Key.MaxStat.HP, yokaiConfig.Key.MaxStat.Strength, yokaiConfig.Key.MaxStat.Spirit, yokaiConfig.Key.MaxStat.Defense, yokaiConfig.Key.MaxStat.Speed },
                        Strongest = (byte)yokaiConfig.Key.Strongest,
                        Weakness = (byte)yokaiConfig.Key.Strongest,
                        AttackID = yokaiConfig.Key.AttackID,
                        TechniqueID = yokaiConfig.Key.TechniqueID,
                        InspiritID = yokaiConfig.Key.InspiritID,
                        SoultimateID = yokaiConfig.Key.SoultimateID,
                        SkillID = yokaiConfig.Key.SkillID,
                        //Money = yokaiConfig.Key.Money,
                        //Experience = yokaiConfig.Key.Experience,
                        //DropID = new uint[] { yokaiConfig.Key.Drop1.ID, yokaiConfig.Key.Drop2.ID },
                        //DropRate = new int[] { yokaiConfig.Key.Drop1.Rate, yokaiConfig.Key.Drop2.Rate },
                        ExperienceCurve = yokaiConfig.Key.ExperienceCurve,
                        EvolveOffset = yokaiConfig.Key.EvolveOffset,
                        MedaliumOffset = yokaiConfig.Key.MedalliumOffset,
                        Medal = new Point(yokaiConfig.Value.Medal.X, yokaiConfig.Value.Medal.Y),
                        ScoutableID = (uint)yokaiConfig.Key.ScoutableID,
                        BaseID = yokaiConfig.Key.BaseID,
                        ParamID = yokaiConfig.Key.ParamID,
                        Statut = new Statut
                        {
                            IsLegendary = yokaiConfig.Value.IsLegendary,
                            IsRare = yokaiConfig.Value.IsRare,
                            IsBoss = yokaiConfig.Value.Tribe == 0x09 || yokaiConfig.Value.Tribe == 0x00,
                            IsScoutable = yokaiConfig.Key.ScoutableID != 0x00,
                            IsStatic = !(yokaiConfig.Key.ScoutableID != 0x00) && (yokaiConfig.Value.Tribe == 0x09 || yokaiConfig.Value.Tribe == 0x00),
                            IsClassic = yokaiConfig.Value.IsClassic,
                            IsDeva = yokaiConfig.Value.IsDeva,
                            IsMerican = yokaiConfig.Value.IsMerican,
                            IsMystery = yokaiConfig.Value.IsMystery,
                            IsTreasure = yokaiConfig.Value.IsTreasure,
                        },
                    }).ToList();

                    yokais = yokais.Select((yokai, index) =>
                    {
                        if (yokai.Name == null)
                        {
                            yokai.Name = "Yokai n°" + index;
                        }

                        return yokai;
                    }).ToList();
                }
            }

            return yokais;
        }

        public List<Evolution> GetEvolutions(List<Yokai> yokais)
        {
            List<Evolution> evolutions;

            using (BinaryDataReader charaparam = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.03.25.cfg.bin")))
            {
                charaparam.SeekOf<uint>(0x568635E4, 0x10);
                charaparam.Skip(0x08);
                YW3Support.Evolution[] yokaiEvolutions = charaparam.ReadMultipleStruct<YW3Support.Evolution>(charaparam.ReadValue<int>());

                // Create Evolution Object List
                evolutions = yokaiEvolutions.Select((yokaiEvolution, index) => new Evolution()
                {
                    EvolveTo = yokaiEvolution.EvolveToID,
                    Level = yokaiEvolution.Level,
                    BaseYokai = yokais.FirstOrDefault(yokai => yokai.EvolveOffset == index).ParamID,
                }).ToList();
            }

            return evolutions;
        }

        public void SaveYokais(List<Yokai> yokais, List<Evolution> evolutions)
        {
            // Charaparam
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter charaparamWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader charaparamReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.03.25.cfg.bin")))
                    {
                        int entryCount = charaparamReader.ReadValue<int>();
                        long tableOffset = charaparamReader.ReadValue<int>();

                        charaparamReader.SeekOf<uint>(0xA2D54CD5, 0x10);
                        charaparamWriter.Write(charaparamReader.GetSection(0, (int)charaparamReader.Position));
                        charaparamReader.Skip(0x08);
                        int yokaiParamCount = charaparamReader.ReadValue<int>();

                        // Header Param
                        charaparamWriter.Write(0xA2D54CD5);
                        charaparamWriter.Write(0xFFFF0101);
                        charaparamWriter.Write(yokaiParamCount);

                        // Write Param
                        for (int i = 0; i < yokaiParamCount; i++)
                        {
                            // Get Yokai Param
                            YW3Support.Charaparam yokaiParam = charaparamReader.ReadStruct<YW3Support.Charaparam>();

                            // Try to find Yokai Param
                            Yokai yokai = yokais.FirstOrDefault(x => x.ParamID == yokaiParam.ParamID);
                            if (yokai != null)
                            {
                                // Replace
                                yokaiParam.ReplaceWith(yokai);
                            }

                            // Write 
                            charaparamWriter.WriteStruct<YW3Support.Charaparam>(yokaiParam);
                        }

                        long position = charaparamReader.Position;

                        // Reach Evolution
                        charaparamReader.SeekOf<uint>(0x568635E4, 0x10);
                        charaparamWriter.Write(charaparamReader.GetSection((uint)position, (int)(charaparamReader.Position - position)));
                        charaparamReader.Skip(0x08);
                        int evolutionCount = charaparamReader.ReadValue<int>();
                        YW3Support.Evolution[] evolutionsSkip = charaparamReader.ReadMultipleStruct<YW3Support.Evolution>(evolutionCount);

                        // Header Evolution
                        charaparamWriter.Write(0x568635E4);
                        charaparamWriter.Write(0xFFFF0101);
                        charaparamWriter.Write(evolutions.Count);

                        // Write Evolution
                        foreach (Evolution evolution in evolutions)
                        {
                            charaparamWriter.Write(0x1448C0EF);
                            charaparamWriter.Write(0xFFFF0502);
                            charaparamWriter.Write(evolution.EvolveTo);
                            charaparamWriter.Write(evolution.Level);
                        }

                        // Write End File
                        position = charaparamReader.Position;
                        charaparamReader.SeekOf<uint>(0xA8E69AE2, 0x10);
                        charaparamWriter.Write(charaparamReader.GetSection((uint)position, (int)(charaparamReader.Position - position)));
                        charaparamWriter.Write(0xA8E69AE2);
                        charaparamWriter.WriteAlignment();
                        charaparamReader.Seek((uint)tableOffset);
                        tableOffset = charaparamWriter.Position;
                        charaparamWriter.Write(charaparamReader.GetSection((int)(charaparamReader.Length - charaparamReader.Position)));

                        // Edit Header file
                        charaparamWriter.Seek(0x0);
                        entryCount -= yokaiParamCount;
                        entryCount -= evolutionCount;
                        charaparamWriter.Write(entryCount + yokaiParamCount + evolutions.Count);
                        charaparamWriter.Write((int)tableOffset);
                    }

                    // Replace File
                    Game.Directory.GetFolderFromFullPath("/data/res/character/").Files["chara_param_0.03.25.cfg.bin"].ByteContent = memoryStream.ToArray();
                }
            }

            // Charabase
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter charabaseWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader charabaseReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_base_0.03.25.cfg.bin")))
                    {
                        charabaseReader.SeekOf<uint>(0x76687850, 0x10);
                        charabaseWriter.Write(charabaseReader.GetSection(0, (int)charabaseReader.Position));
                        charabaseReader.Skip(0x08);
                        int yokaiBaseCount = charabaseReader.ReadValue<int>();

                        // Header Param
                        charabaseWriter.Write(0x76687850);
                        charabaseWriter.Write(0xFFFF0101);
                        charabaseWriter.Write(yokaiBaseCount);

                        // Write Param
                        for (int i = 0; i < yokaiBaseCount; i++)
                        {
                            // Get Yokai Param
                            YW3Support.Charabase yokaiBase = charabaseReader.ReadStruct<YW3Support.Charabase>();

                            // Try to find Yokai Base
                            Yokai yokai = yokais.FirstOrDefault(x => x.BaseID == yokaiBase.BaseID);
                            if (yokai != null)
                            {
                                // Replace
                                yokaiBase.ReplaceWith(yokai);
                            }

                            // Write 
                            charabaseWriter.WriteStruct<YW3Support.Charabase>(yokaiBase);
                        }

                        // Write end of file
                        charabaseWriter.Write(charabaseReader.GetSection((int)(charabaseReader.Length - charabaseReader.Position)));
                    }

                    // Replace File
                    Game.Directory.GetFolderFromFullPath("/data/res/character/").Files["chara_base_0.03.25.cfg.bin"].ByteContent = memoryStream.ToArray();
                }
            }
        }

        public List<LegendSeal> GetLegendaries()
        {
            List<LegendSeal> legendaries = new List<LegendSeal>();

            using (BinaryDataReader legendconfig = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/legend_config_0.01.cfg.bin")))
            {
                legendconfig.SeekOf<uint>(0x835F55C0, 0x10);
                legendconfig.Skip(0x08);
                YW3Support.LegendSeal[] legendSeals = legendconfig.ReadMultipleStruct<YW3Support.LegendSeal>(legendconfig.ReadValue<int>());

                // Create Legend Seal Object List
                legendaries = legendSeals.Select(legendseal => new LegendSeal()
                {
                    SealID = legendseal.SealdID,
                    LegendaryParamID = legendseal.LegendaryParamID,
                    RequirmentParamID = legendseal.Seals.Select(x => x.ParamID).ToArray(),
                }).ToList();
            }

            return legendaries;
        }

        public void SaveLegendaries(List<LegendSeal> legendaries, bool spoil)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter legendconfigWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader legendconfigReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/legend_config_0.01.cfg.bin")))
                    {
                        int entryCount = legendconfigReader.ReadValue<int>();
                        long tableOffset = legendconfigReader.ReadValue<int>();

                        legendconfigReader.SeekOf<uint>(0x835F55C0, 0x10);
                        legendconfigWriter.Write(legendconfigReader.GetSection(0, (int)legendconfigReader.Position));
                        legendconfigReader.Skip(0x08);
                        int legendSealsCount = legendconfigReader.ReadValue<int>();

                        // Header legend
                        legendconfigWriter.Write(0x835F55C0);
                        legendconfigWriter.Write(0xFFFF0101);
                        legendconfigWriter.Write(legendaries.Count);

                        // Write legend
                        foreach (LegendSeal legendary in legendaries)
                        {
                            legendconfigWriter.Write(0x66C81CEA);
                            legendconfigWriter.Write(0x55555514);
                            legendconfigWriter.Write(0xFFFF5555);
                            legendconfigWriter.Write(legendary.SealID);
                            legendconfigWriter.Write(legendary.LegendaryParamID);

                            foreach (uint seal in legendary.RequirmentParamID)
                            {
                                legendconfigWriter.Write(seal);
                                legendconfigWriter.Write(Convert.ToInt32(spoil));
                            }

                            legendconfigWriter.Write(0x08);
                            legendconfigWriter.Write(0x00);
                        }

                        // Write end of file
                        legendconfigWriter.Write(0xD03C0AA2);
                        legendconfigWriter.WriteAlignment();
                        legendconfigReader.Seek((uint)tableOffset);
                        tableOffset = legendconfigWriter.Position;
                        legendconfigWriter.Write(legendconfigReader.GetSection((int)(legendconfigReader.Length - legendconfigReader.Position)));

                        // Edit Header file
                        legendconfigWriter.Seek(0x0);
                        entryCount -= legendSealsCount;
                        legendconfigWriter.Write(entryCount + legendaries.Count);
                        legendconfigWriter.Write((uint)tableOffset);
                    }

                    // Replace File
                    Game.Directory.GetFolderFromFullPath("/data/res/character/").Files["legend_config_0.01.cfg.bin"].ByteContent = memoryStream.ToArray();
                }
            }
        }

        public List<Fusion> GetFusions()
        {
            List<Fusion> fusions;

            using (BinaryDataReader combineconfig = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/shop/combine_config.cfg.bin")))
            {
                combineconfig.SeekOf<uint>(0x9DFA1616, 0x10);
                combineconfig.Skip(0x08);
                YW3Support.Fusion[] yokaiFusions = combineconfig.ReadMultipleStruct<YW3Support.Fusion>(combineconfig.ReadValue<int>());

                // Create Fusion Object List
                fusions = yokaiFusions.Select(yokaiFusion => new Fusion()
                {
                    BaseYokai = yokaiFusion.BaseID,
                    BaseIsItem = yokaiFusion.BaseIsItem,
                    Material = yokaiFusion.MaterialID,
                    MaterialIsItem = yokaiFusion.MaterialIsItem,
                    EvolveTo = yokaiFusion.EvolveToID,
                    EvolveToIsItem = yokaiFusion.EvolveToIsItem,
                    FusionID = yokaiFusion.FusionID,
                    FusionIsItem = yokaiFusion.fusionIsItem,
                }).ToList();
            }

            return fusions;
        }

        public void SaveFusions(List<Fusion> fusions)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter combineconfigWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader combineconfigReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/shop/combine_config.cfg.bin")))
                    {
                        int entryCount = combineconfigReader.ReadValue<int>();
                        long tableOffset = combineconfigReader.ReadValue<int>();

                        combineconfigReader.SeekOf<uint>(0x9DFA1616, 0x10);
                        combineconfigWriter.Write(combineconfigReader.GetSection(0, (int)combineconfigReader.Position));
                        combineconfigReader.Skip(0x08);
                        int fusionCount = combineconfigReader.ReadValue<int>();

                        // Header fusion
                        combineconfigWriter.Write(0x9DFA1616);
                        combineconfigWriter.Write(0xFFFF0101);
                        combineconfigWriter.Write(fusions.Count);

                        // Write fusion
                        foreach (Fusion fusion in fusions)
                        {
                            combineconfigWriter.Write(0xF9D0FE0F);
                            combineconfigWriter.Write(0xFF555508);
                            combineconfigWriter.Write(Convert.ToInt32(fusion.BaseIsItem));
                            combineconfigWriter.Write(fusion.BaseYokai);
                            combineconfigWriter.Write(Convert.ToInt32(fusion.MaterialIsItem));
                            combineconfigWriter.Write(fusion.Material);
                            combineconfigWriter.Write(Convert.ToInt32(fusion.EvolveToIsItem));
                            combineconfigWriter.Write(fusion.EvolveTo);
                            combineconfigWriter.Write(fusion.FusionID);
                            combineconfigWriter.Write(Convert.ToInt32(fusion.FusionIsItem));
                        }

                        // Write end of file
                        combineconfigWriter.Write(0x7950A9EA);
                        combineconfigWriter.WriteAlignment();
                        combineconfigReader.Seek((uint)tableOffset);
                        tableOffset = combineconfigWriter.Position;
                        combineconfigWriter.Write(combineconfigReader.GetSection((int)(combineconfigReader.Length - combineconfigReader.Position)));

                        // Edit Header file
                        combineconfigWriter.Seek(0x0);
                        entryCount -= fusionCount;
                        combineconfigWriter.Write(entryCount + fusions.Count);
                        combineconfigWriter.Write((uint)tableOffset);
                    }

                    // Replace File
                    Game.Directory.GetFolderFromFullPath("/data/res/shop/").Files["combine_config.cfg.bin"].ByteContent = memoryStream.ToArray();
                }
            }
        }

        public List<uint> GetSouls()
        {
            return null;
        }

        public void SaveSouls(List<uint> souls)
        {

        }

        public List<(uint, int)> GetEncounter()
        {
            List<(uint, int)> encounters;

            using (BinaryDataReader commonencReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/battle/common_enc_0.01.cfg.bin")))
            {
                commonencReader.SeekOf<uint>(0x5CF8BAE4, 0x10);
                commonencReader.Skip(0x08);
                YW3Support.Encounter[] encounterData = commonencReader.ReadMultipleStruct<YW3Support.Encounter>(commonencReader.ReadValue<int>());

                encounters = encounterData.Select(encounter => (encounter.ParamID, encounter.Level)).ToList();
            }

            return encounters;
        }

        public void SaveEncounter(List<(uint, int)> encounters)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter commonencWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader commonencReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/battle/common_enc_0.01.cfg.bin")))
                    {
                        int entryCount = commonencReader.ReadValue<int>();
                        long tableOffset = commonencReader.ReadValue<int>();

                        commonencReader.SeekOf<uint>(0x5CF8BAE4, 0x10);
                        commonencWriter.Write(commonencReader.GetSection(0, (int)commonencReader.Position));
                        commonencReader.Skip(0x08);
                        int encounterCount = commonencReader.ReadValue<int>();
                        YW3Support.Encounter[] encounterData = commonencReader.ReadMultipleStruct<YW3Support.Encounter>(encounterCount);

                        // Header encounter
                        commonencWriter.Write(0x5CF8BAE4);
                        commonencWriter.Write(0xFFFF0101);
                        commonencWriter.Write(encounterCount);

                        // Write encounter
                        for (int i = 0; i < encounterCount; i++)
                        {
                            YW3Support.Encounter encounter = encounterData[i];
                            encounter.ParamID = encounters[i].Item1;
                            encounter.Level = encounters[i].Item2;
                            commonencWriter.WriteStruct(encounter);
                        }

                        // Write end of file
                        commonencWriter.Write(0xCE654EF7);
                        commonencWriter.WriteAlignment();
                        commonencReader.Seek((uint)tableOffset);
                        tableOffset = commonencWriter.Position;
                        commonencWriter.Write(commonencReader.GetSection((int)(commonencReader.Length - commonencReader.Position)));

                        // Edit Header file
                        commonencWriter.Seek(0x0);
                        entryCount -= encounterCount;
                        commonencWriter.Write(entryCount + encounterCount);
                        commonencWriter.Write((uint)tableOffset);
                    }

                    // Replace File
                    Game.Directory.GetFolderFromFullPath("/data/res/battle/").Files["common_enc_0.01.cfg.bin"].ByteContent = memoryStream.ToArray();
                }
            }
        }

        public List<(uint, int)> GetWorldEncounter(byte[] file)
        {
            List<(uint, int)> encounters;

            using (BinaryDataReader commonencReader = new BinaryDataReader(file))
            {
                commonencReader.SeekOf<uint>(0x5CF8BAE4, 0x10);
                commonencReader.Skip(0x08);
                YW3Support.WorldEncounter[] encounterData = commonencReader.ReadMultipleStruct<YW3Support.WorldEncounter>(commonencReader.ReadValue<int>());

                encounters = encounterData.Select(encounter => (encounter.ParamID, encounter.Level)).ToList();
            }

            return encounters;
        }

        public byte[] SaveWorldEncounter(List<(uint, int)> encounters, byte[] file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter commonencWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader commonencReader = new BinaryDataReader(file))
                    {
                        int entryCount = commonencReader.ReadValue<int>();
                        long tableOffset = commonencReader.ReadValue<int>();

                        commonencReader.SeekOf<uint>(0x5CF8BAE4, 0x10);
                        commonencWriter.Write(commonencReader.GetSection(0, (int)commonencReader.Position));
                        commonencReader.Skip(0x08);
                        int encounterCount = commonencReader.ReadValue<int>();
                        YW3Support.Encounter[] encounterData = commonencReader.ReadMultipleStruct<YW3Support.Encounter>(encounterCount);

                        // Header fusion
                        commonencWriter.Write(0x5CF8BAE4);
                        commonencWriter.Write(0xFFFF0101);
                        commonencWriter.Write(encounterCount);

                        // Write encounter
                        for (int i = 0; i < encounters.Count; i++)
                        {
                            YW3Support.Encounter encounter = encounterData[i];
                            encounter.ParamID = encounters[i].Item1;
                            encounter.Level = encounters[i].Item2;
                            commonencWriter.WriteStruct(encounter);
                        }

                        // Write end of file
                        commonencWriter.Write(0xCE654EF7);
                        commonencWriter.WriteAlignment();
                        commonencReader.Seek((uint)tableOffset);
                        tableOffset = commonencWriter.Position;
                        commonencWriter.Write(commonencReader.GetSection((int)(commonencReader.Length - commonencReader.Position)));

                        // Edit Header file
                        commonencWriter.Seek(0x0);
                        entryCount -= encounterCount;
                        commonencWriter.Write(entryCount + encounterCount);
                        commonencWriter.Write((uint)tableOffset);
                    }

                    return memoryStream.ToArray();
                }
            }
        }

        public List<uint> GetCapsule()
        {
            return null;
        }

        public void SaveCapsule(List<uint> capsules)
        {
        }

        public List<uint> GetShop(string fileName)
        {
            return null;
        }

        public void SaveShop(List<uint> capsules, string fileName)
        {
        }

        public List<uint> GetTreasureBox(byte[] file)
        {
            return null;
        }

        public byte[] SaveTreasureBox(List<uint> treasures, byte[] file)
        {
            return null;
        }

        public void FixStory()
        {
        }

        public void FixYokai(List<Yokai> yokais)
        {
            if (yokais.Any(x => x.ParamID == 0xFDC725BF))
            {
                Yokai treetter = yokais.Find(x => x.ParamID == 0xFDC725BF);
                treetter.Rank = 0x00;
            }
        }
    }
}
