namespace YKWrandomizer.Logic
{
    public class Rank
    {
        public string Name;

        public short ID;

        public Rank(string _Name, short _ID)
        {
            Name = _Name;
            ID = _ID;
        }

        public static Rank E()
        {
            return new Rank("E", 0x00);
        }

        public static Rank D()
        {
            return new Rank("D", 0x01);
        }

        public static Rank C()
        {
            return new Rank("C", 0x02);
        }

        public static Rank B()
        {
            return new Rank("B", 0x03);
        }

        public static Rank A()
        {
            return new Rank("A", 0x04);
        }

        public static Rank S()
        {
            return new Rank("S", 0x05);
        }
    }
}
