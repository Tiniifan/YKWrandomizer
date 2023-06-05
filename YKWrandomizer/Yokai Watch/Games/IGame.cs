using System.Collections.Generic;
using YKWrandomizer.Yokai_Watch.Logic;
using YKWrandomizer.Level5.Archive.ARC0;

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

        Dictionary<uint, string> Items { get; }

        Dictionary<int, string> Tribes { get; }

        Dictionary<string, List<uint>> YokaiGiven { get; }

        List<uint> YokaiUnscoutableAutorized { get; }

        ARC0 Game { get; set; }

        List<Yokai> GetYokais();

        List<Evolution> GetEvolutions(List<Yokai> yokais);

        void SaveYokais(List<Yokai> yokais, List<Evolution> evolutions);

        List<LegendSeal> GetLegendaries();

        void SaveLegendaries(List<LegendSeal> legendaries, bool spoil);

        List<Fusion> GetFusions();

        void SaveFusions(List<Fusion> fusions);

        List<uint> GetSouls();

        void SaveSouls(List<uint> souls);

        List<(uint, int)> GetEncounter();

        void SaveEncounter(List<(uint, int)> encounters);

        List<(uint, int)> GetWorldEncounter(byte[] file);

        byte[] SaveWorldEncounter(List<(uint, int)> encounters, byte[] file);

        List<uint> GetCapsule();

        void SaveCapsule(List<uint> capsules);

        List<uint> GetShop(string fileName);

        void SaveShop(List<uint> capsules, string fileName);

        List<uint> GetTreasureBox(byte[] file);

        byte[] SaveTreasureBox(List<uint> treasures, byte[] file);

        void FixStory();

        void FixYokai(List<Yokai> yokais);
    }
}
