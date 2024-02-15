
namespace YKWrandomizer
{
    partial class NewProjectWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.settingGroupBox = new System.Windows.Forms.GroupBox();
            this.romfsTextBox = new System.Windows.Forms.TextBox();
            this.gameComboBox = new System.Windows.Forms.ComboBox();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.openButton = new System.Windows.Forms.Button();
            this.settingGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingGroupBox
            // 
            this.settingGroupBox.Controls.Add(this.romfsTextBox);
            this.settingGroupBox.Controls.Add(this.gameComboBox);
            this.settingGroupBox.Controls.Add(this.languageComboBox);
            this.settingGroupBox.Controls.Add(this.label4);
            this.settingGroupBox.Controls.Add(this.label3);
            this.settingGroupBox.Controls.Add(this.label2);
            this.settingGroupBox.ForeColor = System.Drawing.Color.Black;
            this.settingGroupBox.Location = new System.Drawing.Point(12, 12);
            this.settingGroupBox.Name = "settingGroupBox";
            this.settingGroupBox.Size = new System.Drawing.Size(341, 113);
            this.settingGroupBox.TabIndex = 3;
            this.settingGroupBox.TabStop = false;
            this.settingGroupBox.Text = "Game Settings";
            // 
            // romfsTextBox
            // 
            this.romfsTextBox.Location = new System.Drawing.Point(129, 83);
            this.romfsTextBox.Name = "romfsTextBox";
            this.romfsTextBox.Size = new System.Drawing.Size(206, 20);
            this.romfsTextBox.TabIndex = 39;
            this.romfsTextBox.Click += new System.EventHandler(this.RomfsTextBox_Click);
            // 
            // gameComboBox
            // 
            this.gameComboBox.FormattingEnabled = true;
            this.gameComboBox.Items.AddRange(new object[] {
            "Yo-Kai Watch 1",
            "Yo-Kai Watch 2",
            "Yo-Kai Watch 3"});
            this.gameComboBox.Location = new System.Drawing.Point(129, 25);
            this.gameComboBox.Name = "gameComboBox";
            this.gameComboBox.Size = new System.Drawing.Size(206, 21);
            this.gameComboBox.TabIndex = 38;
            this.gameComboBox.SelectedIndexChanged += new System.EventHandler(this.GameComboBox_SelectedIndexChanged);
            // 
            // languageComboBox
            // 
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Location = new System.Drawing.Point(129, 55);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(206, 21);
            this.languageComboBox.TabIndex = 37;
            this.languageComboBox.SelectedIndexChanged += new System.EventHandler(this.LanguageComboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(16, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "ExtractedRomfs Path";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(17, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Language";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(16, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Game";
            // 
            // openButton
            // 
            this.openButton.Enabled = false;
            this.openButton.Location = new System.Drawing.Point(12, 131);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(341, 23);
            this.openButton.TabIndex = 4;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // NewProjectWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 163);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.settingGroupBox);
            this.Name = "NewProjectWindow";
            this.Text = "NewProjectWindow";
            this.settingGroupBox.ResumeLayout(false);
            this.settingGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox settingGroupBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox gameComboBox;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.TextBox romfsTextBox;
    }
}