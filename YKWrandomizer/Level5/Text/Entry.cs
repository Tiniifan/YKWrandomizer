using System;
using System.Text;

namespace YKWrandomizer.Level5.Text
{
    public class Entry
    {
        public UInt32 Key;

        public int VariantKey;

        public string Text;

        public int Offset;

        public Entry(UInt32 key, byte[] text)
        {
            Key = key;
            VariantKey = 0;
            Text = Encoding.UTF8.GetString(text);
        }

        public Entry(UInt32 key, string text)
        {
            Key = key;
            VariantKey = 0;
            Text = text;
        }

        public Entry(UInt32 key, int variantKey, byte[] text)
        {
            Key = key;
            VariantKey = variantKey;
            Text = Encoding.UTF8.GetString(text);
        }
    }
}
