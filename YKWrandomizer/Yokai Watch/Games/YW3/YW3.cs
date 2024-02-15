using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Text;
using YKWrandomizer.Level5.Archive.ARC0;
using YKWrandomizer.Level5.Archive.XPCK;
using YKWrandomizer.Level5.Binary;
using YKWrandomizer.Level5.Binary.Logic;
using YKWrandomizer.Yokai_Watch.Games.YW3.Logic;
using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3
{
    public class YW3 : IGame
    {
        public string Name => "Yo-Kai Watch 3";

        public Dictionary<uint, string> Attacks => Common.Attacks.YW3;

        public Dictionary<uint, string> Techniques => Common.Techniques.YW3;

        public Dictionary<uint, string> Inspirits => Common.Inspirits.YW3;

        public Dictionary<uint, string> Soultimates => Common.Soultimates.YW3;

        public Dictionary<uint, string> Skills => Common.Skills.YW3;

        public Dictionary<int, string> Tribes => Common.Tribes.YW3;

        public Dictionary<int, string> FoodsType => Common.FoodsType.YW3;

        public Dictionary<int, string> ScoutablesType => Common.ScoutablesType.YW3;

        public Dictionary<int, int> BossBattles => Common.Battles.BossBattles.YW1;

        public ARC0 Game { get; set; }

        public ARC0 Language { get; set; }

        public string LanguageCode { get; set; }

        private string RomfsPath;

        public Dictionary<string, GameFile> Files { get; set; }

        public YW3(string romfsPath, string language)
        {
            RomfsPath = romfsPath;
            LanguageCode = language;

            Game = new ARC0(new FileStream(RomfsPath + @"\yw_a.fa", FileMode.Open));
            Language = new ARC0(new FileStream(RomfsPath + @"\yw_lg_" + LanguageCode + ".fa", FileMode.Open));

            Files = new Dictionary<string, GameFile>
            {
                { "chara_text", new GameFile(Language, "/data/res/text/chara_text_" + LanguageCode + ".cfg.bin") },
                { "chara_desc_text", new GameFile(Language, "/data/res/text/chara_desc_text_" + LanguageCode + ".cfg.bin") },
                { "item_text", new GameFile(Language, "/data/res/text/item_text_" + LanguageCode + ".cfg.bin") },
                { "battle_text", new GameFile(Language, "/data/res/text/battle_text_" + LanguageCode + ".cfg.bin") },
                { "skill_text", new GameFile(Language, "/data/res/text/skill_text_" + LanguageCode + ".cfg.bin") },
                { "hackslash_technic_text", new GameFile(Language, "/data/res/text/hackslash/hackslash_technic_text_" + LanguageCode + ".cfg.bin") },
                { "hackslash_chara_ability_text", new GameFile(Language, "/data/res/text/hackslash/hackslash_chara_ability_text_" + LanguageCode + ".cfg.bin") },
                { "chara_ability_text", new GameFile(Language, "/data/res/text/chara_ability_text_" + LanguageCode + ".cfg.bin") },
                { "addmembermenu_text", new GameFile(Language, "/data/res/text/menu/addmembermenu_text_" + LanguageCode + ".cfg.bin") },
                { "face_icon", new GameFile(Game, "/data/menu/face_icon") },
                { "item_icon", new GameFile(Game, "/data/menu/item_icon") },
                { "model", new GameFile(Game, "/data/character") },
                { "shop", new GameFile(Game, "/data/res/shop") },
                { "map_encounter", new GameFile(Game, "/data/res/map") },
            };
        }

        public void Save()
        {
            string tempPath = @"./temp";

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            // Save
            Game.Save(tempPath + @"\yw_a.fa");
            Language.Save(tempPath + @"\yw_lg_" + LanguageCode + ".fa");

            // Close File
            Game.Close();
            Language.Close();

            // Move
            string[] sourceFiles = new string[2] { @"./temp/yw_a.fa", @"./temp/yw_lg_" + LanguageCode + ".fa" };
            string[] destinationFiles = new string[2] { RomfsPath + @"\yw_a.fa", RomfsPath + @"\yw_lg_" + LanguageCode + ".fa" };

            for (int i = 0; i < 2; i++)
            {
                if (File.Exists(destinationFiles[i]))
                {
                    File.Delete(destinationFiles[i]);
                }

                File.Move(sourceFiles[i], destinationFiles[i]);
            }

            // Re Open
            Game = new ARC0(new FileStream(RomfsPath + @"\yw_a.fa", FileMode.Open));
            Language = new ARC0(new FileStream(RomfsPath + @"\yw_lg_" + LanguageCode + ".fa", FileMode.Open));
        }

        public ICharabase[] GetCharacterbase(bool isYokai)
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastcharabase = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_base")).OrderByDescending(x => x).First();

            CfgBin charaBaseFile = new CfgBin();
            charaBaseFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastcharabase));

            if (isYokai)
            {
                return charaBaseFile.Entries
                .Where(x => x.GetName() == "CHARA_BASE_YOKAI_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<YokaiCharabase>())
                .ToArray();
            }
            else
            {
                return charaBaseFile.Entries
                    .Where(x => x.GetName() == "CHARA_BASE_INFO_BEGIN")
                    .SelectMany(x => x.Children)
                    .Select(x => x.ToClass<NPCCharabase>())
                    .ToArray();
            }
        }

        public void SaveCharaBase(ICharabase[] charabases)
        {
            NPCCharabase[] npcCharabases = charabases.OfType<NPCCharabase>().ToArray();
            YokaiCharabase[] yokaiCharabases = charabases.OfType<YokaiCharabase>().ToArray();

            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastcharabase = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_base")).OrderByDescending(x => x).First();

            CfgBin charaBaseFile = new CfgBin();
            charaBaseFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastcharabase));

            charaBaseFile.ReplaceEntry("CHARA_BASE_INFO_BEGIN", "CHARA_BASE_INFO_", npcCharabases);
            charaBaseFile.ReplaceEntry("CHARA_BASE_YOKAI_INFO_BEGIN", "CHARA_BASE_YOKAI_INFO_", yokaiCharabases);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files[lastcharabase].ByteContent = charaBaseFile.Save();
        }

        public ICharascale[] GetCharascale()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastCharascale = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_scale")).OrderByDescending(x => x).First();

            CfgBin charaScaleFile = new CfgBin();
            charaScaleFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastCharascale));

            return charaScaleFile.Entries
                .Where(x => x.GetName() == "CHARA_SCALE_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<Charascale>())
                .ToArray();
        }

        public void SaveCharascale(ICharascale[] charascales)
        {
            Charascale[] formatCharascales = charascales.OfType<Charascale>().ToArray();

            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastCharascale = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_scale")).OrderByDescending(x => x).First();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastCharascale));

            charaparamFile.ReplaceEntry("CHARA_SCALE_INFO_LIST_BEG", "CHARA_SCALE_INFO_", formatCharascales);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files[lastCharascale].ByteContent = charaparamFile.Save();
        }

        public ICharaparam[] GetCharaparam()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastCharaparam = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_param")).OrderByDescending(x => x).First();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastCharaparam));

            List<Charaparam> charaparams = charaparamFile.Entries
                .Where(x => x.GetName() == "CHARA_PARAM_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<Charaparam>())
                .ToList();


            // Remove blasters t mini boss
            foreach (int paramHash in Common.MiniBossBlastersT.YW3)
            {
                Charaparam charaparam = charaparams.FirstOrDefault(x => x.ParamHash == paramHash);

                if (charaparam != null)
                {
                    charaparams.Remove(charaparam);
                }
            }

            return charaparams.ToArray();
        }

        public void SaveCharaparam(ICharaparam[] charaparams)
        {
            Charaparam[] formatCharaparams = charaparams.OfType<Charaparam>().ToArray();

            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastCharaparam = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_param")).OrderByDescending(x => x).First();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastCharaparam));

            charaparamFile.ReplaceEntry("CHARA_PARAM_INFO_LIST_BEG", "CHARA_PARAM_INFO_", formatCharaparams);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files[lastCharaparam].ByteContent = charaparamFile.Save();
        }

        public ICharaevolve[] GetCharaevolution()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastCharaparam = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_param")).OrderByDescending(x => x).First();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastCharaparam));

            return charaparamFile.Entries
                .Where(x => x.GetName() == "CHARA_EVOLVE_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<Charaevolve>())
                .ToArray();
        }

        public void SaveCharaevolution(ICharaevolve[] charaevolutions)
        {
            Charaevolve[] formatCharaevolutions = charaevolutions.OfType<Charaevolve>().ToArray();

            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastCharaparam = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_param")).OrderByDescending(x => x).First();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastCharaparam));

            charaparamFile.ReplaceEntry("CHARA_EVOLVE_INFO_LIST_BEG", "CHARA_EVOLVE_INFO_", formatCharaevolutions);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files[lastCharaparam].ByteContent = charaparamFile.Save();
        }

        public IItem[] GetItems(string itemType)
        {
            VirtualDirectory itemFolder = Game.Directory.GetFolderFromFullPath("data/res/item");
            string lastItemconfig = itemFolder.Files.Keys.Where(x => x.StartsWith("item_config")).OrderByDescending(x => x).First();

            CfgBin itemconfigFile = new CfgBin();
            itemconfigFile.Open(Game.Directory.GetFileFromFullPath("data/res/item/" + lastItemconfig));

            switch (itemType)
            {
                case "all":
                    string[] itemTypes = { "ITEM_CONSUME_LIST_BEG", "ITEM_HACKSLASH_BATTLE_LIST_BEG", "ITEM_SOUL_LIST_BEG", "ITEM_EQUIPMENT_LIST_BEG" };

                    return itemconfigFile.Entries
                    .Where(x =>
                    {
                        if (itemTypes.Contains(x.GetName()))
                        {
                            itemType = x.GetName();
                            return true;
                        }

                        return false;
                    })
                    .SelectMany(x => x.Children)
                        .Where(x =>
                        {
                            if (itemType == "ITEM_SOUL_LIST_BEG")
                            {
                                return x.GetName() == "ITEM_SOUL";
                            } else if (itemType == "ITEM_EQUIPMENT_LIST_BEG")
                            {
                                return x.GetName() == "ITEM_EQUIPMENT";
                            }

                            return true;
                        })
                        .Select(x => x.ToClass<Item>())
                    .ToArray();
                default:
                    return new IItem[] { };
            }
        }

        public ICharaabilityConfig[] GetAbilities()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/skill");
            string lastskillFile = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_ability")).OrderByDescending(x => x).First();

            CfgBin charaabilityConfig = new CfgBin();
            charaabilityConfig.Open(Game.Directory.GetFileFromFullPath("/data/res/skill/" + lastskillFile));

            return charaabilityConfig.Entries
                .Where(x => x.GetName() == "CHARA_ABILITY_CONFIG_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                    .Where(x => x.GetName() == "CHARA_ABILITY_CONFIG_INFO")
                    .Select(x => x.ToClass<CharaabilityConfig>())
                .ToArray();
        }

        public ISkillconfig[] GetSkills()
        {
            VirtualDirectory skillFolder = Game.Directory.GetFolderFromFullPath("data/res/skill");
            string lastCharaparam = skillFolder.Files.Keys.Where(x => x.StartsWith("skill_config")).OrderByDescending(x => x).First();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/skill/" + lastCharaparam));

            return charaparamFile.Entries
                .Where(x => x.GetName() == "SKILL_CONFIG_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<Skillconfig>())
                .ToArray();
        }

        public IBattleCharaparam[] GetBattleCharaparam()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastBattleCharaparam = characterFolder.Files.Keys.Where(x => x.StartsWith("battle_chara_param")).OrderByDescending(x => x).First();

            CfgBin battleCharaparamFile = new CfgBin();
            battleCharaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastBattleCharaparam));

            return battleCharaparamFile.Entries
                .Where(x => x.GetName() == "BATTLE_CHARA_PARAM_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<BattleCharaparam>())
                .ToArray();
        }

        public void SaveBattleCharaparam(IBattleCharaparam[] battleCharaparams)
        {
            BattleCharaparam[] formatBattleCharaparam = battleCharaparams.OfType<BattleCharaparam>().ToArray();

            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastBattleCharaparam = characterFolder.Files.Keys.Where(x => x.StartsWith("battle_chara_param")).OrderByDescending(x => x).First();

            CfgBin battleCharaparamFile = new CfgBin();
            battleCharaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastBattleCharaparam));

            battleCharaparamFile.ReplaceEntry("BATTLE_CHARA_PARAM_INFO_LIST_BEG", "BATTLE_CHARA_PARAM_INFO_", formatBattleCharaparam);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files[lastBattleCharaparam].ByteContent = battleCharaparamFile.Save();
        }

        public IHackslashCharaparam[] GetHackslashCharaparam()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastHackslashCharaparam = characterFolder.Files.Keys.Where(x => x.StartsWith("hackslash_chara_param")).OrderByDescending(x => x).First();

            CfgBin hackslashCharaparamFile = new CfgBin();
            hackslashCharaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastHackslashCharaparam));

            return hackslashCharaparamFile.Entries
                .Where(x => x.GetName() == "HACKSLASH_CHARA_PARAM_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<HackslashCharaparam>())
                .ToArray();
        }

        public void SaveHackslashCharaparam(IHackslashCharaparam[] hackslashCharaparams)
        {
            HackslashCharaparam[] formatHackslashCharaparams = hackslashCharaparams.OfType<HackslashCharaparam>().ToArray();

            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastHackslashCharaparam = characterFolder.Files.Keys.Where(x => x.StartsWith("hackslash_chara_param")).OrderByDescending(x => x).First();

            CfgBin hackslashCharaparamFile = new CfgBin();
            hackslashCharaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastHackslashCharaparam));

            hackslashCharaparamFile.ReplaceEntry("HACKSLASH_CHARA_PARAM_INFO_LIST_BEG", "HACKSLASH_CHARA_PARAM_INFO_", formatHackslashCharaparams);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files[lastHackslashCharaparam].ByteContent = hackslashCharaparamFile.Save();
        }

        public IHackslashCharaabilityConfig[] GetHackslashAbilities()
        {
            VirtualDirectory hackslashFolder = Game.Directory.GetFolderFromFullPath("data/res/hackslash");
            string lastHackslashCharaability = hackslashFolder.Files.Keys.Where(x => x.StartsWith("hackslash_chara_ability")).OrderByDescending(x => x).First();

            CfgBin hackslashCharaabilityFile = new CfgBin();
            hackslashCharaabilityFile.Open(Game.Directory.GetFileFromFullPath("/data/res/hackslash/" + lastHackslashCharaability));

            return hackslashCharaabilityFile.Entries
                .Where(x => x.GetName() == "CHARA_ABILITY_CONFIG_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<HackslashCharaability>())
                .ToArray();
        }

        public IHackslashTechnic[] GetHackslashSkills()
        {
            VirtualDirectory hackslashFolder = Game.Directory.GetFolderFromFullPath("data/res/hackslash");
            string lastHackslashTechnic = hackslashFolder.Files.Keys.Where(x => x.StartsWith("hackslash_technic") && x.Contains("menu") == false).OrderByDescending(x => x).First();

            CfgBin hackslashTechnicFile = new CfgBin();
            hackslashTechnicFile.Open(Game.Directory.GetFileFromFullPath("/data/res/hackslash/" + lastHackslashTechnic));

            return hackslashTechnicFile.Entries
                .Where(x => x.GetName() == "HACKSLASH_TECHNIC_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<HackslashTechnic>())
                .ToArray();
        }

        public IOrgetimeTechnic[] GetOrgetimeTechnics()
        {
            return new IOrgetimeTechnic[0];
        }

        public IBattleCommand[] GetBattleCommands()
        {
            List<IBattleCommand> ouput = new List<IBattleCommand>();

            ouput.AddRange(GetBattleCommandsOnly());
            ouput.AddRange(GetSkillBattleCommands());

            return ouput.ToArray();
        }

        private IBattleCommand[] GetBattleCommandsOnly()
        {
            VirtualDirectory battleFolder = Game.Directory.GetFolderFromFullPath("data/res/battle");
            string lastBattleCommand = battleFolder.Files.Keys.Where(x => x.StartsWith("battle_command") && x.Contains("link") == false).OrderByDescending(x => x).First();

            CfgBin battlecommandConfig = new CfgBin();
            battlecommandConfig.Open(Game.Directory.GetFileFromFullPath("/data/res/battle/" + lastBattleCommand));

            return battlecommandConfig.Entries
                .Where(x => x.GetName() == "BATTLE_COMMAND_INFO_BEGIN")
                .SelectMany(x => x.Children)
                    .Where(x => x.GetName() == "BATTLE_COMMAND_INFO")
                    .Select(x => x.ToClass<Battlecommand>())
                .ToArray();
        }

        private IBattleCommand[] GetSkillBattleCommands()
        {
            VirtualDirectory skillBattleFolder = Game.Directory.GetFolderFromFullPath("data/res/skill");
            string lastSkillBattleCommand = skillBattleFolder.Files.Keys.Where(x => x.StartsWith("skill_btl_config")).OrderByDescending(x => x).First();

            CfgBin skillBattlecommandConfig = new CfgBin();
            skillBattlecommandConfig.Open(Game.Directory.GetFileFromFullPath("/data/res/skill/" + lastSkillBattleCommand));

            return skillBattlecommandConfig.Entries
                .Where(x => x.GetName() == "SKILL_CONFIG_BTL_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                    .Where(x => x.GetName() == "SKILL_CONFIG_BTL_INFO")
                    .Select(x => x.ToClass<SkillBattleConfig>())
                .ToArray();
        }

        public string[] GetMapWhoContainsEncounter()
        {
            VirtualDirectory mapEncounterFolder = Game.Directory.GetFolderFromFullPath("/data/res/map");

            return mapEncounterFolder.Folders
                .Where(folder =>
                {
                    SubMemoryStream pckFile = folder.Files.FirstOrDefault(x => x.Key == folder.Name + ".pck").Value;

                    if (pckFile != null)
                    {
                        if (pckFile.ByteContent == null)
                        {
                            pckFile.Read();
                        }

                        XPCK mapArchive = new XPCK(pckFile.ByteContent);

                        if (mapArchive.Directory.Files.Any(file => file.Key.StartsWith(folder.Name + "_enc_") && !file.Key.Contains("_enc_pos")))
                        {
                            return true;
                        }
                    }

                    return false;
                })
                .Select(folder => folder.Name)
                .ToArray();
        }

        public (IEncountTable[], IEncountChara[]) GetMapEncounter(string mapName)
        {
            VirtualDirectory mapFolder = Game.Directory.GetFolderFromFullPath(Files["map_encounter"].Path);
            XPCK mapArchive = new XPCK(mapFolder.GetFolder(mapName).Files[mapName + ".pck"].ByteContent);
            string lastEncountConfigFile = mapArchive.Directory.Files.Keys.Where(x => x.StartsWith(mapName + "_enc_") && !x.Contains("_enc_pos")).OrderByDescending(x => x).First();

            CfgBin encountConfig = new CfgBin();

            if (mapArchive.Directory.Files[lastEncountConfigFile].ByteContent == null)
            {
                mapArchive.Directory.Files[lastEncountConfigFile].Read();
            }

            encountConfig.Open(mapArchive.Directory.Files[lastEncountConfigFile].ByteContent);

            IEncountTable[] encountTable = encountConfig.Entries
                .Where(x => x.GetName() == "ENCOUNT_TABLE_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass2<EncountTable>())
                .ToArray();

            IEncountChara[] encountChara = encountConfig.Entries
                .Where(x => x.GetName() == "ENCOUNT_CHARA_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass2<EncountChara>())
                .ToArray();

            return (encountTable, encountChara);
        }

        public void SaveMapEncounter(string mapName, IEncountTable[] encountTables, IEncountChara[] encountCharas)
        {
            EncountTable[] formatEncountTables = encountTables.OfType<EncountTable>().ToArray();
            EncountChara[] formatEncountCharas = encountCharas.OfType<EncountChara>().ToArray();

            VirtualDirectory mapFolder = Game.Directory.GetFolderFromFullPath(Files["map_encounter"].Path);
            XPCK mapArchive = new XPCK(mapFolder.GetFolder(mapName).Files[mapName + ".pck"].ByteContent);
            string lastEncountConfigFile = mapArchive.Directory.Files.Keys.Where(x => x.StartsWith(mapName + "_enc_") && !x.Contains("_enc_pos")).OrderByDescending(x => x).First();

            CfgBin encountConfig = new CfgBin();
            mapArchive.Directory.Files[lastEncountConfigFile].Read();
            encountConfig.Open(mapArchive.Directory.Files[lastEncountConfigFile].ByteContent);

            encountConfig.ReplaceEntry("ENCOUNT_TABLE_BEGIN", "ENCOUNT_TABLE_", formatEncountTables);
            encountConfig.ReplaceEntry("ENCOUNT_CHARA_BEGIN", "ENCOUNT_CHARA_", formatEncountCharas);

            mapArchive.Directory.Files[lastEncountConfigFile].ByteContent = encountConfig.Save();
            mapFolder.GetFolder(mapName).Files[mapName + ".pck"].ByteContent = mapArchive.Save();
        }

        public ICombineConfig[] GetFusions()
        {
            VirtualDirectory shopFolder = Game.Directory.GetFolderFromFullPath("data/res/shop");
            string lastCombineConfig = shopFolder.Files.Keys.Where(x => x.StartsWith("combine_config")).OrderByDescending(x => x).First();

            CfgBin combineConfigFile = new CfgBin();
            combineConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/shop/" + lastCombineConfig));

            return combineConfigFile.Entries
                .Where(x => x.GetName() == "COMBINE_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<CombineConfig>())
                .ToArray();
        }

        public void SaveFusions(ICombineConfig[] combineConfigs)
        {
            CombineConfig[] formatCombineConfigs = combineConfigs.OfType<CombineConfig>().ToArray();

            VirtualDirectory shopFolder = Game.Directory.GetFolderFromFullPath("data/res/shop");
            string lastCombineConfig = shopFolder.Files.Keys.Where(x => x.StartsWith("combine_config")).OrderByDescending(x => x).First();

            CfgBin combineConfigFile = new CfgBin();
            combineConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/shop/" + lastCombineConfig));

            combineConfigFile.ReplaceEntry("COMBINE_INFO_LIST_BEG", "COMBINE_INFO_", formatCombineConfigs);

            Game.Directory.GetFolderFromFullPath("/data/res/shop").Files[lastCombineConfig].ByteContent = combineConfigFile.Save();
        }

        public (IEncountTable[], IEncountChara[]) GetStaticEncounters()
        {
            VirtualDirectory battleFolder = Game.Directory.GetFolderFromFullPath("data/res/battle");
            string lastEncounter = battleFolder.Files.Keys.Where(x => x.StartsWith("common_enc")).OrderByDescending(x => x).First();

            CfgBin encountConfig = new CfgBin();
            encountConfig.Open(Game.Directory.GetFileFromFullPath("/data/res/battle/" + lastEncounter));

            IEncountTable[] encountTable = encountConfig.Entries
                .Where(x => x.GetName() == "ENCOUNT_TABLE_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass2<EncountTable>())
                .ToArray();

            IEncountChara[] encountChara = encountConfig.Entries
                .Where(x => x.GetName() == "ENCOUNT_CHARA_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass2<EncountChara>())
                .ToArray();

            return (encountTable, encountChara);
        }

        public void SaveStaticEncounters(IEncountTable[] encountTables, IEncountChara[] encountCharas)
        {
            EncountTable[] formatEncountTables = encountTables.OfType<EncountTable>().ToArray();
            EncountChara[] formatEncountCharas = encountCharas.OfType<EncountChara>().ToArray();

            VirtualDirectory battleFolder = Game.Directory.GetFolderFromFullPath("data/res/battle");
            string lastEncounter = battleFolder.Files.Keys.Where(x => x.StartsWith("common_enc")).OrderByDescending(x => x).First();

            CfgBin encountConfig = new CfgBin();
            encountConfig.Open(Game.Directory.GetFileFromFullPath("/data/res/battle/" + lastEncounter));

            encountConfig.ReplaceEntry("ENCOUNT_TABLE_BEGIN", "ENCOUNT_TABLE_INFO_", formatEncountTables);
            encountConfig.ReplaceEntry("ENCOUNT_CHARA_BEGIN", "ENCOUNT_CHARA_INFO_", formatEncountCharas);

            Game.Directory.GetFolderFromFullPath("/data/res/battle").Files[lastEncounter].ByteContent = encountConfig.Save();
        }

        public (IShopConfig[], IShopValidCondition[]) GetShop(string shopName)
        {
            VirtualDirectory shopFolder = Game.Directory.GetFolderFromFullPath("data/res/shop");
            string lastShopFile = shopFolder.Files.Keys.Where(x => x.StartsWith("shop_") && x.Contains(shopName)).OrderByDescending(x => x).First();

            CfgBin shopCfgBin = new CfgBin();
            shopCfgBin.Open(Game.Directory.GetFileFromFullPath("/data/res/shop/" + lastShopFile));

            IShopConfig[] shopConfig = shopCfgBin.Entries
                .Where(x => x.GetName() == "SHOP_CONFIG_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass2<ShopConfig>())
                .ToArray();

            IShopValidCondition[] validConfig = shopCfgBin.Entries
                .Where(x => x.GetName() == "SHOP_VALID_CONDITION_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass2<ShopValidCondition>())
                .ToArray();

            return (shopConfig, validConfig);
        }

        public void SaveShop(string shopName, IShopConfig[] shopConfigs, IShopValidCondition[] shopValidConditions)
        {
            ShopConfig[] formatShopConfigs = shopConfigs.OfType<ShopConfig>().ToArray();
            ShopValidCondition[] formatShopValidCondition = null;

            if (shopValidConditions != null)
            {
                formatShopValidCondition = shopValidConditions.OfType<ShopValidCondition>().ToArray();
            }

            VirtualDirectory shopFolder = Game.Directory.GetFolderFromFullPath("data/res/shop");
            string lastShopFile = shopFolder.Files.Keys.Where(x => x.StartsWith("shop_") && x.Contains(shopName)).OrderByDescending(x => x).First();

            CfgBin shopCfgBin = new CfgBin();
            shopCfgBin.Open(Game.Directory.GetFileFromFullPath("/data/res/shop/" + lastShopFile));

            shopCfgBin.ReplaceEntry("SHOP_CONFIG_INFO_BEGIN", "SHOP_CONFIG_INFO_", formatShopConfigs);

            if (shopValidConditions != null)
            {
                shopCfgBin.ReplaceEntry("SHOP_VALID_CONDITION_BEGIN", "SHOP_VALID_CONDITION_", shopValidConditions);
            }

            Game.Directory.GetFolderFromFullPath("/data/res/shop/").Files[lastShopFile].ByteContent = shopCfgBin.Save();
        }

        public string[] GetMapWhoContainsTreasureBoxes()
        {
            VirtualDirectory mapEncounterFolder = Game.Directory.GetFolderFromFullPath("/data/res/map");

            return mapEncounterFolder.Folders
                .Where(folder =>
                {
                    SubMemoryStream pckFile = folder.Files.FirstOrDefault(x => x.Key == folder.Name + ".pck").Value;

                    if (pckFile != null)
                    {
                        if (pckFile.ByteContent == null)
                        {
                            pckFile.Read();
                        }

                        XPCK mapArchive = new XPCK(pckFile.ByteContent);

                        if (mapArchive.Directory.Files.Any(file => file.Key.StartsWith(folder.Name + "_tbox")))
                        {
                            return true;
                        }
                    }

                    return false;
                })
                .Select(folder => folder.Name)
                .ToArray();
        }

        public IItableDataMore[] GetTreasureBox(string mapName)
        {
            VirtualDirectory mapFolder = Game.Directory.GetFolderFromFullPath(Files["map_encounter"].Path);
            XPCK mapArchive = new XPCK(mapFolder.GetFolder(mapName).Files[mapName + ".pck"].ByteContent);
            string lastTboxConfigFile = mapArchive.Directory.Files.Keys.Where(x => x.StartsWith(mapName + "_tbox")).OrderByDescending(x => x).First();

            CfgBin tboxConfig = new CfgBin();

            if (mapArchive.Directory.Files[lastTboxConfigFile].ByteContent == null)
            {
                mapArchive.Directory.Files[lastTboxConfigFile].Read();
            }

            tboxConfig.Open(mapArchive.Directory.Files[lastTboxConfigFile].ByteContent);

            return tboxConfig.Entries
                .Where(x => x.GetName() == "REWARD_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                    .Where(x => x.GetName() == "REWARD_INFO_DATA_LIST_BEG")
                    .SelectMany(x => x.Children)
                        .Where(x => x.GetName() == "REWARD_INFO_DATA")
                        .Select(x => x.ToClass2<ItableDataMore>())
                .ToArray();
        }

        public void SaveTreasureBox(string mapName, IItableDataMore[] itableDataMores)
        {
            int index = 0;
            ItableDataMore[] formatItableDataMore = itableDataMores.OfType<ItableDataMore>().ToArray();

            VirtualDirectory mapFolder = Game.Directory.GetFolderFromFullPath(Files["map_encounter"].Path);
            XPCK mapArchive = new XPCK(mapFolder.GetFolder(mapName).Files[mapName + ".pck"].ByteContent);
            string lastTboxConfigFile = mapArchive.Directory.Files.Keys.Where(x => x.StartsWith(mapName + "_tbox")).OrderByDescending(x => x).First();

            CfgBin tboxConfig = new CfgBin();
            mapArchive.Directory.Files[lastTboxConfigFile].Read();
            tboxConfig.Open(mapArchive.Directory.Files[lastTboxConfigFile].ByteContent);

            if (tboxConfig.Entries.Count > 0)
            {
                foreach (Entry child in tboxConfig.Entries[0].Children)
                {
                    if (child.GetName() == "REWARD_INFO_DATA_LIST_BEG")
                    {
                        foreach (Entry subChild in child.Children)
                        {
                            subChild.SetVariablesFromClass(formatItableDataMore[index]);
                            index++;
                        }
                    }
                }
            }

            mapArchive.Directory.Files[lastTboxConfigFile].ByteContent = tboxConfig.Save();
            mapFolder.GetFolder(mapName).Files[mapName + ".pck"].ByteContent = mapArchive.Save();
        }

        public ICapsuleConfig[] GetCapsuleConfigs()
        {
            VirtualDirectory capsuleFolder = Game.Directory.GetFolderFromFullPath("data/res/capsule");
            string lastCapsuleConfig = capsuleFolder.Files.Keys.Where(x => x.StartsWith("capsule_prize_config")).OrderByDescending(x => x).First();

            CfgBin capsuleConfigFile = new CfgBin();
            capsuleConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/capsule/" + lastCapsuleConfig));

            return capsuleConfigFile.Entries
                .Where(x => x.GetName() == "CPSL_PRIZE_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<CapsuleConfig>())
                .ToArray();
        }

        public void SaveCapsuleConfigs(ICapsuleConfig[] capsuleConfigs)
        {
            CapsuleConfig[] formatCapsuleConfigs = capsuleConfigs.OfType<CapsuleConfig>().ToArray();

            VirtualDirectory capsuleFolder = Game.Directory.GetFolderFromFullPath("data/res/capsule");
            string lastCapsuleConfig = capsuleFolder.Files.Keys.Where(x => x.StartsWith("capsule_prize_config")).OrderByDescending(x => x).First();

            CfgBin capsuleConfigFile = new CfgBin();
            capsuleConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/capsule/" + lastCapsuleConfig));

            capsuleConfigFile.ReplaceEntry("CPSL_PRIZE_INFO_LIST_BEG", "CPSL_PRIZE_INFO_", formatCapsuleConfigs);

            Game.Directory.GetFolderFromFullPath("/data/res/capsule").Files[lastCapsuleConfig].ByteContent = capsuleConfigFile.Save();
        }

        public ILegendDataConfig[] GetLegends()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastLegendConfig = characterFolder.Files.Keys.Where(x => x.StartsWith("legend_config")).OrderByDescending(x => x).First();

            CfgBin legendConfigFile = new CfgBin();
            legendConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastLegendConfig));

            return legendConfigFile.Entries
                .Where(x => x.GetName() == "LEGEND_DATA_CONFIG_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<LegendDataConfig>())
                .ToArray();
        }

        public void SaveLegends(ILegendDataConfig[] legendDataConfigs)
        {
            LegendDataConfig[] formatLegendConfigs = legendDataConfigs.OfType<LegendDataConfig>().ToArray();

            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastLegendConfig = characterFolder.Files.Keys.Where(x => x.StartsWith("legend_config")).OrderByDescending(x => x).First();

            CfgBin legendConfigFile = new CfgBin();
            legendConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastLegendConfig));

            legendConfigFile.ReplaceEntry("LEGEND_DATA_CONFIG_LIST_BEG", "LEGEND_DATA_CONFIG_", formatLegendConfigs);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files[lastLegendConfig].ByteContent = legendConfigFile.Save();
        }

        public void UnlockUnscoutableYokai(List<ICharaparam> charaparams, List<ICharabase> charabases, List<ICharascale> charascales, List<IHackslashCharaparam> hackslashCharaparams = null, List<IBattleCharaparam> battleCharaparams = null)
        {
            List<int> unbefriendableYokaiParamHashes = new List<int>()
            {
                unchecked((int)0x7D7ED684),
                unchecked((int)0xF21C41D3),
                unchecked((int)0x0076990E),
                unchecked((int)0xB8CAFE6B),
                unchecked((int)0x52AB50DA),
                unchecked((int)0xEA1737BF),
                unchecked((int)0x77C00F06),
                unchecked((int)0x6575A0E8),
                unchecked((int)0x85AAD7DB),
                unchecked((int)0xBF02E281),
                unchecked((int)0x150B2A0A),
                unchecked((int)0xADB74D6F),
                unchecked((int)0x88DC12B3),
                unchecked((int)0x3ADEAC54),
                unchecked((int)0x286B03BA),
                unchecked((int)0x47D6E3DE),
                unchecked((int)0x079CE713),
                unchecked((int)0xAD952F98),
                unchecked((int)0x4D4A58AB),
                unchecked((int)0xB59E59F4),
                unchecked((int)0xA72BF61A),
                unchecked((int)0x90F50628),
                unchecked((int)0xC896167E),
                unchecked((int)0x702A711B),
                unchecked((int)0x4A824441),
                unchecked((int)0xF23E2324),
                unchecked((int)0x5837EBAF),
                unchecked((int)0x6FE91B9D),
                unchecked((int)0xD7557CF8),
                unchecked((int)0x378A0BCB),
                unchecked((int)0xCF5E0A94),
                unchecked((int)0xDDEBA57A),
                unchecked((int)0xF880FAA6),
                unchecked((int)0x403C9DC3),
                unchecked((int)0x0AEA227B),
                unchecked((int)0x4D0E9D45),
                unchecked((int)0x7AD06D77),
                unchecked((int)0x88BAB5AA),
                unchecked((int)0x706EB4F5),
                unchecked((int)0x62DB1B1B),
                unchecked((int)0xDA677C7E),
                unchecked((int)0xFF0C23A2),
                unchecked((int)0xEDB98C4C),
                unchecked((int)0x7488337D),
                unchecked((int)0x663D9C93),
                unchecked((int)0x86E2EBA0),
                unchecked((int)0x7E36EAFF),
                unchecked((int)0xC68A8D9A),
                unchecked((int)0x6C834511),
                unchecked((int)0xE3E1D246),
                unchecked((int)0xBB82C210),
                unchecked((int)0x3996902F),
                unchecked((int)0x332849AD),
                unchecked((int)0x99218126),
                unchecked((int)0x39B4F2D8),
                unchecked((int)0xCBDE2A05),
                unchecked((int)0x661FFE64),
                unchecked((int)0x51C10E56),
                unchecked((int)0x09A21E00),
                unchecked((int)0x7E148808),
                unchecked((int)0x6CA127E6),
                unchecked((int)0x7326888E),
                unchecked((int)0xCB9AEFEB),
                unchecked((int)0xF6FAC65B),
                unchecked((int)0x9ABFBF04),
                unchecked((int)0x0DD65E3F),
                unchecked((int)0x4A7624EF),
                unchecked((int)0x58C38B01),
                unchecked((int)0x612EDE03),
                unchecked((int)0xD992B966),
                unchecked((int)0x444581DF),
                unchecked((int)0x89AB487B),
                unchecked((int)0x698D03C5),
                unchecked((int)0x9BE7DB18),
                unchecked((int)0x36260F79),
                unchecked((int)0xABF137C0),
                unchecked((int)0xA14FEE42),
                unchecked((int)0xABD35537),
                unchecked((int)0x96B37C87),
                unchecked((int)0x3CBAB40C),
                unchecked((int)0xE6CDF665),
                unchecked((int)0x6E23B836),
                unchecked((int)0xBD1EE51B),
                unchecked((int)0xCE94A93F),
                unchecked((int)0xCA722EB7),
                unchecked((int)0xC0CCF735),
                unchecked((int)0x9F67FB89),
                unchecked((int)0x10056CDE),
                unchecked((int)0x95FB40FC),
                unchecked((int)0x5674D2DE),
                unchecked((int)0xF003D96A),
                unchecked((int)0x3062F43C),
                unchecked((int)0xE387EF89),
                unchecked((int)0x45F0E43D),
                unchecked((int)0x8057DAB3),
                unchecked((int)0x0BA3747D),
                unchecked((int)0xC4B68260),
                unchecked((int)0xB3285090),
                unchecked((int)0x288D1CFF),
                unchecked((int)0x137A2F0B),
            };

            List<int> unbefriendableYokaiBaseHashes = new List<int>()
            {
                unchecked((int)0xC6112648),
                unchecked((int)0xADD74C4F),
                unchecked((int)0xD0A0B80A),
                unchecked((int)0x642C610D),
                unchecked((int)0x666ADF54),
                unchecked((int)0x67A8B563),
                unchecked((int)0x9B121E4F),
                unchecked((int)0x82092F0E),
                unchecked((int)0x8865DB96),
                unchecked((int)0xBFBB2BA4),
                unchecked((int)0xC0753E10),
                unchecked((int)0x56DEB51C),
                unchecked((int)0x0D239B68),
                unchecked((int)0x1438AA29),
                unchecked((int)0x3F15F9EA),
                unchecked((int)0x8BF32C28),
                unchecked((int)0x89B59271),
                unchecked((int)0x2A865716),
                unchecked((int)0xDAD67CC7),
                unchecked((int)0x960AB5A2),
                unchecked((int)0x855F128C),
                unchecked((int)0x0141366E),
                unchecked((int)0x13F49980),
                unchecked((int)0xAB48FEE5),
                unchecked((int)0x369FC65C),
                unchecked((int)0x8E23A139),
                unchecked((int)0x9C960ED7),

                unchecked((int)0x72D38212),
                unchecked((int)0x60662DFC),
                unchecked((int)0xD8DA4A99),
                unchecked((int)0xC1C17BD8),
                unchecked((int)0xEAEC281B),
                unchecked((int)0x4FB3ABA2),
                unchecked((int)0x0813D172),
                unchecked((int)0x84EF7807),
                unchecked((int)0x9DF44946),
                unchecked((int)0xC34F02D7),
                unchecked((int)0x36CFA417),
                unchecked((int)0x0BAF8DA7),
                unchecked((int)0x89FF1A76),
                unchecked((int)0x3F1B2319),
                unchecked((int)0x26001258),
                unchecked((int)0x027B0AA9),
                unchecked((int)0x1B603BE8),
                unchecked((int)0x10CEA547),
                unchecked((int)0x09D59406),
                unchecked((int)0xA872C222),
                unchecked((int)0xB169F363),
                unchecked((int)0x45DB7079),
                unchecked((int)0xCFEC1BE7),
                unchecked((int)0xB3E78A6C),
                unchecked((int)0xAAFCBB2D),
                unchecked((int)0x18DC673D),
                unchecked((int)0x2AEA05BF),
                unchecked((int)0x93829364),
                unchecked((int)0xAEE2BAD4),
                unchecked((int)0x1CC266C4),
                unchecked((int)0x5B621C14),
                unchecked((int)0x49D7B3FA),
                unchecked((int)0xA48A610F),
                unchecked((int)0x99EA48BF),
                unchecked((int)0xAE34B88D),
                unchecked((int)0xDE4A326F),
                unchecked((int)0xE32A1BDF),
                unchecked((int)0xF19FB431),
                unchecked((int)0x510AC7CF),
                unchecked((int)0x43BF6821),
                unchecked((int)0x7EDF4191),
                unchecked((int)0x67C470D0),
                unchecked((int)0x4CE92313),
                unchecked((int)0x2BCA94AF),
                unchecked((int)0x16AABD1F),
                unchecked((int)0x0FB18C5E),
                unchecked((int)0x94FA2ACE),
                unchecked((int)0x864F8520),
                unchecked((int)0xA99A037E),
                unchecked((int)0x6FD6B2AA),
                unchecked((int)0x52B69B1A),
                unchecked((int)0xDDD40C4D),
                unchecked((int)0x1516E1CA),
                unchecked((int)0x2876C87A),
                unchecked((int)0xD9328BC5),
                unchecked((int)0x9A56146A),
                unchecked((int)0x88E3BB84),
                unchecked((int)0x305FDCE1),
                unchecked((int)0x834D252B),
                unchecked((int)0x91F88AC5),
                unchecked((int)0x2944EDA0),
                unchecked((int)0xA7363DDA),
                unchecked((int)0xE096470A),
                unchecked((int)0xDDF66EBA),
                unchecked((int)0x5FA6F96B),
                unchecked((int)0x62C6D0DB),
                unchecked((int)0x9E80007F),
                unchecked((int)0xE452A275),
                unchecked((int)0xFD499334),
                unchecked((int)0x05D95785),
                unchecked((int)0xEAFE9CD1),
                unchecked((int)0xAD5EE601),
                unchecked((int)0x2F0E71D0),
                unchecked((int)0x36154091),
                unchecked((int)0x3DBBDE3E),
                unchecked((int)0x24A0EF7F),
                unchecked((int)0xA3E029CF),
                unchecked((int)0xB1558621),
                unchecked((int)0xE3581B63),
                unchecked((int)0xE29A7154),
                unchecked((int)0xE0DCCF0D),
                unchecked((int)0xE11EA53A),
                unchecked((int)0xE451B3BF),
                unchecked((int)0xCF2A3EFE),
                unchecked((int)0x9DF9AC47),
            };

            List<int> yoBossBaseHashes = new List<int>()
            {
                unchecked((int)0x72D38212),
                unchecked((int)0x60662DFC),
                unchecked((int)0xD8DA4A99),
                unchecked((int)0xC1C17BD8),
                unchecked((int)0xEAEC281B),
                unchecked((int)0x4FB3ABA2),
                unchecked((int)0x0813D172),
                unchecked((int)0x84EF7807),
                unchecked((int)0x9DF44946),
                unchecked((int)0xC34F02D7),
                unchecked((int)0x36CFA417),
                unchecked((int)0x0BAF8DA7),
                unchecked((int)0x89FF1A76),
                unchecked((int)0x3F1B2319),
                unchecked((int)0x26001258),
                unchecked((int)0x027B0AA9),
                unchecked((int)0x1B603BE8),
                unchecked((int)0x10CEA547),
                unchecked((int)0x09D59406),
                unchecked((int)0xA872C222),
                unchecked((int)0xB169F363),
                unchecked((int)0x45DB7079),
                unchecked((int)0xCFEC1BE7),
                unchecked((int)0xB3E78A6C),
                unchecked((int)0xAAFCBB2D),
                unchecked((int)0x18DC673D),
                unchecked((int)0x2AEA05BF),
                unchecked((int)0x93829364),
                unchecked((int)0xAEE2BAD4),
                unchecked((int)0x1CC266C4),
                unchecked((int)0x5B621C14),
                unchecked((int)0x49D7B3FA),
                unchecked((int)0xA48A610F),
                unchecked((int)0x99EA48BF),
                unchecked((int)0xAE34B88D),
                unchecked((int)0xDE4A326F),
                unchecked((int)0xE32A1BDF),
                unchecked((int)0xF19FB431),
                unchecked((int)0x510AC7CF),
                unchecked((int)0x43BF6821),
                unchecked((int)0x7EDF4191),
                unchecked((int)0x67C470D0),
                unchecked((int)0x4CE92313),
                unchecked((int)0x2BCA94AF),
                unchecked((int)0x16AABD1F),
                unchecked((int)0x0FB18C5E),
                unchecked((int)0x94FA2ACE),
                unchecked((int)0x864F8520),
                unchecked((int)0xA99A037E),
                unchecked((int)0x6FD6B2AA),
                unchecked((int)0x52B69B1A),
                unchecked((int)0xDDD40C4D),
                unchecked((int)0x1516E1CA),
                unchecked((int)0x2876C87A),
                unchecked((int)0xD9328BC5),
                unchecked((int)0x9A56146A),
                unchecked((int)0x88E3BB84),
                unchecked((int)0x305FDCE1),
                unchecked((int)0x834D252B),
                unchecked((int)0x91F88AC5),
                unchecked((int)0x2944EDA0),
                unchecked((int)0xA7363DDA),
                unchecked((int)0xE096470A),
                unchecked((int)0xDDF66EBA),
                unchecked((int)0x5FA6F96B),
                unchecked((int)0x62C6D0DB),
                unchecked((int)0x9E80007F),
                unchecked((int)0xE452A275),
                unchecked((int)0xFD499334),
                unchecked((int)0x05D95785),
                unchecked((int)0xEAFE9CD1),
                unchecked((int)0xAD5EE601),
                unchecked((int)0x2F0E71D0),
                unchecked((int)0x36154091),
                unchecked((int)0x3DBBDE3E),
                unchecked((int)0x24A0EF7F),
                unchecked((int)0xA3E029CF),
                unchecked((int)0xB1558621),
                unchecked((int)0xE3581B63),
                unchecked((int)0xE29A7154),
                unchecked((int)0xE0DCCF0D),
                unchecked((int)0xE11EA53A),
                unchecked((int)0xE451B3BF),
                unchecked((int)0xCF2A3EFE),
                unchecked((int)0x9DF9AC47),
            };

            // Get the index of the latest befriendable yokai
            int lastBefriendableIndex = charaparams.FindLastIndex(x =>
            {
                ICharabase charabase = charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash);
                string tribeName = Tribes[charabase.Tribe];
                return ((x.ScoutableHash != 0x00 && x.ShowInMedalium == true)
                        || (x.ScoutableHash == 0x00 && x.ShowInMedalium == true && tribeName != "Boss" && tribeName != "Untribe"));
            }) + 1;

            int medalIndex = 699;

            foreach (int unbefriendableYokaiParamHash in unbefriendableYokaiParamHashes)
            {
                // Search if the boss exists
                if (charaparams.Any(x => x.ParamHash == unbefriendableYokaiParamHash))
                {
                    ICharaparam unbefriendableYokaiCharaparam = charaparams.FirstOrDefault(x => x.ParamHash == unbefriendableYokaiParamHash);
                    unbefriendableYokaiCharaparam.ShowInMedalium = true;
                    unbefriendableYokaiCharaparam.ScoutableHash = 4;
                    unbefriendableYokaiCharaparam.BattleType = 2;

                    if (unbefriendableYokaiCharaparam.MedaliumOffset == 0)
                    {
                        unbefriendableYokaiCharaparam.MedaliumOffset = 699;
                    }

                }
            }

            foreach (int unbefriendableYokaiBaseHash in unbefriendableYokaiBaseHashes)
            {
                int newBaseHash = unchecked((int)Crc32.Compute(Encoding.GetEncoding("Shift-JIS").GetBytes("added_base_" + medalIndex)));
                int newParamHash = unchecked((int)Crc32.Compute(Encoding.GetEncoding("Shift-JIS").GetBytes("added_param_" + medalIndex)));

                // Avoid to add already existing yokai
                if (!charabases.Any(x => x.BaseHash == newBaseHash))
                {
                    // Search yokai scale
                    if (charascales.Any(x => x.BaseHash == unbefriendableYokaiBaseHash))
                    {
                        ICharascale yokaiCharascale = (ICharascale)charascales.FirstOrDefault(x => x.BaseHash == unbefriendableYokaiBaseHash).Clone();
                        yokaiCharascale.BaseHash = newBaseHash;

                        // Perform blasters t scale
                        if (yokaiCharascale.Scale7 == 0)
                        {
                            yokaiCharascale.Scale7 = 1f;
                        }

                        charascales.Add(yokaiCharascale);
                    }
                    else
                    {
                        ICharascale yokaiCharascale = new Charascale();
                        yokaiCharascale.BaseHash = newBaseHash;
                        yokaiCharascale.Scale1 = 1f;
                        yokaiCharascale.Scale2 = 1f;
                        yokaiCharascale.Scale3 = 1f;
                        yokaiCharascale.Scale4 = 1f;
                        yokaiCharascale.Scale5 = 1f;
                        yokaiCharascale.Scale6 = 1f;
                        yokaiCharascale.Scale7 = 1f;
                        charascales.Add(yokaiCharascale);
                    }

                    ICharabase yokaiCharabase = (ICharabase)charabases.FirstOrDefault(x => x.BaseHash == unbefriendableYokaiBaseHash).Clone();
                    yokaiCharabase.BaseHash = newBaseHash;
                    yokaiCharabase.Tribe = 6;
                    charabases.Add(yokaiCharabase);
                }

                // Avoid to add already existing yokai
                if (!charaparams.Any(x => x.ParamHash == newParamHash))
                {
                    ICharaparam yokaiCharaparam = new Charaparam();
                    yokaiCharaparam.BaseHash = newBaseHash;
                    yokaiCharaparam.ParamHash = newParamHash;
                    yokaiCharaparam.Strongest = 1;
                    yokaiCharaparam.Weakness = 2;
                    yokaiCharaparam.EvolveParam = 0x00;
                    yokaiCharaparam.EvolveLevel = 0;
                    yokaiCharaparam.EvolveOffset = -1;
                    yokaiCharaparam.MedaliumOffset = 699;
                    yokaiCharaparam.ShowInMedalium = true;
                    yokaiCharaparam.ScoutableHash = 4;
                    yokaiCharaparam.BattleType = 2;
                    yokaiCharaparam.EquipmentSlotsAmount = 1;
                    charaparams.Insert(lastBefriendableIndex, yokaiCharaparam);

                    // Add battle charaparam
                    IBattleCharaparam yokaiBattleCharaparam = new BattleCharaparam();
                    yokaiBattleCharaparam.ParamHash = newParamHash;
                    yokaiBattleCharaparam.Experience = 5;
                    yokaiBattleCharaparam.Money = 5;
                    yokaiBattleCharaparam.Drop1Rate = 50;
                    yokaiBattleCharaparam.Drop2Rate = 50;
                    battleCharaparams.Insert(lastBefriendableIndex, yokaiBattleCharaparam);

                    // Add hackslash charaparam
                    IHackslashCharaparam yokaiHackslashCharaparam = new HackslashCharaparam();
                    yokaiHackslashCharaparam.ParamHash = newParamHash;
                    hackslashCharaparams.Insert(lastBefriendableIndex, yokaiHackslashCharaparam);

                    medalIndex++;
                    lastBefriendableIndex++;
                }
            }

            // Remove scoutable limit and specification for all scoutable yokai
            foreach(ICharaparam charaparam in charaparams.Where(x => x.ScoutableHash != 0x0))
            {
                charaparam.ScoutableHash = 4;
                charaparam.BattleType = 2;
            }
        }

        public void FixYokai(List<ICharabase> charabases)
        {
            ICharabase treetter = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0x9BCCD58A));
            if (treetter != null)
            {
                treetter.Rank = 0;
            }
        }

        public void FixArea(Dictionary<string, (List<int>, List<int>)> areas)
        {

        }

        public void FixShop()
        {

        }
    }
}
