using System;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Collections;

namespace YKWrandomizer.Yokai_Watch.Logic
{
    public class Yokai
    {
        public string Name { get; set; }

        public string ModelName { get; set; }

        public int Rank { get; set; }

        public int Tribe { get; set; }

        public int WaitTime { get; set; }

        public int[] MinStat { get; set; }

        public int[] MaxStat { get; set; }

        public float[] AttributeDamage { get; set; }

        public byte Strongest { get; set; }

        public byte Weakness { get; set; }

        public UInt32 AttackID { get; set; }

        public UInt32 TechniqueID { get; set; }

        public UInt32 InspiritID { get; set; }

        public UInt32 SoultimateID { get; set; }

        public UInt32 FoodID { get; set; }

        public UInt32 SkillID { get; set; }

        public int Money { get; set; }

        public int Experience { get; set; }

        public UInt32[] DropID { get; set; }

        public int[] DropRate { get; set; }

        public int ExperienceCurve { get; set; }

        public int EvolveOffset { get; set; }

        public int MedaliumOffset { get; set; }

        public Point Medal { get; set; }

        public Statut Statut { get; set; }

        public UInt32 ScoutableID { get; set; }

        public UInt32 BaseID { get; set; }

        public UInt32 ParamID { get; set; }

        public Yokai()
        {

        }

        public Yokai(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Statut
    {
        public bool IsRare { get; set; }

        public bool IsLegendary { get; set; }

        public bool IsClassic { get; set; }

        public bool IsMerican { get; set; }

        public bool IsDeva { get; set; }

        public bool IsMystery { get; set; }

        public bool IsTreasure { get; set; }

        public bool IsBoss { get; set; }

        public bool IsStatic { get; set; }

        public bool IsScoutable { get; set; }

        public Statut()
        {

        }

        public override string ToString()
        {
            Type objectType = typeof(Statut);
            PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(bool))
                {
                    bool value = (bool)property.GetValue(this);
                    sb.Append($"{property.Name}: {value}, ");
                }
            }

            if (sb.Length > 1)
            {
                sb.Length -= 2;
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}
