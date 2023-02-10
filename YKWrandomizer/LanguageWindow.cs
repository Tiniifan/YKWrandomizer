using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace YKWrandomizer
{
    public partial class LanguageWindow : Form
    {
        public Dictionary<string, string> Languages;

        public string SelectedLanguage { get; set; }

        public LanguageWindow(Dictionary<string, string> languages)
        {
            InitializeComponent();
            Languages = languages;          
        }

        private void LanguageWindow_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(Languages.Keys.ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a language");
            } else
            {
                SelectedLanguage = Languages[comboBox1.Items[comboBox1.SelectedIndex].ToString()];

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
