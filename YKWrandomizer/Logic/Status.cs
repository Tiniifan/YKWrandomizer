namespace YKWrandomizer.Logic
{
    public class Status
    {
        public string Name;

        public short ID;

        public Status(string _Name)
        {
            Name = _Name;
        }

        public static Status Normal()
        {
            return new Status("Normal");
        }

        public static Status MiniBoss()
        {
            return new Status("MiniBoss");
        }

        public static Status Boss()
        {
            return new Status("Boss");
        }

        public static Status Unused()
        {
            return new Status("Unused");
        }

        public static Status NPC()
        {
            return new Status("NPC");
        }
    }
}
