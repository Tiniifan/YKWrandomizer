using System.Windows.Forms;
using System.Collections.Generic;

namespace YKWrandomizer.Tool
{
    public class Option
    {
        public string Name { get; set; }

        public Dictionary<string, CheckBox> CheckBoxes { get; set; }

        public Dictionary<string, NumericUpDown> NumericUpDowns { get; set; }

        public Option(List<RadioButton> radioButtons)
        {
            List<string> names = new List<string>();
            int index = 0;

            if (radioButtons.Count == 0)
            {
                names = new List<string>() { "NoRadioButton" };
            }
            else if (radioButtons.Count == 2)
            {
                names = new List<string>() { "Unchanged", "Random" };
                index = radioButtons.FindIndex(x => x.Checked == true);
            }
            else
            {
                names = new List<string>() { "Unchanged", "Swap", "Random" };
                index = radioButtons.FindIndex(x => x.Checked == true);
            }

            Name = names[index];
        }
    }
}
