using System.Collections.Generic;

namespace YKWrandomizer.Level5.Text.Logic
{
    public class TextConfig
    {
        public int WashaID;

        public List<StringLevel5> Strings;

        public TextConfig()
        {

        }

        public TextConfig(List<StringLevel5> strings, int washaID = 0)
        {
            WashaID = washaID;
            Strings = strings;
        }
    }

    public class StringLevel5
    {
        public int Variance;

        public string Text;

        public StringLevel5()
        {

        }

        public StringLevel5(int variance, string text)
        {
            Variance = variance;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
