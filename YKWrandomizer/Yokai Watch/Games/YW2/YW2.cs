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
using YKWrandomizer.Yokai_Watch.Games.YW2.Logic;
using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW2
{
    public class YW2 : IGame 
    {
        public string Name => "Yo-Kai Watch 2";

        public Dictionary<uint, string> Attacks => Common.Attacks.YW2;

        public Dictionary<uint, string> Techniques => Common.Techniques.YW2;

        public Dictionary<uint, string> Inspirits => Common.Inspirits.YW2;

        public Dictionary<uint, string> Soultimates => Common.Soultimates.YW2;

        public Dictionary<uint, string> Skills => Common.Skills.YW2;

        public Dictionary<int, string> Tribes => Common.Tribes.YW2;

        public Dictionary<int, string> FoodsType => Common.FoodsType.YW2;

        public Dictionary<int, string> ScoutablesType => Common.ScoutablesType.YW2;

        public Dictionary<int, int> BossBattles => Common.Battles.BossBattles.YW2;

        public ARC0 Game { get; set; }

        public ARC0 Language { get; set; }

        public string LanguageCode { get; set; }

        private string RomfsPath;

        public Dictionary<string, GameFile> Files { get; set; }

        public YW2(string romfsPath, string language)
        {
            RomfsPath = romfsPath;
            LanguageCode = language;

            Game = new ARC0(new FileStream(RomfsPath + @"\yw2_a.fa", FileMode.Open));
            Language = new ARC0(new FileStream(RomfsPath + @"\yw2_lg_" + LanguageCode + ".fa", FileMode.Open));

            Files = new Dictionary<string, GameFile>
            {
                { "chara_text", new GameFile(Language, "/data/res/text/chara_text_" + LanguageCode + ".cfg.bin") },
                { "item_text", new GameFile(Language, "/data/res/text/item_text_" + LanguageCode + ".cfg.bin") },
                { "battle_text", new GameFile(Language, "/data/res/text/battle_text_" + LanguageCode + ".cfg.bin") },
                { "skill_text", new GameFile(Language, "/data/res/text/skill_text_" + LanguageCode + ".cfg.bin") },
                { "chara_ability_text", new GameFile(Language, "/data/res/text/chara_ability_text_" + LanguageCode + ".cfg.bin") },
                { "addmembermenu_text", new GameFile(Language, "/data/res/text/menu/addmembermenu_text_" + LanguageCode + ".cfg.bin") },
                { "face_icon", new GameFile(Game, "/data/menu/face_icon") },
                { "item_icon", new GameFile(Game, "/data/menu/item_icon") },
                { "model", new GameFile(Game, "/data/character") },
                { "map_encounter", new GameFile(Game, "/data/res/map") },
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
            Game.Save(tempPath + @"\yw2_a.fa");
            Language.Save(tempPath + @"\yw2_lg_" + LanguageCode + ".fa");

            // Close File
            Game.Close();
            Language.Close();

            // Move
            string[] sourceFiles = new string[2] { @"./temp/yw2_a.fa", @"./temp/yw2_lg_" + LanguageCode + ".fa" };
            string[] destinationFiles = new string[2] { RomfsPath + @"\yw2_a.fa", RomfsPath + @"\yw2_lg_" + LanguageCode + ".fa" };

            for (int i = 0; i < 2; i++)
            {
                if (File.Exists(destinationFiles[i]))
                {
                    File.Delete(destinationFiles[i]);
                }

                File.Move(sourceFiles[i], destinationFiles[i]);
            }

            // Re Open
            Game = new ARC0(new FileStream(RomfsPath + @"\yw2_a.fa", FileMode.Open));
            Language = new ARC0(new FileStream(RomfsPath + @"\yw2_lg_" + LanguageCode + ".fa", FileMode.Open));
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
            } else
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
                .Where(x => x.GetName() == "CHARA_PARAM_INFO_BEGIN")
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

            charaparamFile.ReplaceEntry("CHARA_PARAM_INFO_BEGIN", "CHARA_PARAM_INFO_", formatCharaparams);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files[lastCharaparam].ByteContent = charaparamFile.Save();
        }

        public ICharaevolve[] GetCharaevolution()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastCharaparam = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_param")).OrderByDescending(x => x).First();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastCharaparam));

            return charaparamFile.Entries
                .Where(x => x.GetName() == "CHARA_EVOLVE_INFO_BEGIN")
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

            charaparamFile.ReplaceEntry("CHARA_EVOLVE_INFO_BEGIN", "CHARA_EVOLVE_INFO_", formatCharaevolutions);

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
                    string[] itemTypes = { "ITEM_EQUIPMENT_BEGIN", "ITEM_SOUL_BEGIN", "ITEM_CONSUME_BEGIN"};
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
            return new IOrgetimeTechnic[0];
        }

        public ICharaabilityConfig[] GetAbilities()
        {
            VirtualDirectory characterFolder = Game.Directory.GetFolderFromFullPath("data/res/character");
            string lastskillFile = characterFolder.Files.Keys.Where(x => x.StartsWith("chara_ability")).OrderByDescending(x => x).First();

            CfgBin charaabilityConfig = new CfgBin();
            charaabilityConfig.Open(Game.Directory.GetFileFromFullPath("/data/res/character/" + lastskillFile));

            return charaabilityConfig.Entries
                .Where(x => x.GetName() == "CHARA_ABILITY_CONFIG_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<CharaabilityConfig>())
                .ToArray();
        }

        public IBattleCommand[] GetBattleCommands()
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
                .Where(x => x.GetName() == "ITABLE_DATA_MORE_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<ItableDataMore>())
                .ToArray();
        }

        public void SaveTreasureBox(string mapName, IItableDataMore[] itableDataMores)
        {
            ItableDataMore[] formatItableDataMore = itableDataMores.OfType<ItableDataMore>().ToArray();

            VirtualDirectory mapFolder = Game.Directory.GetFolderFromFullPath(Files["map_encounter"].Path);
            XPCK mapArchive = new XPCK(mapFolder.GetFolder(mapName).Files[mapName + ".pck"].ByteContent);
            string lastTboxConfigFile = mapArchive.Directory.Files.Keys.Where(x => x.StartsWith(mapName + "_tbox")).OrderByDescending(x => x).First();

            CfgBin tboxConfig = new CfgBin();
            mapArchive.Directory.Files[lastTboxConfigFile].Read();
            tboxConfig.Open(mapArchive.Directory.Files[lastTboxConfigFile].ByteContent);

            tboxConfig.ReplaceEntry("ITABLE_DATA_MORE_BEGIN", "ITABLE_DATA_MORE_", formatItableDataMore);

            mapArchive.Directory.Files[lastTboxConfigFile].ByteContent = tboxConfig.Save();
            mapFolder.GetFolder(mapName).Files[mapName + ".pck"].ByteContent = mapArchive.Save();
        }

        public ICapsuleConfig[] GetCapsuleConfigs()
        {
            VirtualDirectory capsuleFolder = Game.Directory.GetFolderFromFullPath("data/res/capsule");
            string lastCapsuleConfig = capsuleFolder.Files.Keys.Where(x => x.StartsWith("capsule_config")).OrderByDescending(x => x).First();

            CfgBin capsuleConfigFile = new CfgBin();
            capsuleConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/capsule/" + lastCapsuleConfig));

            return capsuleConfigFile.Entries
                .Where(x => x.GetName() == "CAPSULE_ITEM_INFO_LIST_BEG")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<CapsuleConfig>())
                .ToArray();
        }

        public void SaveCapsuleConfigs(ICapsuleConfig[] capsuleConfigs)
        {
            CapsuleConfig[] formatCapsuleConfigs = capsuleConfigs.OfType<CapsuleConfig>().ToArray();

            VirtualDirectory capsuleFolder = Game.Directory.GetFolderFromFullPath("data/res/capsule");
            string lastCapsuleConfig = capsuleFolder.Files.Keys.Where(x => x.StartsWith("capsule_config")).OrderByDescending(x => x).First();

            CfgBin capsuleConfigFile = new CfgBin();
            capsuleConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/capsule/" + lastCapsuleConfig));

            capsuleConfigFile.ReplaceEntry("CAPSULE_ITEM_INFO_LIST_BEG", "CAPSULE_ITEM_INFO_", formatCapsuleConfigs);

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

        public ICombineConfig[] GetFusions()
        {
            VirtualDirectory shopFolder = Game.Directory.GetFolderFromFullPath("data/res/shop");
            string lastCombineConfig = shopFolder.Files.Keys.Where(x => x.StartsWith("combine_config")).OrderByDescending(x => x).First();

            CfgBin combineConfigFile = new CfgBin();
            combineConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/shop/" + lastCombineConfig));

            return combineConfigFile.Entries
                .Where(x => x.GetName() == "COMBINE_INFO_BEGIN")
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

            combineConfigFile.ReplaceEntry("COMBINE_INFO_BEGIN", "COMBINE_INFO_", formatCombineConfigs);

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

            encountConfig.ReplaceEntry("ENCOUNT_TABLE_BEGIN", "ENCOUNT_TABLE_", formatEncountTables);
            encountConfig.ReplaceEntry("ENCOUNT_CHARA_BEGIN", "ENCOUNT_CHARA_", formatEncountCharas);

            Game.Directory.GetFolderFromFullPath("/data/res/battle").Files[lastEncounter].ByteContent = encountConfig.Save();
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
                unchecked((int)0x316EF7F4),
                unchecked((int)0x332849AD),
                unchecked((int)0x99218126),
                unchecked((int)0x39B4F2D8),
                unchecked((int)0xC998945C),
                unchecked((int)0xCBDE2A05),
                unchecked((int)0x661FFE64),
                unchecked((int)0x51C10E56),
                unchecked((int)0x09A21E00),
                unchecked((int)0x7C523651),
                unchecked((int)0x7E148808),
                unchecked((int)0x6EE799BF),
                unchecked((int)0x6CA127E6),
                unchecked((int)0x716036D7),
                unchecked((int)0x7326888E),
                unchecked((int)0xC9DC51B2),
                unchecked((int)0xCB9AEFEB),
                unchecked((int)0xF6FAC65B),
                unchecked((int)0x98F9015D),
                unchecked((int)0x9ABFBF04),
                unchecked((int)0x0DD65E3F),
                unchecked((int)0x48309AB6),
                unchecked((int)0x4A7624EF),
                unchecked((int)0x5A853558),
                unchecked((int)0x58C38B01),
                unchecked((int)0x33F317D7),
                unchecked((int)0x612EDE03),
                unchecked((int)0xD992B966),
                unchecked((int)0x444581DF),
            };

            List<int> yoBossBaseHashes = new List<int>()
            {
                unchecked((int)0x84EF7807),
                unchecked((int)0x9DF44946),
                unchecked((int)0xC34F02D7),
                unchecked((int)0xD1FAAD39),
                unchecked((int)0x6946CA5C),
                unchecked((int)0xF491F2E5),
                unchecked((int)0x4C2D9580),
                unchecked((int)0xDA543396),
                unchecked((int)0xC8E19C78),
                unchecked((int)0x705DFB1D),
                unchecked((int)0xED8AC3A4),
                unchecked((int)0x5536A4C1),
                unchecked((int)0xFE2F2B67),
                unchecked((int)0xE7341A26),
                unchecked((int)0x4C0FF777),
                unchecked((int)0x5514C636),
                unchecked((int)0x5EBA5899),
                unchecked((int)0x47A169D8),
                unchecked((int)0x716FDEC7),
                unchecked((int)0x36CFA417),
                unchecked((int)0x2FD49556),
                unchecked((int)0x0BAF8DA7),
                unchecked((int)0x89FF1A76),
                unchecked((int)0x90E42B37),
                unchecked((int)0xB49F33C6),
                unchecked((int)0x3F1B2319),
                unchecked((int)0x027B0AA9),
                unchecked((int)0x10CEA547),
                unchecked((int)0xA872C222),
                unchecked((int)0xF447F0BC),
                unchecked((int)0xED5CC1FD),
                unchecked((int)0xC671923E),
                unchecked((int)0xE6F25F52),
                unchecked((int)0xC927D90C),
                unchecked((int)0xD03CE84D),
                unchecked((int)0xCFEC1BE7),
                unchecked((int)0x8E87A3DC),
                unchecked((int)0x979C929D),
                unchecked((int)0x9C320C32),
                unchecked((int)0x85293D73),
                unchecked((int)0x248E6B57),
                unchecked((int)0xB3E78A6C),
                unchecked((int)0xAAFCBB2D),
                unchecked((int)0x01C7567C),
                unchecked((int)0x18DC673D),
                unchecked((int)0x33F134FE),
                unchecked((int)0x3CA77FCC),
                unchecked((int)0x25BC4E8D),
                unchecked((int)0x7B07051C),
                unchecked((int)0x621C345D),
                unchecked((int)0x46672CAC),
                unchecked((int)0xC437BB7D),
                unchecked((int)0xE942C004),
                unchecked((int)0xD422E9B4),
                unchecked((int)0x93829364),
                unchecked((int)0xAEE2BAD4),
                unchecked((int)0x1CC266C4),
                unchecked((int)0x21A24F74),
                unchecked((int)0x660235A4),
            };

            // Get the index of the latest befriendable yokai
            int lastBefriendableIndex = charaparams.FindLastIndex(x =>
            {
                ICharabase charabase = charabases.FirstOrDefault(y => y.BaseHash == x.BaseHash);
                string tribeName = Tribes[charabase.Tribe];
                return ((x.ScoutableHash != 0x00 && x.ShowInMedalium == true)
                        || (x.ScoutableHash == 0x00 && x.ShowInMedalium == true && tribeName != "Boss" && tribeName != "Untribe"));
            }) + 1;

            int medalIndex = 449;

            foreach (int unbefriendableYokaiParamHash in unbefriendableYokaiParamHashes)
            {
                // Search if the boss exists
                if (charaparams.Any(x => x.ParamHash == unbefriendableYokaiParamHash))
                {
                    ICharaparam unbefriendableYokaiCharaparam = charaparams.FirstOrDefault(x => x.ParamHash == unbefriendableYokaiParamHash);
                    unbefriendableYokaiCharaparam.ShowInMedalium = true;
                    unbefriendableYokaiCharaparam.ScoutableHash = 0x00654331;

                    if (unbefriendableYokaiCharaparam.MedaliumOffset == 0)
                    {
                        unbefriendableYokaiCharaparam.MedaliumOffset = 449;
                    }

                }
            }

            foreach (int yoBossBaseHash in yoBossBaseHashes)
            {
                int newBaseHash = unchecked((int)Crc32.Compute(Encoding.GetEncoding("Shift-JIS").GetBytes("added_base_" + medalIndex)));
                int newParamHash = unchecked((int)Crc32.Compute(Encoding.GetEncoding("Shift-JIS").GetBytes("added_param_" + medalIndex)));

                // Avoid to add already existing yokai
                if (!charabases.Any(x => x.BaseHash == newBaseHash))
                {
                    // Search boss scale
                    if (charascales.Any(x => x.BaseHash == yoBossBaseHash))
                    {
                        ICharascale yokaiBossCharascale = (ICharascale)charascales.FirstOrDefault(x => x.BaseHash == yoBossBaseHash).Clone();
                        yokaiBossCharascale.BaseHash = newBaseHash;
                        yokaiBossCharascale.Scale1 /= 1.5f;
                        yokaiBossCharascale.Scale2 /= 1.5f;
                        yokaiBossCharascale.Scale3 /= 2f;
                        yokaiBossCharascale.Scale4 /= 1.5f;
                        yokaiBossCharascale.Scale5 /= 1.1f;
                        yokaiBossCharascale.Scale6 /= 1.5f;

                        if (yoBossBaseHash == unchecked((int)0xC437BB7D))
                        {
                            yokaiBossCharascale.Scale1 /= 2f;
                            yokaiBossCharascale.Scale2 /= 2f;
                            yokaiBossCharascale.Scale3 /= 2f;
                            yokaiBossCharascale.Scale4 /= 2f;
                            yokaiBossCharascale.Scale5 /= 2f;
                            yokaiBossCharascale.Scale6 /= 2f;
                        }

                        charascales.Add(yokaiBossCharascale);
                    }
                    else
                    {
                        ICharascale yokaiBossCharascale = new Charascale();
                        yokaiBossCharascale.BaseHash = newBaseHash;
                        yokaiBossCharascale.Scale1 = 0.5f;
                        yokaiBossCharascale.Scale2 = 0.5f;
                        yokaiBossCharascale.Scale3 = 0.5f;
                        yokaiBossCharascale.Scale4 = 0.5f;
                        yokaiBossCharascale.Scale5 = 0.5f;
                        charascales.Add(yokaiBossCharascale);
                    }

                    ICharabase yokaiBossCharabase = (ICharabase)charabases.FirstOrDefault(x => x.BaseHash == yoBossBaseHash).Clone();
                    yokaiBossCharabase.BaseHash = newBaseHash;
                    yokaiBossCharabase.Tribe = 6;
                    charabases.Add(yokaiBossCharabase);
                }

                // Avoid to add already existing yokai
                if (!charaparams.Any(x => x.ParamHash == newParamHash))
                {
                    ICharaparam yokaiBossCharaparam = new Charaparam();

                    yokaiBossCharaparam.BaseHash = newBaseHash;
                    yokaiBossCharaparam.ParamHash = newParamHash;
                    yokaiBossCharaparam.AttributeDamageEarth = 1;
                    yokaiBossCharaparam.AttributeDamageFire = 1;
                    yokaiBossCharaparam.AttributeDamageIce = 1;
                    yokaiBossCharaparam.AttributeDamageLigthning = 1;
                    yokaiBossCharaparam.AttributeDamageWater = 1;
                    yokaiBossCharaparam.AttributeDamageWind = 1;
                    yokaiBossCharaparam.EvolveParam = 0x00;
                    yokaiBossCharaparam.EvolveLevel = 0;
                    yokaiBossCharaparam.EvolveOffset = -1;
                    yokaiBossCharaparam.MedaliumOffset = 449;
                    yokaiBossCharaparam.ShowInMedalium = true;
                    yokaiBossCharaparam.ScoutableHash = 0x00654331;
                    yokaiBossCharaparam.EquipmentSlotsAmount = 1;
                    yokaiBossCharaparam.Experience = 5;
                    yokaiBossCharaparam.Money = 5;
                    yokaiBossCharaparam.Drop1Rate = 50;
                    yokaiBossCharaparam.Drop2Rate = 50;

                    charaparams.Insert(lastBefriendableIndex, yokaiBossCharaparam);

                    medalIndex++;
                    lastBefriendableIndex++;
                }
            }
        }

        public void FixYokai(List<ICharabase> charabases)
        {
            ICharabase brushido = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0xCC79FD93));
            if (brushido != null)
            {
                brushido.Rank = 0;
            }

            ICharabase ake = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0x3D3DBE2C));
            if (ake != null)
            {
                ake.Rank = 0;
            }

            ICharabase flushback = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0x12589D32));
            if (flushback != null)
            {
                flushback.Rank = 0;
            }

            ICharabase wiglin = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0x9A067621));
            if (wiglin != null)
            {
                wiglin.Rank = 0;
            }

            ICharabase rhyth = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0xA83014A3));
            if (rhyth != null)
            {
                if (rhyth.Rank > 0x01)
                {
                    rhyth.Rank = 0x01;
                }
            }

            ICharabase steppa = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0x831D4760));
            if (steppa != null)
            {
                steppa.Rank = 0;
            }

            ICharabase fidgephant = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0xA7665F91));
            if (fidgephant != null)
            {
                if (fidgephant.Rank > 0x01)
                {
                    fidgephant.Rank = 0x01;
                }
            }
        }

        public void FixArea(Dictionary<string, (List<int>, List<int>)> areas)
        {
            // Les 3 algues de morts and Flushback
            areas["t121d11"].Item1[0] = unchecked((int)0xE516B755);
            areas["t121d11"].Item1[2] = unchecked((int)0xCE3BE496);
            areas["t121d11"].Item1[1] = unchecked((int)0xFC0D8614);
            areas["t121d11"].Item1[3] = unchecked((int)0x74536D07);

            // Ake fusion quest
            areas["t103d11"].Item1[0] = unchecked((int)0x5B364E19);

            // fidgephant
            areas["t131g00"].Item1[5] = unchecked((int)0xC16DAFA4);
            areas["t131g00"].Item1[6] = unchecked((int)0xC16DAFA4);
            areas["t131g00"].Item1[7] = unchecked((int)0xC16DAFA4);
        }

        public void FixShop()
        {
            // Bamboo quest
            string[] files = new string[] { "shop_shpN024.cfg", "shop_shpN024_0.01n.cfg" };
            int[] pos = new int[] { 2, 4 };

            for (int i = 0; i < files.Length; i++)
            {
                (IShopConfig[], IShopValidCondition[]) shopData = GetShop(files[i]);
                IShopConfig[] shopConfigs = shopData.Item1;
                IShopValidCondition[] validConditions = null;

                if (shopData.Item2 != null && shopData.Item2.Length > 0)
                {
                    validConditions = shopData.Item2;
                }

                shopConfigs[pos[i]].ItemHash = unchecked((int)0x3CF5AC04);
                shopConfigs[pos[i]].Price = 500;

                if (validConditions != null && shopConfigs[pos[i]].ShopValidConditionIndex != -1)
                {
                    validConditions[shopConfigs[pos[i]].ShopValidConditionIndex].Price = 500;
                }

                // Save
                SaveShop(files[i], shopConfigs, validConditions);
            }
        }
    }
}
