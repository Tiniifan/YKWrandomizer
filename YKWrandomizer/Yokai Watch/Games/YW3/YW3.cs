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

        public YW3(string path)
        {
            Game = new ARC0(new FileStream(path, FileMode.Open));
        }

        public List<Yokai> GetYokais()
        {
            return null;
        }

        public void SaveYokais(List<Yokai> yokais, List<Evolution> evolutions)
        {
        }

        public List<Evolution> GetEvolutions(List<Yokai> yokais)
        {
            return null;
        }

        public List<Fusion> GetFusions()
        {
            return null;
        }

        public void SaveFusions(List<Fusion> fusions)
        {
        }

        public List<LegendSeal> GetLegendaries()
        {
            return null;
        }

        public void SaveLegendaries(List<LegendSeal> legendaries, bool spoil)
        {
        }

        public List<(uint, int)> GetEncounter()
        {
            return null;
        }

        public void SaveEncounter(List<(uint, int)> encounters)
        {
        }

        public List<(uint, int)> GetWorldEncounter(byte[] file)
        {
            return null;
        }

        public byte[] SaveWorldEncounter(List<(uint, int)> encounters, byte[] file)
        {
            return null;
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
    }
}
