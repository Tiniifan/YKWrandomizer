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
using YKWrandomizer.Level5.Archive.ARC0;

namespace YKWrandomizer.Yokai_Watch.Games.YW1
{
    public class YW1 : IGame
    {
        public string Name => "Yo-kai Watch 1";

        public Dictionary<uint, string> Attacks => Common.Attacks.YW1;

        public Dictionary<uint, string> Techniques => Common.Techniques.YW1;

        public Dictionary<uint, string> Inspirits => Common.Inspirits.YW1;

        public Dictionary<uint, string> Soultimates => Common.Soultimates.YW1;

        public Dictionary<uint, string> Skills => Common.Skills.YW1;

        public Dictionary<uint, string> Items => Common.Items.YW1;

        public Dictionary<int, string> Tribes => Common.Tribes.YW1;

        public Dictionary<uint, string> YokaiScoutable => Common.YokaiScoutable.YW1;

        public Dictionary<uint, string> YokaiStatic => Common.YokaiStatic.YW1;

        public Dictionary<uint, string> YokaiBoss => Common.YokaiBoss.YW1;

        public Dictionary<uint, string> YokaiUnused => Common.YokaiUnused.YW1;

        public Dictionary<string, List<uint>> YokaiGiven => Common.GivenYokais.YW1;

        public List<uint> YokaiUnscoutableAutorized => Common.YokaiUnscoutableAutorized.YW1;

        public ARC0 Game { get; set; }

        public YW1(string path)
        {
            Game = new ARC0(new FileStream(path, FileMode.Open));
        }

