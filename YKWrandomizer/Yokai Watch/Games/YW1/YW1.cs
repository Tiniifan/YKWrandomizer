using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YKWrandomizer.Tool;
using YKWrandomizer.Logic;
using YKWrandomizer.Yokai_Watch.Games;
using YKWrandomizer.Yokai_Watch.Games.YW2;

namespace YKWrandomizer.Yokai_Watch.Games.YW1
{
    public class YW1 : Randomizer
    {
        public YW1(string romfsPath)
        {
            Name = "Yo-Kai Watch 1";
            NameCode = "YW1";
            TribeCount = 8;

            YokaiScoutable = Yokai_Watch.Common.YokaiScoutable.YW1;
            YokaiStatic = Yokai_Watch.Common.YokaiStatic.YW1;
            YokaiBoss = Yokai_Watch.Common.YokaiBoss.YW1;

            Attacks = Yokai_Watch.Common.Attacks.YW1;
            Techniques = Yokai_Watch.Common.Techniques.YW1;
            Inspirits = Yokai_Watch.Common.Inspirits.YW1;
            Soultimates = Yokai_Watch.Common.Soultimates.YW1;
            Skills = Yokai_Watch.Common.Skills.YW1;
            Items = Yokai_Watch.Common.Items.YW1;

            RomfsPath = romfsPath;

            LocalFiles = new Dictionary<string, LocalFile>();
            LocalFiles.Add("charabase", new LocalFile(RomfsPath + @"\yw1_a_fa\data\res\character\chara_base_0.04j.cfg.bin"));
            LocalFiles.Add("charaparam", new LocalFile(RomfsPath + @"\yw1_a_fa\data\res\character\chara_param_0.02.cfg.bin"));
            LocalFiles.Add("combineconfig", new LocalFile(RomfsPath + @"\yw1_a_fa\data\res\shop\combine_config.cfg.bin"));
        }
    }
}
