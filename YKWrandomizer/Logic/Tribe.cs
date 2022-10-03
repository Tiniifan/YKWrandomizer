namespace YKWrandomizer.Logic
{
    public class Tribe
    {
        public string Name;

        public short ID;

        public Tribe(string _Name, short _ID)
        {
            Name = _Name;
            ID = _ID;
        }

        public static Tribe NoTribe()
        {
            return new Tribe("No Tribe", 0x00);
        }

        public static Tribe Brave()
        {
            return new Tribe("Brave", 0x01);
        }

        public static Tribe Mysterious()
        {
            return new Tribe("Mysterious", 0x02);
        }

        public static Tribe Tough()
        {
            return new Tribe("Tough", 0x03);
        }

        public static Tribe Charming()
        {
            return new Tribe("Charming", 0x04);
        }

        public static Tribe Heartful()
        {
            return new Tribe("Heartful", 0x05);
        }

        public static Tribe Shady()
        {
            return new Tribe("Shady", 0x06);
        }

        public static Tribe Eerie()
        {
            return new Tribe("Eerie", 0x07);
        }

        public static Tribe Slippery()
        {
            return new Tribe("Slippery", 0x08);
        }

        public static Tribe Wicked()
        {
            return new Tribe("Wicked", 0x09);
        }
    }
}
