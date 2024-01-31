namespace YKWrandomizer.Level5.Binary.Logic
{
    public class OffsetTextPair
    {
        public int Offset { get; set; }
        public string Text { get; set; }

        public OffsetTextPair(int offset, string text)
        {
            Offset = offset;
            Text = text;
        }
    }
}