using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

namespace PriorityChatV3
{
    public partial class FormSettings : Form
    {
        private Color colorMessages;
        private Color colorMessagesRead;
        private Color colorUsername;
        public FormSettings()
        {
            InitializeComponent();
        }
        private void FormSettings_Load(object sender, EventArgs e)
        {
            textBox1.Text = ConfigManager.GetConfig().Ip;
            textBox2.Text = ConfigManager.GetConfig().Port.ToString();
            textBox3.Text = ConfigManager.GetConfig().Username;
            checkBox1.Checked = ConfigManager.GetConfig().ShowNotifications;
            checkBox2.Checked = ConfigManager.GetConfig().SendOnEnter;
            numericUpDown1.Value = ConfigManager.GetConfig().EmoteScale;
            colorMessages = ConfigManager.GetConfig().ColorMessages;
            colorMessagesRead = ConfigManager.GetConfig().ColorMessagesRead;
            colorUsername = ConfigManager.GetConfig().ColorUsername;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if(CheckConfig()){
                ConfigManager.UpdateConfig(textBox1.Text, int.Parse(textBox2.Text), textBox3.Text, checkBox1.Checked, colorMessages, colorMessagesRead, colorUsername, checkBox2.Checked, (int)numericUpDown1.Value);
                Hide();
            }
        }
        private bool CheckConfig(){
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Please fill in all fields");
                return false;
            }
            if (!Regex.IsMatch(textBox1.Text, @"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$"))
            {
                MessageBox.Show("Please enter a valid IP");
                return false;
            }
            if (!Regex.IsMatch(textBox2.Text, @"^[0-9]{1,5}$"))
            {
                MessageBox.Show("Please enter a valid Port");
                return false;
            }
            if (textBox3.Text.Length > 30 || textBox3.Text.Length < 3 || !Regex.IsMatch(textBox3.Text, @"^[a-zA-Z0-9_]*$"))
            {
                MessageBox.Show("Please enter a valid Username");
                return false;
            }
            if(numericUpDown1.Value < 10 || numericUpDown1.Value > 400){
                MessageBox.Show("Please enter a valid Scale factor");
                return false;
            }
            return true;
        }
        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new();
            if (cd.ShowDialog() == DialogResult.OK)
                colorMessages = cd.Color;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                colorMessagesRead = cd.Color;
            }
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            EmoteManager.LoadEmotes();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                colorUsername = cd.Color;
            }
        }
    }
}
