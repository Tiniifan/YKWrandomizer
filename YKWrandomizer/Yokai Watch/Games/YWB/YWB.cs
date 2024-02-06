using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Text;
using YKWrandomizer.Level5.Binary;
using YKWrandomizer.Level5.Binary.Logic;
using YKWrandomizer.Yokai_Watch.Logic;
using YKWrandomizer.Level5.Archive.ARC0;
using YKWrandomizer.Level5.Archive.XPCK;
using YKWrandomizer.Yokai_Watch.Games.YWB.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YWB
{
    public class YWB : IGame
    {
        public string Name => "Yo-Kai Watch Blaster";

        public Dictionary<uint, string> Attacks => Common.Attacks.YW1;

        public Dictionary<uint, string> Techniques => Common.Techniques.YW1;

        public Dictionary<uint, string> Inspirits => Common.Inspirits.YW1;

        public Dictionary<uint, string> Soultimates => Common.Soultimates.YW1;

        public Dictionary<uint, string> Skills => Common.Skills.YW1;

        public Dictionary<int, string> Tribes => Common.Tribes.YWB;

        public Dictionary<int, string> FoodsType => new Dictionary<int, string>();

        public Dictionary<int, string> ScoutablesType => Common.ScoutablesType.YW3;

        public Dictionary<string, int> BossBattles => Common.Battles.BossBattles.YWB;

        public ARC0 Game { get; set; }

        public ARC0 Language { get; set; }

        public string LanguageCode { get; set; }

        private string RomfsPath;

        public Dictionary<string, GameFile> Files { get; set; }

        public YWB(string romfsPath, string language)
        {
            RomfsPath = romfsPath;
            LanguageCode = language;

            Game = new ARC0(new FileStream(RomfsPath + @"\yw_a.fa", FileMode.Open));
            Language = new ARC0(new FileStream(RomfsPath + @"\ywb_lg_" + LanguageCode + ".fa", FileMode.Open));

            Files = new Dictionary<string, GameFile>
            {
                { "chara_text", new GameFile(Language, "/data/res/text/chara_text_" + LanguageCode + ".cfg.bin") },
                { "item_text", new GameFile(Language, "/data/res/text/item_text_" + LanguageCode + ".cfg.bin") },
                { "battle_text", new GameFile(Language, "/data/res/text/battle_text_" + LanguageCode + ".cfg.bin") },
                { "skill_text", new GameFile(Language, "/data/res/text/skill_text_" + LanguageCode + ".cfg.bin") },
                { "chara_ability_text", new GameFile(Language, "/data/res/text/chara_ability_text_" + LanguageCode + ".cfg.bin") },
                { "addmembermenu_text", new GameFile(Language, "/data/res/text/menu/addmembermenu_text_" + LanguageCode + ".cfg.bin") },
                { "orgetime_technic", new GameFile(Language, "/data/res/text/orgetime_technic_text_" + LanguageCode + ".cfg.bin") },
                { "face_icon", new GameFile(Game, "/data/menu/face_icon") },
                { "item_icon", new GameFile(Game, "/data/menu/item_icon") },
                { "model", new GameFile(Game, "/data/character") },
                { "shop", new GameFile(Game, "/data/res/shop") },
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
            Language.Save(tempPath + @"\ywb_lg_" + LanguageCode + ".fa");

            // Close File
            Game.Close();
            Language.Close();

            // Move
            string[] sourceFiles = new string[2] { @"./temp/yw_a.fa", @"./temp/ywb_lg_" + LanguageCode + ".fa" };
            string[] destinationFiles = new string[2] { RomfsPath + @"\yw_a.fa", RomfsPath + @"\ywb_lg_" + LanguageCode + ".fa" };

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
            Language = new ARC0(new FileStream(RomfsPath + @"\ywb_lg_" + LanguageCode + ".fa", FileMode.Open));
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
            string lastcharabase = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_param")).OrderByDescending(x => x).First();

            CfgBin charaBaseFile = new CfgBin();
            charaBaseFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastcharabase));

            return charaBaseFile.Entries
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
                .Where(x => x.GetName() == "CHARA_PARAM_EVOLVE_INFO_LIST_BEG")
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

            charaparamFile.ReplaceEntry("CHARA_PARAM_EVOLVE_INFO_LIST_BEG", "CHARA_PARAM_EVOLVE_INFO_", formatCharaevolutions);

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
                    string[] itemTypes = { "ITEM_EQUIPMENT_BEGIN", "ITEM_SOUL_BEGIN", "ITEM_BATTLE_BEGIN", "ITEM_CONSUME_BEGIN", "ITEM_ELEMENT_BEGIN", "ITEM_AURA_BEGIN", "ITEM_KEY_BEGIN" };
                    return itemconfigFile.Entries
                        .Where(x => itemTypes.Contains(x.GetName()))
                        .SelectMany(x => x.Children)
                        .Select(x => x.ToClass<Item>())
                        .ToArray();
                default:
                    return new IItem[] { };
            }
        }

        public ISkillconfig[] GetSkills()
        {
            return null;
        }

        public IBattleCharaparam[] GetBattleCharaparam()
        {
            return null; ;
        }

        public void SaveBattleCharaparam(IBattleCharaparam[] battleCharaparams)
        {

        }

        public IHackslashCharaparam[] GetHackslashCharaparam()
        {
            return null;
        }

        public void SaveHackslashCharaparam(IHackslashCharaparam[] hackslashCharaparams)
        {

        }

        public IHackslashCharaabilityConfig[] GetHackslashAbilities()
        {
            return null;
        }

        public IHackslashTechnic[] GetHackslashSkills()
        {
            return null;
        }

        public IOrgetimeTechnic[] GetOrgetimeTechnics()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/orge");
            string lastskillFile = characterFolder.Files.Keys.Where(x => x.StartsWith("orge_time_technic") && x.Contains("menu") == false).OrderByDescending(x => x).First();

            CfgBin charaabilityConfig = new CfgBin();
            charaabilityConfig.Open(Game.Directory.GetFileFromFullPath("/data/res/orge/" + lastskillFile));

            return charaabilityConfig.Entries
                .Where(x => x.GetName() == "ORGE_TECHNIC_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<OrgetimeTechnic>())
                .ToArray();
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
                .Select(x => x.ToClass<CharaabilityConfig>())
                .ToArray();
        }

        public IBattleCommand[] GetBattleCommands()
        {
            return new IBattleCommand[0];
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
                        pckFile.Read();

                        XPCK mapArchive = new XPCK(pckFile.ByteContent);

                        if (mapArchive.Directory.Files.Any(file => file.Key.StartsWith(folder.Name + "_enc_")))
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
            mapArchive.Directory.Files[lastEncountConfigFile].Read();
            encountConfig.Open(mapArchive.Directory.Files[lastEncountConfigFile].ByteContent);

            IEncountTable[] encountTable = encountConfig.Entries
                .Where(x => x.GetName() == "ENCOUNT_TABLE_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<EncountTable>())
                .ToArray();

            return (encountTable, null);
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
                    .Where(x => x.GetName() == "COMBINE_INFO_CNF_LIST")
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

            int index = 0;
            foreach(Entry entry in combineConfigFile.Entries.Where((x => x.GetName() == "COMBINE_INFO_LIST_BEG"))) 
            {
                if (entry.GetName() == "COMBINE_INFO_CNF_LIST")
                {
                    entry.Children[0].SetVariablesFromClass(formatCombineConfigs[index]);
                    index++;
                }
            }

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

            return (encountTable, null);
        }

        public void SaveStaticEncounters(IEncountTable[] encountTables, IEncountChara[] encountCharas)
        {
            EncountTable[] formatEncountTables = encountTables.OfType<EncountTable>().ToArray();

            VirtualDirectory battleFolder = Game.Directory.GetFolderFromFullPath("data/res/battle");
            string lastEncounter = battleFolder.Files.Keys.Where(x => x.StartsWith("common_enc")).OrderByDescending(x => x).First();

            CfgBin encountConfig = new CfgBin();
            encountConfig.Open(Game.Directory.GetFileFromFullPath("/data/res/battle/" + lastEncounter));

            encountConfig.ReplaceEntry("ENCOUNT_TABLE_BEGIN", "ENCOUNT_TABLE_INFO_", encountTables);

            Game.Directory.GetFolderFromFullPath("/data/res/battle").Files[lastEncounter].ByteContent = encountConfig.Save();
        }

        public (IShopConfig[], IShopValidCondition[]) GetShop(string shopName)
        {
            return (null, null);
        }

        public void SaveShop(string shopName, IShopConfig[] shopConfigs, IShopValidCondition[] shopValidConditions)
        {

        }

        public string[] GetMapWhoContainsTreasureBoxes()
        {
            return null;
        }

        public IItableDataMore[] GetTreasureBox(string mapName)
        {
            return null;
        }

        public void SaveTreasureBox(string mapName, IItableDataMore[] itableDataMores)
        {

        }

        public ICapsuleConfig[] GetCapsuleConfigs()
        {
            return null;
        }

        public void SaveCapsuleConfigs(ICapsuleConfig[] capsuleConfigs)
        {

        }

        public ILegendDataConfig[] GetLegends()
        {
            return null;
        }

        public void SaveLegends(ILegendDataConfig[] legendDataConfigs)
        {

        }

        public void UnlockUnscoutableYokai(List<ICharaparam> charaparams, List<ICharabase> charabases, List<ICharascale> charascales, List<IHackslashCharaparam> hackslashCharaparams = null, List<IBattleCharaparam> battleCharaparams = null)
        {

        }

        public void FixYokai(List<ICharabase> charabases)
        {

        }
    }
}