        public List<Yokai> GetYokais()
        {
            List<Yokai> yokais;

            using (BinaryDataReader charaparam = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.02.cfg.bin")))
            {
                charaparam.SeekOf<uint>(0xEEEFA832, 0x10);
                charaparam.Skip(0x08);
                YW1Support.Charaparam[] yokaiParams = charaparam.ReadMultipleStruct<YW1Support.Charaparam>(charaparam.ReadValue<int>());

                // Charabase
                using (BinaryDataReader charabase = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_base_0.04j.cfg.bin")))
                {
                    charabase.SeekOf<uint>(0x76687850, 0x10);
                    charabase.Skip(0x08);
                    YW1Support.Charabase[] yokaiBases = charabase.ReadMultipleStruct<YW1Support.Charabase>(charabase.ReadValue<int>());

                    // Link
                    Dictionary<YW1Support.Charaparam, YW1Support.Charabase> yokaiConfigs = yokaiParams
                        .Join(yokaiBases, x => x.BaseID, y => y.BaseID, (x, y) => new { Charaparam = x, Charabase = y })
                        .ToDictionary(z => z.Charaparam, z => z.Charabase);

                    // Create Yokai Object List
                    yokais = yokaiConfigs.Select(yokaiConfig => new Yokai
                    {
                        ModelName = yokaiConfig.Value.Model.GetText(),
                        Rank = yokaiConfig.Value.Rank,
                        Tribe = yokaiConfig.Key.Tribe,
                        MinStat = new int[] { yokaiConfig.Key.BaseStat.HP, yokaiConfig.Key.BaseStat.Strength, yokaiConfig.Key.BaseStat.Spirit, yokaiConfig.Key.BaseStat.Defense, yokaiConfig.Key.BaseStat.Speed },
                        AttributeDamage = yokaiConfig.Key.AttributesDamage.GetAttributes(),
                        AttackID = yokaiConfig.Key.AttackID,
                        TechniqueID = yokaiConfig.Key.TechniqueID,
                        InspiritID = yokaiConfig.Key.InspiritID,
                        SoultimateID = yokaiConfig.Key.SoultimateID,
                        SkillID = yokaiConfig.Key.SkillID,
                        Money = yokaiConfig.Key.Money,
                        Experience = yokaiConfig.Key.Experience,
                        DropID = new uint[] { yokaiConfig.Key.Drop1.ID, yokaiConfig.Key.Drop2.ID },
                        DropRate = new int[] { yokaiConfig.Key.Drop1.Rate, yokaiConfig.Key.Drop2.Rate },
                        ExperienceCurve = yokaiConfig.Key.ExperienceCurve,
                        EvolveOffset = yokaiConfig.Key.EvolveOffset,
                        MedaliumOffset = yokaiConfig.Key.MedaliumOffset,
                        Medal = new Point(yokaiConfig.Value.Medal.X, yokaiConfig.Value.Medal.Y),
                        ScoutableID = yokaiConfig.Key.ScoutableID,
                        BaseID = yokaiConfig.Key.BaseID,
                        ParamID = yokaiConfig.Key.ParamID,
                        Statut = new Statut
                        {
                            IsLegendary = yokaiConfig.Value.IsLegendary,
                            IsRare = yokaiConfig.Value.IsRare,
                            IsBoss = yokaiConfig.Key.Tribe == 0x09 || yokaiConfig.Key.Tribe == 0x00,
                            IsScoutable = yokaiConfig.Key.ScoutableID != 0x00,
                            IsStatic = !(yokaiConfig.Key.ScoutableID != 0x00) && (yokaiConfig.Key.Tribe == 0x09 || yokaiConfig.Key.Tribe == 0x00),
                        },
                    }).ToList();

                    yokais = yokais.Select((yokai, index) =>
                    {

                        if (YokaiScoutable.ContainsKey(yokai.ParamID))
                        {
                            yokai.Name = YokaiScoutable[yokai.ParamID];
                            yokai.Statut.IsBoss = false;
                            yokai.Statut.IsScoutable = true;
                            yokai.Statut.IsStatic = false;
                        }
                        else if (YokaiStatic.ContainsKey(yokai.ParamID))
                        {
                            yokai.Name = YokaiStatic[yokai.ParamID];
                            yokai.Statut.IsBoss = false;
                            yokai.Statut.IsScoutable = false;
                            yokai.Statut.IsStatic = true;
                        }
                        else if (YokaiBoss.ContainsKey(yokai.ParamID))
                        {
                            yokai.Name = YokaiBoss[yokai.ParamID];
                            yokai.Statut.IsBoss = true;
                            yokai.Statut.IsScoutable = false;
                            yokai.Statut.IsStatic = false;
                        }
                        else if (YokaiUnused.ContainsKey(yokai.ParamID))
                        {
                            // Let the randomizer determine the type of yokai
                            yokai.Name = YokaiUnused[yokai.ParamID];
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

            using (BinaryDataReader charaparam = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.02.cfg.bin")))
            {
                charaparam.SeekOf<uint>(0x4C6181CB, 0x10);
                charaparam.Skip(0x08);
                YW1Support.Evolution[] yokaiEvolutions = charaparam.ReadMultipleStruct<YW1Support.Evolution>(charaparam.ReadValue<int>());

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
                    using (BinaryDataReader charaparamReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.02.cfg.bin")))
                    {
                        int entryCount = charaparamReader.ReadValue<int>();
                        long tableOffset = charaparamReader.ReadValue<int>();

                        charaparamReader.SeekOf<uint>(0xEEEFA832, 0x10);
                        charaparamWriter.Write(charaparamReader.GetSection(0, (int)charaparamReader.Position));
                        charaparamReader.Skip(0x08);
                        int yokaiParamCount = charaparamReader.ReadValue<int>();

                        // Header Param
                        charaparamWriter.Write(0xEEEFA832);
                        charaparamWriter.Write(0xFFFF0101);
                        charaparamWriter.Write(yokaiParamCount);

                        // Write Param
                        for (int i = 0; i < yokaiParamCount; i++)
                        {
                            // Get Yokai Param
                            YW1Support.Charaparam yokaiParam = charaparamReader.ReadStruct<YW1Support.Charaparam>();

                            // Try to find Yokai Param
                            Yokai yokai = yokais.FirstOrDefault(x => x.ParamID == yokaiParam.ParamID);
                            if (yokai != null)
                            {
                                // Replace
                                yokaiParam.ReplaceWith(yokai);
                            }

                            // Write 
                            charaparamWriter.WriteStruct<YW1Support.Charaparam>(yokaiParam);
                        }

                        long position = charaparamReader.Position;

                        // Reach Evolution
                        charaparamReader.SeekOf<uint>(0x4C6181CB, 0x10);
                        charaparamWriter.Write(charaparamReader.GetSection((uint)position, (int)(charaparamReader.Position - position)));
                        charaparamReader.Skip(0x08);
                        int evolutionCount = charaparamReader.ReadValue<int>();
                        YW1Support.Evolution[] evolutionsSkip = charaparamReader.ReadMultipleStruct<YW1Support.Evolution>(evolutionCount);

                        // Header Evolution
                        charaparamWriter.Write(0x4C6181CB);
                        charaparamWriter.Write(0xFFFF0101);
                        charaparamWriter.Write(evolutions.Count);

                        // Write Evolution
                        foreach (Evolution evolution in evolutions)
                        {
                            charaparamWriter.Write(0x1448C0EF);
                            charaparamWriter.Write(0xFFFF0502);
                            charaparamWriter.Write(evolution.Level);
                            charaparamWriter.Write(evolution.EvolveTo);
                        }

                        // Write End File
                        position = charaparamReader.Position;
                        charaparamReader.SeekOf<uint>(0x257EC7C3, 0x10);
                        charaparamWriter.Write(charaparamReader.GetSection((uint)position, (int)(charaparamReader.Position - position)));
                        charaparamWriter.Write(0x257EC7C3);
                        charaparamWriter.WriteAlignment();
                        charaparamReader.Seek((uint)tableOffset);
                        tableOffset = charaparamWriter.Position;
                        charaparamWriter.Write(charaparamReader.GetSection((int)(charaparamReader.Length - charaparamReader.Position)));

                        // Edit Header file
                        charaparamWriter.Seek(0x0);
                        entryCount -= yokaiParamCount;
                        entryCount -= evolutionCount;
                        charaparamWriter.Write(entryCount + yokaiParamCount + evolutions.Count);
                        charaparamWriter.Write(tableOffset);
                    }

                    // Replace File
                    Game.Directory.GetFolderFromFullPath("/data/res/character/").Files["chara_param_0.02.cfg.bin"].ByteContent = memoryStream.ToArray();
                }
            }

            // Charabase
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter charabaseWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader charabaseReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/character/chara_base_0.04j.cfg.bin")))
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
                            YW1Support.Charabase yokaiBase = charabaseReader.ReadStruct<YW1Support.Charabase>();

                            // Try to find Yokai Base
                            Yokai yokai = yokais.FirstOrDefault(x => x.BaseID == yokaiBase.BaseID);
                            if (yokai != null)
                            {
                                // Replace
                                yokaiBase.ReplaceWith(yokai);
                            }

                            // Write 
                            charabaseWriter.WriteStruct<YW1Support.Charabase>(yokaiBase);
                        }

                        // Write end of file
                        charabaseWriter.Write(charabaseReader.GetSection((int)(charabaseReader.Length - charabaseReader.Position)));
                    }

                    // Replace File
                    Game.Directory.GetFolderFromFullPath("/data/res/character/").Files["chara_base_0.04j.cfg.bin"].ByteContent = memoryStream.ToArray();
                }
            }
        }

        public List<LegendSeal> GetLegendaries()
        {
            List<LegendSeal> legendaries = new List<LegendSeal>();

            using (BinaryDataReader legendconfig = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/legend/legend_config.cfg.bin")))
            {
                legendconfig.SeekOf<uint>(0xEA858825, 0x10);
                legendconfig.Skip(0x08);
                YW1Support.LegendSeal[] legendSeals = legendconfig.ReadMultipleStruct<YW1Support.LegendSeal>(legendconfig.ReadValue<int>());

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
                    using (BinaryDataReader legendconfigReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/legend/legend_config.cfg.bin")))
                    {
                        int entryCount = legendconfigReader.ReadValue<int>();
                        long tableOffset = legendconfigReader.ReadValue<int>();

                        legendconfigReader.SeekOf<uint>(0xEA858825, 0x10);
                        legendconfigWriter.Write(legendconfigReader.GetSection(0, (int)legendconfigReader.Position));
                        legendconfigReader.Skip(0x08);
                        int legendSealsCount = legendconfigReader.ReadValue<int>();

                        // Header legend
                        legendconfigWriter.Write(0xEA858825);
                        legendconfigWriter.Write(0xFFFF0101);
                        legendconfigWriter.Write(legendaries.Count);

                        // Write legend
                        foreach (LegendSeal legendary in legendaries)
                        {
                            legendconfigWriter.Write(0x66C81CEA);
                            legendconfigWriter.Write(0x55555513);
                            legendconfigWriter.Write(0xFFFF1555);
                            legendconfigWriter.Write(legendary.SealID);
                            legendconfigWriter.Write(legendary.LegendaryParamID);

                            foreach (uint seal in legendary.RequirmentParamID)
                            {
                                legendconfigWriter.Write(seal);
                                legendconfigWriter.Write(Convert.ToInt32(spoil));
                            }

                            legendconfigWriter.Write(0x08);
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
                    Game.Directory.GetFolderFromFullPath("/data/res/legend/").Files["legend_config.cfg.bin"].ByteContent = memoryStream.ToArray();
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
                YW1Support.Fusion[] yokaiFusions = combineconfig.ReadMultipleStruct<YW1Support.Fusion>(combineconfig.ReadValue<int>());

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
                            combineconfigWriter.Write(0xFF155507);
                            combineconfigWriter.Write(Convert.ToInt32(fusion.BaseIsItem));
                            combineconfigWriter.Write(fusion.BaseYokai);
                            combineconfigWriter.Write(Convert.ToInt32(fusion.MaterialIsItem));
                            combineconfigWriter.Write(fusion.Material);
                            combineconfigWriter.Write(Convert.ToInt32(fusion.EvolveToIsItem));
                            combineconfigWriter.Write(fusion.EvolveTo);
                            combineconfigWriter.Write(fusion.FusionID);
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

            using (BinaryDataReader commonencReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/battle/common_enc_0.02m.cfg.bin")))
            {
                commonencReader.SeekOf<uint>(0x5CF8BAE4, 0x10);
                commonencReader.Skip(0x08);
                YW1Support.Encounter[] encounterData = commonencReader.ReadMultipleStruct<YW1Support.Encounter>(commonencReader.ReadValue<int>());

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
                    using (BinaryDataReader commonencReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/battle/common_enc_0.02m.cfg.bin")))
                    {
                        int entryCount = commonencReader.ReadValue<int>();
                        long tableOffset = commonencReader.ReadValue<int>();

                        commonencReader.SeekOf<uint>(0x5CF8BAE4, 0x10);
                        commonencWriter.Write(commonencReader.GetSection(0, (int)commonencReader.Position));
                        commonencReader.Skip(0x08);
                        int encounterCount = commonencReader.ReadValue<int>();
                        YW1Support.Encounter[] encounterData = commonencReader.ReadMultipleStruct<YW1Support.Encounter>(encounterCount);

                        // Header encounter
                        commonencWriter.Write(0x5CF8BAE4);
                        commonencWriter.Write(0xFFFF0101);
                        commonencWriter.Write(encounterCount);

                        // Write encounter
                        for (int i = 0; i < encounters.Count; i ++)
                        {
                            YW1Support.Encounter encounter = encounterData[i];
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
                    Game.Directory.GetFolderFromFullPath("/data/res/battle/").Files["common_enc_0.02m.cfg.bin"].ByteContent = memoryStream.ToArray();
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
                YW1Support.WorldEncounter[] encounterData = commonencReader.ReadMultipleStruct<YW1Support.WorldEncounter>(commonencReader.ReadValue<int>());

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
                        YW1Support.Encounter[] encounterData = commonencReader.ReadMultipleStruct<YW1Support.Encounter>(encounterCount);

                        // Header fusion
                        commonencWriter.Write(0x5CF8BAE4);
                        commonencWriter.Write(0xFFFF0101);
                        commonencWriter.Write(encounterCount);

                        // Write encounter
                        for (int i = 0; i < encounters.Count; i++)
                        {
                            YW1Support.Encounter encounter = encounterData[i];
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
            List<uint> capsules;

            using (BinaryDataReader commonencReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/capsule/capsule_config.cfg.bin")))
            {
                commonencReader.SeekOf<uint>(0x61CFF7F4, 0x10);
                commonencReader.Skip(0x08);
                YW1Support.CapsuleItem[] capsuleItems = commonencReader.ReadMultipleStruct<YW1Support.CapsuleItem>(commonencReader.ReadValue<int>());

                capsules = capsuleItems.Select(capsuleItem => capsuleItem.ID).ToList();
            }

            return capsules;
        }

        public void SaveCapsule(List<uint> capsules)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter capsuleconfigWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader capsuleconfigreader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/capsule/capsule_config.cfg.bin")))
                    {
                        capsuleconfigreader.SeekOf<uint>(0x61CFF7F4, 0x10);
                        capsuleconfigWriter.Write(capsuleconfigreader.GetSection(0, (int)capsuleconfigreader.Position));
                        capsuleconfigreader.Skip(0x08);
                        int capsuleCount = capsuleconfigreader.ReadValue<int>();

                        // Header Capsule Config
                        capsuleconfigWriter.Write(0x61CFF7F4);
                        capsuleconfigWriter.Write(0xFFFF0101);
                        capsuleconfigWriter.Write(capsuleCount);

                        // Write Capsule Config
                        for (int i = 0; i < capsuleCount; i++)
                        {
                            YW1Support.CapsuleItem capsuleItem = capsuleconfigreader.ReadStruct<YW1Support.CapsuleItem>();
                            capsuleItem.ID = capsules[i];
                            capsuleconfigWriter.WriteStruct<YW1Support.CapsuleItem>(capsuleItem);
                        }

                        // Write end of file
                        capsuleconfigWriter.Write(capsuleconfigreader.GetSection((int)(capsuleconfigreader.Length - capsuleconfigreader.Position)));
                    }

                    // Replace File
                    Game.Directory.GetFolderFromFullPath("/data/res/capsule/").Files["capsule_config.cfg.bin"].ByteContent = memoryStream.ToArray();
                }
            }
        }

        public List<uint> GetShop(string fileName)
        {
            List<uint> shops;

            using (BinaryDataReader shopReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/shop/" + fileName)))
            {
                shopReader.SeekOf<uint>(0x6CFC021A, 0x10);
                shopReader.Skip(0x0C);
                YW1Support.ShopConfig[] shopConfigs = shopReader.ReadMultipleStruct<YW1Support.ShopConfig>(shopReader.ReadValue<int>());

                shops = shopConfigs.Select(shop => shop.ItemID).ToList();
            }

            return shops;
        }

        public void SaveShop(List<uint> capsules, string fileName)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter shopWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader shopReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/shop/" + fileName)))
                    {
                        shopReader.SeekOf<uint>(0x6CFC021A, 0x10);
                        shopReader.Skip(0x0C);
                        shopWriter.Write(shopReader.GetSection(0, (int)shopReader.Position));
                        int shopCount = shopReader.ReadValue<int>();

                        shopWriter.Write(shopCount);

                        // Write Shop Config
                        for (int i = 0; i < shopCount; i++)
                        {
                            YW1Support.ShopConfig shop = shopReader.ReadStruct<YW1Support.ShopConfig>();
                            shop.ItemID = capsules[i];
                            shopWriter.WriteStruct<YW1Support.ShopConfig>(shop);
                        }

                        // Write end of file
                        shopWriter.Write(shopReader.GetSection((int)(shopReader.Length - shopReader.Position)));
                    }

                    // Replace File
                    Game.Directory.GetFolderFromFullPath("/data/res/shop/").Files[fileName].ByteContent = memoryStream.ToArray();
                }
            }
        }

        public List<uint> GetTreasureBox(byte[] file)
        {
            List<uint> treasures;

            using (BinaryDataReader treasureBoxReader = new BinaryDataReader(file))
            {
                treasureBoxReader.SeekOf<uint>(0x890AA6BE, 0x10);
                treasureBoxReader.Skip(0x08);
                YW1Support.TreasureBoxConfig[] treasureConfigs = treasureBoxReader.ReadMultipleStruct<YW1Support.TreasureBoxConfig>(treasureBoxReader.ReadValue<int>());

                treasures = treasureConfigs.Select(treasure => treasure.ItemID).ToList();
            }

            return treasures;
        }

        public byte[] SaveTreasureBox(List<uint> treasures, byte[] file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter treasureWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader treasureReader = new BinaryDataReader(file))
                    {
                        int entryCount = treasureReader.ReadValue<int>();
                        long tableOffset = treasureReader.ReadValue<int>();

                        treasureReader.SeekOf<uint>(0x890AA6BE, 0x10);
                        treasureWriter.Write(treasureReader.GetSection(0, (int)treasureReader.Position));
                        treasureReader.Skip(0x08);
                        int encounterCount = treasureReader.ReadValue<int>();
                        YW1Support.TreasureBoxConfig[] treasureConfigs = treasureReader.ReadMultipleStruct<YW1Support.TreasureBoxConfig>(encounterCount);

                        // Header treasure
                        treasureWriter.Write(0x890AA6BE);
                        treasureWriter.Write(0xFFFF0101);
                        treasureWriter.Write(encounterCount);

                        // Write treasure
                        for (int i = 0; i < treasures.Count; i++)
                        {
                            YW1Support.TreasureBoxConfig treasure = treasureConfigs[i];
                            treasure.ItemID = treasures[i];
                            treasureWriter.WriteStruct(treasure);
                        }

                        // Write end of file
                        treasureWriter.Write(treasureReader.GetSection((int)(treasureReader.Length - treasureReader.Position)));
                    }

                    return memoryStream.ToArray();
                }
            }
        }

        public void FixStory()
        {
            // Fusion Quest Mochismo
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter mochismoMapWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader mochismoMapReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/map/t103d11/t103d11_enc_0.02m.cfg.bin")))
                    {
                        mochismoMapWriter.Write(mochismoMapReader.GetSection(0, (int)mochismoMapReader.Length));
                        mochismoMapWriter.Seek(0x528);
                        mochismoMapWriter.Write(0x86056C74);
                        mochismoMapWriter.Seek(0x578);
                        mochismoMapWriter.Write(0x86056C74);
                        mochismoMapWriter.Seek(0x5B8);
                        mochismoMapWriter.Write(0x86056C74);
                        mochismoMapWriter.Seek(0x648);
                        mochismoMapWriter.Write(0x86056C74);
                    }
                }
            }

            // Fusion Quest Dulluma
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryDataWriter dullumaMapWriter = new BinaryDataWriter(memoryStream))
                {
                    using (BinaryDataReader dullumaMapReader = new BinaryDataReader(Game.Directory.GetFileFromFullPath("/data/res/map/t101d03/t101d03_enc_0.02m.cfg.bin")))
                    {
                        dullumaMapWriter.Write(dullumaMapReader.GetSection(0, (int)dullumaMapReader.Length));
                        dullumaMapWriter.Seek(0x2BC);
                        dullumaMapWriter.Write(0xAEACEBD9);
                        dullumaMapWriter.Seek(0x328);
                        dullumaMapWriter.Write(0xAEACEBD9);
                        dullumaMapWriter.Seek(0x394);
                        dullumaMapWriter.Write(0xAEACEBD9);
                        dullumaMapWriter.Seek(0x3B8);
                        dullumaMapWriter.Write(0xAEACEBD9);
                    }
                }
            }
        }

        public void FixYokai(List<Yokai> yokais)
        {
            if (yokais.Any(x => x.ParamID == 0x8443D22D))
            {
                Yokai buhu = yokais.Find(x => x.ParamID == 0x8443D22D);
                if (buhu.Rank > 0x00)
                {
                    buhu.Rank = 0x00;
                }
            }

            if (yokais.Any(x => x.ParamID == 0xF6913D61))
            {
                Yokai mochismo = yokais.Find(x => x.ParamID == 0xF6913D61);
                if (mochismo.Rank > 0x01)
                {
                    mochismo.Rank = 0x01;
                }
            }

            if (yokais.Any(x => x.ParamID == 0xAEACEBD9))
            {
                Yokai dulluma = yokais.Find(x => x.ParamID == 0xAEACEBD9);
                if (dulluma.Rank > 0x00)
                {
                    dulluma.Rank = 0x00;
                }
            }
        }
    }
}
