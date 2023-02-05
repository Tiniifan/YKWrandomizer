using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YKWrandomizer.Tool;
using YKWrandomizer.Logic;
using YKWrandomizer.Yokai_Watch.Games;
using YKWrandomizer.Yokai_Watch.Games.YW2;

namespace YKWrandomizer.Yokai_Watch.Games.YW3
{
    public class YW3 : Randomizer
    {
        public YW3(string romfsPath)
        {
            Name = "Yo-Kai Watch 3";
            NameCode = "YW3";
            TribeCount = 12;

            YokaiScoutable = Yokai_Watch.Common.YokaiScoutable.YW2;
            YokaiStatic = Yokai_Watch.Common.YokaiStatic.YW2;
            YokaiBoss = Yokai_Watch.Common.YokaiBoss.YW2;

            Attacks = Yokai_Watch.Common.Attacks.YW2;
            Techniques = Yokai_Watch.Common.Techniques.YW2;
            Inspirits = Yokai_Watch.Common.Inspirits.YW2;
            Soultimates = Yokai_Watch.Common.Soultimates.YW2;
            Skills = Yokai_Watch.Common.Skills.YW2;
            Items = Yokai_Watch.Common.Items.YW2;

            RomfsPath = romfsPath;

            LocalFiles = new Dictionary<string, LocalFile>();
            // LocalFiles.Add("charabase", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\character\chara_base_0.04c.cfg.bin"));
            // LocalFiles.Add("charaparam", new LocalFile(RomfsPath + @"\yw2_a_fa\data\res\character\chara_param_0.03a.cfg.bin"));
        }
    }
}
