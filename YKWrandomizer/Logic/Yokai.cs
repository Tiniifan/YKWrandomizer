using YKWrandomizer.Yokai_Watch.Res;

namespace YKWrandomizer.Logic
{
    public class Yokai
    {
        public string Name;

        public ICharabase Charabase;

        public ICharaparam Charaparam;

        public bool IsBoss;

        public bool IsStatic;

        public bool IsScoutable;

        public Yokai()
        {

        }

        public Yokai(string name)
        {
            Name = name;
        }
    }
}
