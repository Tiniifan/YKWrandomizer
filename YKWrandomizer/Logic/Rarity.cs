namespace YKWrandomizer.Logic
{
    public class Rarity
    {
        public string Name;

        public short ID;

        public Rarity(string _Name, short _ID)
        {
            Name = _Name;
            ID = _ID;
        }

        public static Rarity Normal()
        {
            return new Rarity("E", 0x00);
        }

        public static Rarity Rare()
        {
            return new Rarity("D", 0x01);
        }
    }
}
