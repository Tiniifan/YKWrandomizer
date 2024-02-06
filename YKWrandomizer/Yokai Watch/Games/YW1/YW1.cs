using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using YKWrandomizer.Tools;
using YKWrandomizer.Level5.Text;
using YKWrandomizer.Level5.Archive.ARC0;
using YKWrandomizer.Level5.Binary;
using YKWrandomizer.Level5.Binary.Logic;
using YKWrandomizer.Yokai_Watch.Games;
using YKWrandomizer.Yokai_Watch.Games.YW1.Logic;
using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games.YW1
{
    public class YW1 : IGame
    {
        public string Name => "Yo-Kai Watch 1";

        public Dictionary<uint, string> Attacks => Common.Attacks.YW1;

        public Dictionary<uint, string> Techniques => Common.Techniques.YW1;

        public Dictionary<uint, string> Inspirits => Common.Inspirits.YW1;

        public Dictionary<uint, string> Soultimates => Common.Soultimates.YW1;

        public Dictionary<uint, string> Skills => Common.Skills.YW1;

        public Dictionary<int, string> Tribes => Common.Tribes.YW1;

        public Dictionary<int, string> FoodsType => Common.FoodsType.YW1;

        public Dictionary<int, string> ScoutablesType => Common.ScoutablesType.YW1;

        public Dictionary<string, int> BossBattles => Common.Battles.BossBattles.YW1;

        public ARC0 Game { get; set; }

        public ARC0 Language { get; set; }

        public string LanguageCode { get; set; }

        private string RomfsPath;
        public Dictionary<string, GameFile> Files { get; set; }

        public YW1(string romfsPath, string language)
        {
            RomfsPath = romfsPath;
            LanguageCode = language;

            Game = new ARC0(new FileStream(RomfsPath + @"\yw1_a.fa", FileMode.Open));

            Files = new Dictionary<string, GameFile>
            {
                { "chara_text", new GameFile(Game, "/data/res/text/chara_text_" + LanguageCode + ".cfg.bin") },
                { "item_text", new GameFile(Game, "/data/res/text/item_text_" + LanguageCode + ".cfg.bin") },
                { "battle_text", new GameFile(Game, "/data/res/text/battle_text_" + LanguageCode + ".cfg.bin") },
                { "skill_text", new GameFile(Game, "/data/res/text/skill_text_" + LanguageCode + ".cfg.bin") },
                { "chara_ability_text", new GameFile(Game, "/data/res/text/chara_ability_text_" + LanguageCode + ".cfg.bin") },
                { "addmembermenu_text", new GameFile(Game, "/data/res/text/menu/addmembermenu_text_" + LanguageCode + ".cfg.bin") },
                { "system_text", new GameFile(Game, "/data/res/text/system_text_" + LanguageCode + ".cfg.bin") },
                { "face_icon", new GameFile(Game, "/data/menu/face_icon") },
                { "item_icon", new GameFile(Game, "/data/menu/item_icon") },
                { "model", new GameFile(Game, "/data/character") },
                { "map_encounter", new GameFile(Game, "/data/res/map") },
                { "shop", new GameFile(Game, "/data/res/shop") },
            };
        }

        public void Save()
        {
            // Save
            Game.Save(RomfsPath + @"\yw1_a.fa.temp");

            // Close File
            Game.Close();

            if (File.Exists(RomfsPath + @"\yw1_a.fa"))
            {
                File.Delete(RomfsPath + @"\yw1_a.fa");
            }

            File.Move(RomfsPath + @"\yw1_a.fa.temp", RomfsPath + @"\yw1_a.fa");

            // Re Open
            Game = new ARC0(new FileStream(RomfsPath + @"\yw1_a.fa", FileMode.Open));
        }

        public ICharabase[] GetCharacterbase(bool isYokai)
        {
            CfgBin charaBaseFile = new CfgBin();
            charaBaseFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/chara_base_0.04j.cfg.bin"));

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

            CfgBin charaBaseFile = new CfgBin();
            charaBaseFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/chara_base_0.04j.cfg.bin"));

            charaBaseFile.ReplaceEntry("CHARA_BASE_INFO_BEGIN", "CHARA_BASE_INFO_", npcCharabases);
            charaBaseFile.ReplaceEntry("CHARA_BASE_YOKAI_INFO_BEGIN", "CHARA_BASE_YOKAI_INFO_", yokaiCharabases);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files["chara_base_0.04j.cfg.bin"].ByteContent = charaBaseFile.Save();
        }

        public ICharascale[] GetCharascale()
        {
            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/chara_scale.cfg.bin"));

            return charaparamFile.Entries
                .Where(x => x.GetName() == "CHARA_SCALE_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<Charascale>())
                .ToArray();
        }

        public void SaveCharascale(ICharascale[] charascales)
        {
            Charascale[] formatCharascales = charascales.OfType<Charascale>().ToArray();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/chara_scale.cfg.bin"));

            charaparamFile.ReplaceEntry("CHARA_SCALE_INFO_BEGIN", "CHARA_SCALE_INFO_", formatCharascales);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files["chara_scale.cfg.bin"].ByteContent = charaparamFile.Save();
        }

        public ICharaparam[] GetCharaparam()
        {
            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.02.cfg.bin"));

            return charaparamFile.Entries
                .Where(x => x.GetName() == "CHARA_PARAM_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<Charaparam>())
                .ToArray();
        }

        public void SaveCharaparam(ICharaparam[] charaparams)
        {
            Charaparam[] formatCharaparams = charaparams.OfType<Charaparam>().ToArray();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.02.cfg.bin"));

            charaparamFile.ReplaceEntry("CHARA_PARAM_INFO_BEGIN", "CHARA_PARAM_INFO_", formatCharaparams);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files["chara_param_0.02.cfg.bin"].ByteContent = charaparamFile.Save();
        }

        public ICharaevolve[] GetCharaevolution()
        {
            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.02.cfg.bin"));

            return charaparamFile.Entries
                .Where(x => x.GetName() == "CHARA_EVOLVE_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<Charaevolve>())
                .ToArray();
        }

        public void SaveCharaevolution(ICharaevolve[] charaevolutions)
        {
            Charaevolve[] formatCharaevolutions = charaevolutions.OfType<Charaevolve>().ToArray();

            CfgBin charaparamFile = new CfgBin();
            charaparamFile.Open(Game.Directory.GetFileFromFullPath("/data/res/character/chara_param_0.02.cfg.bin"));

            charaparamFile.ReplaceEntry("CHARA_EVOLVE_INFO_BEGIN", "CHARA_EVOLVE_INFO_", formatCharaevolutions);

            Game.Directory.GetFolderFromFullPath("/data/res/character").Files["chara_param_0.02.cfg.bin"].ByteContent = charaparamFile.Save();
        }

        public IItem[] GetItems(string itemType)
        {
            CfgBin itemconfigFile = new CfgBin();
            itemconfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/item/item_config_0.05d.cfg.bin"));

            switch (itemType)
            {
                case "equipment":
                    return itemconfigFile.Entries
                        .Where(x => x.GetName() == "ITEM_EQUIPMENT_BEGIN")
                        .SelectMany(x => x.Children)
                        .Select(x => x.ToClass<Item>())
                        .ToArray();
                case "consumable":
                    return itemconfigFile.Entries
                        .Where(x => x.GetName() == "ITEM_CONSUME_BEGIN")
                        .SelectMany(x => x.Children)
                        .Select(x => x.ToClass<Item>())
                        .ToArray();
                case "important":
                    return itemconfigFile.Entries
                        .Where(x => x.GetName() == "ITEM_IMPORTANT_BEGIN")
                        .SelectMany(x => x.Children)
                        .Select(x => x.ToClass<Item>())
                        .ToArray();
                case "creature":
                    return itemconfigFile.Entries
                        .Where(x => x.GetName() == "ITEM_CREATURE_BEGIN")
                        .SelectMany(x => x.Children)
                        .Select(x => x.ToClass<Item>())
                        .ToArray();
                case "all":
                    string[] itemTypes = { "ITEM_EQUIPMENT_BEGIN", "ITEM_CONSUME_BEGIN"};
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
                .Where(x => x.GetName() == "CHARA_ABILITY_CONFIG_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<CharaabilityConfig>())
                .ToArray();
        }

        public IBattleCommand[] GetBattleCommands()
        {
            VirtualDirectory battleFolder = Game.Directory.GetFolderFromFullPath("data/res/battle");
            string lastBattleCommand = battleFolder.Files.Keys.Where(x => x.StartsWith("battle_command") && !x.Contains("config")).OrderByDescending(x => x).First();

            CfgBin battlecommandConfig = new CfgBin();
            battlecommandConfig.Open(Game.Directory.GetFileFromFullPath("/data/res/battle/" + lastBattleCommand));

            return battlecommandConfig.Entries
                .Where(x => x.GetName() == "BATTLE_COMMAND_INFO_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<BattleCommand>())
                .ToArray();
        }

        public string[] GetMapWhoContainsEncounter()
        {
            VirtualDirectory mapEncounterFolder = Game.Directory.GetFolderFromFullPath("/data/res/map");

            return mapEncounterFolder.Folders
                    .Where(folder =>
                        folder.Files.Any(file =>
                        file.Key.StartsWith(folder.Name + "_enc_")))
                    .Select(folder => folder.Name)
                    .ToArray();
        }

        public (IEncountTable[], IEncountChara[]) GetMapEncounter(string mapName)
        {
            VirtualDirectory mapFolder = Game.Directory.GetFolderFromFullPath(Files["map_encounter"].Path);
            string lastEncountConfigFile = mapFolder.GetFolder(mapName).Files.Keys.Where(x => x.StartsWith(mapName + "_enc_") && !x.Contains("_enc_pos")).OrderByDescending(x => x).First();

            CfgBin encountConfig = new CfgBin();
            encountConfig.Open(Game.Directory.GetFileFromFullPath(Files["map_encounter"].Path + "/" + mapName + "/" + lastEncountConfigFile));

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
            string lastEncountConfigFile = mapFolder.GetFolder(mapName).Files.Keys.Where(x => x.StartsWith(mapName + "_enc_") && !x.Contains("_enc_pos")).OrderByDescending(x => x).First();

            CfgBin encountConfig = new CfgBin();
            encountConfig.Open(Game.Directory.GetFileFromFullPath(Files["map_encounter"].Path + "/" + mapName + "/" + lastEncountConfigFile));

            encountConfig.ReplaceEntry("ENCOUNT_TABLE_BEGIN", "ENCOUNT_TABLE_", formatEncountTables);
            encountConfig.ReplaceEntry("ENCOUNT_CHARA_BEGIN", "ENCOUNT_CHARA_", formatEncountCharas);

            Game.Directory.GetFolderFromFullPath("/data/res/map/" + mapName).Files[lastEncountConfigFile].ByteContent = encountConfig.Save();
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
                .Select(x => x.ToClass<ShopConfig>())
                .ToArray();

            return (shopConfig, null);
        }

        public void SaveShop(string shopName, IShopConfig[] shopConfigs, IShopValidCondition[] shopValidConditions)
        {
            ShopConfig[] formatShopConfigs = shopConfigs.OfType<ShopConfig>().ToArray();

            VirtualDirectory shopFolder = Game.Directory.GetFolderFromFullPath("data/res/shop");
            string lastShopFile = shopFolder.Files.Keys.Where(x => x.StartsWith("shop_") && x.Contains(shopName)).OrderByDescending(x => x).First();

            CfgBin shopCfgBin = new CfgBin();
            shopCfgBin.Open(Game.Directory.GetFileFromFullPath("/data/res/shop/" + lastShopFile));

            shopCfgBin.ReplaceEntry("SHOP_CONFIG_INFO_BEGIN", "SHOP_CONFIG_INFO_", formatShopConfigs);

            Game.Directory.GetFolderFromFullPath("/data/res/shop/").Files[lastShopFile].ByteContent = shopCfgBin.Save();
        }

        public string[] GetMapWhoContainsTreasureBoxes()
        {
            VirtualDirectory mapEncounterFolder = Game.Directory.GetFolderFromFullPath("/data/res/map");

            return mapEncounterFolder.Folders
                    .Where(folder =>
                        folder.Files.Any(file =>
                        file.Key.StartsWith(folder.Name + "_tbox")))
                    .Select(folder => folder.Name)
                    .ToArray();
        }

        public IItableDataMore[] GetTreasureBox(string mapName)
        {
            VirtualDirectory mapFolder = Game.Directory.GetFolderFromFullPath(Files["map_encounter"].Path);
            string lastTboxConfigFile = mapFolder.GetFolder(mapName).Files.Keys.Where(x => x.StartsWith(mapName + "_tbox")).OrderByDescending(x => x).First();

            CfgBin tboxConfig = new CfgBin();
            tboxConfig.Open(Game.Directory.GetFileFromFullPath(Files["map_encounter"].Path + "/" + mapName + "/" + lastTboxConfigFile));

            return tboxConfig.Entries
                .Where(x => x.GetName() == "ITABLE_DATA_MORE_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<ItableDataMore>())
                .ToArray();
        }

        public void SaveTreasureBox(string mapName, IItableDataMore[] itableDataMores)
        {
            ItableDataMore[] formatItableDataMores = itableDataMores.OfType<ItableDataMore>().ToArray();

            VirtualDirectory mapFolder = Game.Directory.GetFolderFromFullPath(Files["map_encounter"].Path);
            string lastTboxConfigFile = mapFolder.GetFolder(mapName).Files.Keys.Where(x => x.StartsWith(mapName + "_tbox")).OrderByDescending(x => x).First();

            CfgBin tboxConfig = new CfgBin();
            tboxConfig.Open(Game.Directory.GetFileFromFullPath(Files["map_encounter"].Path + "/" + mapName + "/" + lastTboxConfigFile));

            tboxConfig.ReplaceEntry("ITABLE_DATA_MORE_BEGIN", "ITABLE_DATA_MORE_", formatItableDataMores);

            Game.Directory.GetFolderFromFullPath("/data/res/map/" + mapName).Files[lastTboxConfigFile].ByteContent = tboxConfig.Save();
        }

        public ICapsuleConfig[] GetCapsuleConfigs()
        {
            VirtualDirectory capsuleFolder = Game.Directory.GetFolderFromFullPath("data/res/capsule");
            string lastCapsuleConfig = capsuleFolder.Files.Keys.Where(x => x.StartsWith("capsule_config")).OrderByDescending(x => x).First();

            CfgBin capsuleConfigFile = new CfgBin();
            capsuleConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/capsule/" + lastCapsuleConfig));

            return capsuleConfigFile.Entries
                .Where(x => x.GetName() == "CAPSULE_CONFIG_INFO_BEGIN")
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

            capsuleConfigFile.ReplaceEntry("CAPSULE_CONFIG_INFO_BEGIN", "CAPSULE_CONFIG_INFO_", formatCapsuleConfigs);

            Game.Directory.GetFolderFromFullPath("/data/res/capsule").Files[lastCapsuleConfig].ByteContent = capsuleConfigFile.Save();
        }

        public ILegendDataConfig[] GetLegends()
        {
            VirtualDirectory legendFolder = Game.Directory.GetFolderFromFullPath("data/res/legend");
            string lastLegendConfig = legendFolder.Files.Keys.Where(x => x.StartsWith("legend_config")).OrderByDescending(x => x).First();

            CfgBin legendConfigFile = new CfgBin();
            legendConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/legend/" + lastLegendConfig));

            return legendConfigFile.Entries
                .Where(x => x.GetName() == "LEGEND_DATA_CONFIG_BEGIN")
                .SelectMany(x => x.Children)
                .Select(x => x.ToClass<LegendDataConfig>())
                .ToArray();
        }

        public void SaveLegends(ILegendDataConfig[] legendDataConfigs)
        {
            LegendDataConfig[] formatLegendConfigs = legendDataConfigs.OfType<LegendDataConfig>().ToArray();

            VirtualDirectory legendFolder = Game.Directory.GetFolderFromFullPath("data/res/legend");
            string lastLegendConfig = legendFolder.Files.Keys.Where(x => x.StartsWith("legend_config")).OrderByDescending(x => x).First();

            CfgBin legendConfigFile = new CfgBin();
            legendConfigFile.Open(Game.Directory.GetFileFromFullPath("/data/res/legend/" + lastLegendConfig));

            legendConfigFile.ReplaceEntry("LEGEND_DATA_CONFIG_BEGIN", "LEGEND_DATA_CONFIG_", formatLegendConfigs);

            Game.Directory.GetFolderFromFullPath("/data/res/legend").Files[lastLegendConfig].ByteContent = legendConfigFile.Save();
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
            Dictionary<int, int> yoCriminalsBaseHashes = new Dictionary<int, int>()
            {
                {unchecked((int)0xE81D47D4), unchecked((int)0xAAF99754)},
                {unchecked((int)0xF8EE5663), unchecked((int)0x83927ACE)},
                {unchecked((int)0x66BDBAB4), unchecked((int)0xB4EB0EC9)},
                {unchecked((int)0xC9FB64BA), unchecked((int)0xF82E4C60)},
                {unchecked((int)0x44DF4DB4), unchecked((int)0xD40AB77F)},
                {unchecked((int)0x72C3D7B1), unchecked((int)0xCBDA44D5)},
                {unchecked((int)0xC123F7E9), unchecked((int)0x4D80322D)},
                {unchecked((int)0x4F830A89), unchecked((int)0x5392ABB0)},
                {unchecked((int)0x5A6A501C), unchecked((int)0x60E577ED)},
                {unchecked((int)0x41905B31), unchecked((int)0x9B4B21B8)},
                {unchecked((int)0xF92C3C54), unchecked((int)0x9A894B8F)},
                {unchecked((int)0x65396EDA), unchecked((int)0x86DD6C4B)},
                {unchecked((int)0x75CA7F6D), unchecked((int)0xAFB681D1)},
                {unchecked((int)0x677FD083), unchecked((int)0xADF03F88)},
                {unchecked((int)0x82378ECF), unchecked((int)0x30107E45)},
                {unchecked((int)0x714703DF), unchecked((int)0xF9EC2657)},
                {unchecked((int)0xB7FA9D03), unchecked((int)0xA1C8ED58)},
                {unchecked((int)0x799F908C), unchecked((int)0x4C42581A)},
                {unchecked((int)0x451D2783), unchecked((int)0xCD11863E)},
                {unchecked((int)0xFE259488), unchecked((int)0xFEE58E8B)},
                {unchecked((int)0x5BA83A2B), unchecked((int)0x79FE46AC)},
                {unchecked((int)0x566AE25A), unchecked((int)0xD64C0926)},
                {unchecked((int)0x6076785F), unchecked((int)0xC99CFA8C)},
                {unchecked((int)0xF73F6DEC), unchecked((int)0x5250C187)},
                {unchecked((int)0x64AE594A), unchecked((int)0xCA9BFA0A)},
                {unchecked((int)0xFAA8E83A), unchecked((int)0xA8BF290D)},
                {unchecked((int)0x42148F5F), unchecked((int)0xA97D433A)},
                {unchecked((int)0x63F2AC31), unchecked((int)0xFBAA980E)},
                {unchecked((int)0X6230C606), unchecked((int)0XE2B1A94F)},
                {unchecked((int)0XFDA140E6), unchecked((int)0XCCD3EC09)},
                {unchecked((int)0XC0E19DDE), unchecked((int)0X549B036C)},
                {unchecked((int)0XE58AC202), unchecked((int)0X50167FDE)},
                {unchecked((int)0X48DFFFF2), unchecked((int)0X62A3C9B4)},
                {unchecked((int)0X491D95C5), unchecked((int)0X7BB8F8F5)},
                {unchecked((int)0X4A9941AB), unchecked((int)0X498E9A77)},
                {unchecked((int)0XCF30A651), unchecked((int)0X8559B825)},
                {unchecked((int)0X5D36A567), unchecked((int)0X26D3257F)},
                {unchecked((int)0XDE01DDD1), unchecked((int)0XB52964FE)},
                {unchecked((int)0X7301BD86), unchecked((int)0XD2C17594)},
                {unchecked((int)0XCCB4723F), unchecked((int)0XB76FDAA7)},
                {unchecked((int)0X5325F4DF), unchecked((int)0X0004CE5B)},
                {unchecked((int)0XD2543230), unchecked((int)0X56DDBD35)},
                {unchecked((int)0XEA5BF98D), unchecked((int)0X81D4C497)},
                {unchecked((int)0X778CC134), unchecked((int)0X849BD212)},
                {unchecked((int)0xFB6A820D), unchecked((int)0x28AD49F6)},
                {unchecked((int)0xED525151), unchecked((int)0xE5B80193)},
                {unchecked((int)0xEB9993BA), unchecked((int)0x98CFF5D6)},
                {unchecked((int)0xFFE7FEBF), unchecked((int)0x7EF7EE70)},
                {unchecked((int)0xE9DF2DE3), unchecked((int)0xB3E2A615)},
                {unchecked((int)0xEF14EF08), unchecked((int)0xCE955250)},
                {unchecked((int)0x81B35AA1), unchecked((int)0x02261CC7)},
                {unchecked((int)0x52E79EE8), unchecked((int)0x8016AEA0)},
            };

            List<int> yoBossBaseHashes = new List<int>()
            {
                unchecked((int)0xDCD2242C),
                unchecked((int)0x42B6B18F),
                unchecked((int)0x5B4A38E3),
                unchecked((int)0xC2436959),
                unchecked((int)0xB54459CF),
                unchecked((int)0x2B20CC6C),
                unchecked((int)0x5C27FCFA),
                unchecked((int)0xC52EAD40),
                unchecked((int)0xB2299DD6),
                unchecked((int)0x22968047),
                unchecked((int)0x5591B0D1),
                unchecked((int)0x35563934),
                unchecked((int)0x425109A2),
                unchecked((int)0xDC359C01),
                unchecked((int)0x9251D8DC),
                unchecked((int)0x0B588966),
                unchecked((int)0x0C354D7F),
                unchecked((int)0x7B327DE9),
                unchecked((int)0x152E7C3E),
                unchecked((int)0x62294CA8),
                unchecked((int)0xA067BA5E),
                unchecked((int)0x3E032FFD),
                unchecked((int)0xF63D1DD8),
                unchecked((int)0xEF262C99),
                unchecked((int)0x7142B93A),
                unchecked((int)0xC40B7F5A),
                unchecked((int)0xDD104E1B),
                unchecked((int)0x4374DBB8),
                unchecked((int)0x3473EB2E),
                unchecked((int)0xA4CCF6BF),
                unchecked((int)0x43936395),
                unchecked((int)0xDA9A322F),
                unchecked((int)0xAD9D02B9),
            };

            List<int> whiteYokaiParamHashes = new List<int>()
            {
                unchecked((int)0x910D0C8D),
                unchecked((int)0x08045D37),
                unchecked((int)0x7F036DA1),
                unchecked((int)0xE167F802),
                unchecked((int)0x9660C894),
                unchecked((int)0x0F69992E),
                unchecked((int)0x786EA9B8),
                unchecked((int)0xE8D1B429),
                unchecked((int)0x9FD684BF),
                unchecked((int)0xA94BAADC),
                unchecked((int)0xDE4C9A4A),
                unchecked((int)0x4745CBF0),
                unchecked((int)0x3042FB66),
            };

            // Get the index of the latest befriendable yokai
            int lastBefriendableIndex = charaparams.FindLastIndex(x =>
            {
                string tribeName = Tribes[x.Tribe];
                return ((x.ScoutableHash != 0x00 && x.ShowInMedalium == true)
                        || (x.ScoutableHash == 0x00 && x.ShowInMedalium == true && tribeName != "Boss" && tribeName != "Untribe"));
            }) + 1;

            int medalIndex = 248;

            foreach (KeyValuePair<int, int> keyValuePair in yoCriminalsBaseHashes)
            {
                uint newParamHash = Crc32.Compute(Encoding.GetEncoding("Shift-JIS").GetBytes("added_param_" + medalIndex));

                // Avoid to add already existing yokai
                if (!charaparams.Any(x => x.ParamHash == newParamHash))
                {
                    // Found yo-criminal and base yokai
                    if (charabases.Any(x => x.BaseHash == keyValuePair.Key) && charabases.Any(x => x.BaseHash == keyValuePair.Value))
                    {
                        // Search yo-criminal scale
                        if (charascales.Any(x => x.BaseHash != keyValuePair.Key) && charascales.Any(x => x.BaseHash == keyValuePair.Value))
                        {
                            ICharascale yokaiCriminalCharascale = (ICharascale)charascales.FirstOrDefault(x => x.BaseHash == keyValuePair.Value).Clone();
                            yokaiCriminalCharascale.BaseHash = keyValuePair.Key;
                            charascales.Add(yokaiCriminalCharascale);
                        }

                        ICharabase yokaiBaseCharabase = charabases.FirstOrDefault(x => x.BaseHash == keyValuePair.Value);
                        ICharabase yokaiCriminalCharabase = charabases.FirstOrDefault(x => x.BaseHash == keyValuePair.Key);

                        yokaiCriminalCharabase.Tribe = yokaiBaseCharabase.Tribe;
                        yokaiCriminalCharabase.Rank = yokaiBaseCharabase.Rank;
                        yokaiCriminalCharabase.FavoriteFoodHash = yokaiBaseCharabase.FavoriteFoodHash;
                        yokaiCriminalCharabase.HatedFoodHash = yokaiBaseCharabase.HatedFoodHash;
                        yokaiCriminalCharabase.MedalPosX = yokaiBaseCharabase.MedalPosX;
                        yokaiCriminalCharabase.MedalPosY = yokaiBaseCharabase.MedalPosY;
                        yokaiCriminalCharabase.DescriptionHash = yokaiBaseCharabase.DescriptionHash;

                        ICharaparam yokaiCriminalCharaparam = (ICharaparam)charaparams.FirstOrDefault(x => x.BaseHash == keyValuePair.Value).Clone();

                        yokaiCriminalCharaparam.BaseHash = keyValuePair.Key;
                        yokaiCriminalCharaparam.ParamHash = unchecked((int)newParamHash);
                        yokaiCriminalCharaparam.EvolveParam = 0x00;
                        yokaiCriminalCharaparam.EvolveLevel = 0;
                        yokaiCriminalCharaparam.MedaliumOffset = 0;

                        charaparams.Insert(lastBefriendableIndex, yokaiCriminalCharaparam);

                        medalIndex++;
                        lastBefriendableIndex++;
                    }
                }
            }

            foreach(int yoBossBaseHash in yoBossBaseHashes)
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
                        // yokaiBossCharascale.Scale2 /= 1.5f;
                        yokaiBossCharascale.Scale3 /= 2f;
                        yokaiBossCharascale.Scale4 /= 2f;
                        yokaiBossCharascale.Scale5 /= 1.1f;
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
                    charabases.Add(yokaiBossCharabase);
                }

                if (!charaparams.Any(x => x.ParamHash == newParamHash))
                {
                    // Search if the boss exists
                    if (charabases.Any(x => x.BaseHash == yoBossBaseHash))
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
                        yokaiBossCharaparam.MedaliumOffset = 0;
                        yokaiBossCharaparam.ShowInMedalium = true;
                        yokaiBossCharaparam.ScoutableHash = 0x13345632;
                        yokaiBossCharaparam.Experience = 5;
                        yokaiBossCharaparam.Money = 5;
                        yokaiBossCharaparam.Tribe = 6;

                        charaparams.Insert(lastBefriendableIndex, yokaiBossCharaparam);

                        medalIndex++;
                        lastBefriendableIndex++;
                    }
                }
            }

            foreach (int whiteYokaiParamHash in whiteYokaiParamHashes)
            {
                // Search if the boss exists
                if (charaparams.Any(x => x.ParamHash == whiteYokaiParamHash))
                {
                    ICharaparam whiteYokaiCharaparam = charaparams.FirstOrDefault(x => x.ParamHash == whiteYokaiParamHash);
                    whiteYokaiCharaparam.ShowInMedalium = true;
                    whiteYokaiCharaparam.MedaliumOffset = 0;
                    whiteYokaiCharaparam.ScoutableHash = 0x13345632;
                }
            }
        }

        public void FixYokai(List<ICharabase> charabases)
        {
            ICharabase dulluma = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0x871F067C));
            if (dulluma != null)
            {
                dulluma.Rank = 0;
            }

            ICharabase mochismo = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0xAFB681D1));
            if (mochismo != null)
            {
                if (mochismo.Rank > 0x01)
                {
                    mochismo.Rank = 0x01;
                }
            }

            ICharabase buhu = charabases.FirstOrDefault(x => x.BaseHash == unchecked((int)0xADF03F88));
            if (buhu != null)
            {
                buhu.Rank = 0;
            }
        }
    }
}
