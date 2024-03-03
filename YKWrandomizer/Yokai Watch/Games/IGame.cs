using System.Collections.Generic;
using YKWrandomizer.Level5.Text;
using YKWrandomizer.Level5.Archive.ARC0;
using YKWrandomizer.Yokai_Watch.Logic;

namespace YKWrandomizer.Yokai_Watch.Games
{
    public interface IGame
    {
        string Name { get; }

        Dictionary<uint, string> Attacks { get; }

        Dictionary<uint, string> Techniques { get; }

        Dictionary<uint, string> Inspirits { get; }

        Dictionary<uint, string> Soultimates { get; }

        Dictionary<uint, string> Skills { get; }

        Dictionary<int, string> Tribes { get; }

        Dictionary<int, string> FoodsType { get; }

        Dictionary<int, string> ScoutablesType { get; }

        Dictionary<int, int> BossBattles { get; }

        (float, float, float, float)[] ActionPercentages { get; }

        Dictionary<uint, string> StaticYokais { get; }

        ARC0 Game { get; set; }

        ARC0 Language { get; set; }

        Dictionary<string, GameFile> Files { get; set; }

        void Save();

        ICharabase[] GetCharacterbase(bool isYokai);

        void SaveCharaBase(ICharabase[] charabases);

        ICharascale[] GetCharascale();

        void SaveCharascale(ICharascale[] charascales);

        ICharaparam[] GetCharaparam();

        void SaveCharaparam(ICharaparam[] charaparams);

        ICharaevolve[] GetCharaevolution();

        void SaveCharaevolution(ICharaevolve[] charaevolves);

        IItem[] GetItems(string itemType);

        ICharaabilityConfig[] GetAbilities();

        ISkillconfig[] GetSkills();

        IBattleCharaparam[] GetBattleCharaparam();

        void SaveBattleCharaparam(IBattleCharaparam[] battleCharaparams);

        IHackslashCharaparam[] GetHackslashCharaparam();

        void SaveHackslashCharaparam(IHackslashCharaparam[] hackslashCharaparams);

        IHackslashCharaabilityConfig[] GetHackslashAbilities();

        IHackslashTechnic[] GetHackslashSkills();

        IOrgetimeTechnic[] GetOrgetimeTechnics();

        IBattleCommand[] GetBattleCommands();

        string[] GetMapWhoContainsEncounter();

        (IEncountTable[], IEncountChara[]) GetMapEncounter(string mapName);

        void SaveMapEncounter(string mapName, IEncountTable[] encountTables, IEncountChara[] encountCharas);

        (IShopConfig[], IShopValidCondition[]) GetShop(string shopName);

        void SaveShop(string shopName, IShopConfig[] shopConfigs, IShopValidCondition[] shopValidConditions);

        string[] GetMapWhoContainsTreasureBoxes();

        IItableDataMore[] GetTreasureBox(string mapName);

        void SaveTreasureBox(string mapName, IItableDataMore[] itableDataMores);

        ICapsuleConfig[] GetCapsuleConfigs();

        void SaveCapsuleConfigs(ICapsuleConfig[] capsuleConfigs);

        ILegendDataConfig[] GetLegends();

        void SaveLegends(ILegendDataConfig[] legendDataConfigs);

        ICombineConfig[] GetFusions();

        void SaveFusions(ICombineConfig[] combineConfigs);

        (IEncountTable[], IEncountChara[]) GetStaticEncounters();

        void SaveStaticEncounters(IEncountTable[] encountTables, IEncountChara[] encountCharas);

        void UnlockUnscoutableYokai(List<ICharaparam> charaparams, List<ICharabase> charabases, List<ICharascale> charascales, List<IHackslashCharaparam> hackslashCharaparams = null, List<IBattleCharaparam> battleCharaparams = null, bool addFile = false);

        void FixYokai(List<ICharabase> charabases);

        void FixArea();

        void FixShop();

        void DebugMe();
    }
}
