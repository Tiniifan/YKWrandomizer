using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Text;
using YKWrandomizer.Level5.Archive.ARC0;
using YKWrandomizer.Level5.Binary;
using YKWrandomizer.Yokai_Watch.Games.YW3.Logic;
using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW3
{
    public class YWB2 : IGame
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

        public Dictionary<string, int> BossBattles => Common.Battles.BossBattles.YW3;

        public ARC0 Game { get; set; }

        public ARC0 Language { get; set; }

        public string LanguageCode { get; set; }

        private string RomfsPath;

        public Dictionary<string, GameFile> Files { get; set; }

        public YWB2(string romfsPath, string language)
        {
            RomfsPath = romfsPath;
            LanguageCode = language;

            Game = new ARC0(new FileStream(RomfsPath + @"\yw_a.fa", FileMode.Open));

            Files = new Dictionary<string, GameFile>
            {
                { "chara_text", new GameFile(Game, "/data/res/text/chara_text_" + LanguageCode + ".cfg.bin") },
                { "chara_desc_text", new GameFile(Game, "/data/res/text/chara_desc_text_" + LanguageCode + ".cfg.bin") },
                { "item_text", new GameFile(Game, "/data/res/text/item_text_" + LanguageCode + ".cfg.bin") },
                { "battle_text", new GameFile(Game, "/data/res/text/battle_text_" + LanguageCode + ".cfg.bin") },
                { "skill_text", new GameFile(Game, "/data/res/text/skill_text_" + LanguageCode + ".cfg.bin") },
                { "hackslash_technic_text", new GameFile(Game, "/data/res/text/hackslash/hackslash_technic_text_" + LanguageCode + ".cfg.bin") },
                { "hackslash_chara_ability_text", new GameFile(Game, "/data/res/text/hackslash/hackslash_chara_ability_text_" + LanguageCode + ".cfg.bin") },
                { "chara_ability_text", new GameFile(Game, "/data/res/text/chara_ability_text_" + LanguageCode + ".cfg.bin") },
                { "addmembermenu_text", new GameFile(Game, "/data/res/text/menu/addmembermenu_text_" + LanguageCode + ".cfg.bin") },
                { "face_icon", new GameFile(Game, "/data/menu/face_icon") },
                { "item_icon", new GameFile(Game, "/data/menu/item_icon") },
                { "model", new GameFile(Game, "/data/character") },
            };
        }

        public void Save()
        {
            // Save
            Game.Save(RomfsPath + @"\yw_a.fa.temp");

            // Close File
            Game.Close();

            if (File.Exists(RomfsPath + @"\yw_a.fa"))
            {
                File.Delete(RomfsPath + @"\yw_a.fa");
            }

            File.Move(RomfsPath + @"\yw_a.fa.temp", RomfsPath + @"\yw_a.fa");

            // Re Open
            Game = new ARC0(new FileStream(RomfsPath + @"\yw_a.fa", FileMode.Open));
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

            return charaparamFile.Entries
                .Where(x => x.GetName() == "CHARA_PARAM_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<Charaparam>())
                .ToArray();
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
                    string[] itemTypes = { "ITEM_CONSUME_LIST_BEG", "ITEM_CREATURE_LIST_BEG", "ITEM_HACKSLASH_BATTLE_LIST_BEG", "ITEM_FLAG_MANAGE_LIST_BEG", "ITEM_HIDDEN_TREASURE_LIST_BEG", "ITEM_SOUL_LIST_BEG", "ITEM_EQUIPMENT_LIST_BEG" };

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
            return null;
        }

        public (IEncountTable[], IEncountChara[]) GetMapEncounter(string mapName)
        {
            VirtualDirectory mapFolder = Game.Directory.GetFolderFromFullPath(Files["map_encounter"].Path);
            string lastEncountConfigFile = mapFolder.Files.Keys.Where(x => x.StartsWith(mapName + "_enc_")).OrderByDescending(x => x).First();

            CfgBin encountConfig = new CfgBin();
            encountConfig.Open(Game.Directory.GetFileFromFullPath(Files["map_encounter"].Path + mapName + "/" + lastEncountConfigFile));

            //IEncountTable[] encountTable = encountConfig.Entries
            //.Where(x => x.GetName() == "ENCOUNT_TABLE_BEGIN")
            //.SelectMany(x => x.Children)
            //.Select(x => x.ToClass<EncountTable>())
            //.ToArray();

            //IEncountChara[] encountChara = encountConfig.Entries
            //.Where(x => x.GetName() == "ENCOUNT_CHARA_BEGIN")
            //.SelectMany(x => x.Children)
            //.Select(x => x.ToClass<EncountChara>())
            //.ToArray();

            return (new IEncountTable[] { }, new IEncountChara[] { });
        }

        public void SaveMapEncounter(string mapName, IEncountTable[] encountTables, IEncountChara[] encountCharas)
        {

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
                .Select(x => x.ToClass<EncountTable>())
                .ToArray();

            IEncountChara[] encountChara = encountConfig.Entries
                .Where(x => x.GetName() == "ENCOUNT_CHARA_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<EncountChara>())
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

            encountConfig.ReplaceEntry("ENCOUNT_TABLE_BEGIN", "ENCOUNT_TABLE_INFO_", encountTables);
            encountConfig.ReplaceEntry("ENCOUNT_CHARA_BEGIN", "ENCOUNT_CHARA_INFO_", encountCharas);

            Game.Directory.GetFolderFromFullPath("/data/res/battle").Files[lastEncounter].ByteContent = encountConfig.Save();
        }
    }
}
