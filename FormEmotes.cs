using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;

namespace PriorityChatV3
{
    public partial class FormEmotes : Form
    {
        private readonly FormDownload downloadProgress = new();
        private const string fileName = "EmoteDownloader.exe";
        private readonly string filePath = ConfigManager.path + fileName;
        public FormEmotes()
        {
            InitializeComponent();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Hide();
        }
        private async void Button2_Click(object sender, EventArgs e)
        {
            if (!File.Exists(filePath))
                if (!await DownloadEmoteDownloader())
                    return;
            Process p = new();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = @"cmd.exe",
                Arguments = "/c \"" + filePath + " " + GenerateDownloaderArgs() + "\"",
                UseShellExecute = false
            };
            MessageBox.Show(p.StartInfo.Arguments);
            p.EnableRaisingEvents = true;
            p.Exited += P_Exited;
            p.Start();
        }
        private async Task<bool> DownloadEmoteDownloader()
        {
            DialogResult result = MessageBox.Show(fileName + " not found.\nDo you want to download it now?", fileName + " not found", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using var client = new WebClient();
                string url = await GetDownloadURL();
                if (url is null)
                {
                    MessageBox.Show("Could not get download URL.\nPlease make sure your internet connection is stable.");
                    return false;
                }
                MessageBox.Show(url);
                using WebClient wc = new();
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                downloadProgress.Show();
                await wc.DownloadFileTaskAsync(new Uri(url), filePath);
                downloadProgress.Hide();
                return true;
            }
            else
                return false;
        }
        private string GenerateDownloaderArgs()
        {
            string args = "";
            args += "-p " + comboBox1.SelectedItem.ToString();
            if (radioButton1.Checked)
                args += " --channel_names " + textBox1.Text;
            if (radioButton2.Checked)
                args += " --channel_ids " + textBox2.Text;
            if (panel1.Visible)
            {
                args += " --client_id " + textBox3.Text;
                if (radioButton3.Checked)
                {
                    args += " --client_secret " + textBox4.Text;
                }
                else if (radioButton4.Checked)
                {
                    args += " -t " + textBox5.Text;
                }
            }
            args += " --output_dir " + ConfigManager.path + "emotes" + Path.DirectorySeparatorChar;
            return args;
        }
        private void P_Exited(object sender, EventArgs e)
        {
            int exitCode = ((Process)sender).ExitCode;
            if (exitCode == 0)
                MessageBox.Show("Emotes successfully downloaded.");
            else
                MessageBox.Show("Error while downloading emotes.\nError code: " + exitCode);
        }
        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgress.SetProgress(e.ProgressPercentage);
        }
        private void FormEmotes_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            foreach (string emote in EmoteManager.GetEmotes())
            {
                textBox6.Text += emote + Environment.NewLine;
            }
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            EmoteManager.LoadEmotes();
        }
        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Enabled = radioButton3.Checked;
        }
        private void RadioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.Enabled = radioButton4.Checked;
        }
        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = radioButton1.Checked;
            panel1.Visible = radioButton1.Checked || comboBox1.SelectedIndex == 0;
        }
        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = radioButton2.Checked;
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Visible = radioButton1.Checked || comboBox1.SelectedIndex == 0;
        }
        private static async Task<string> GetDownloadURL()
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/Daniel2193/EmoteDownloader/releases/latest");
            request.Headers.Add("User-Agent", "Update-Tool");
            var response = await client.SendAsync(request);
            string json = await response.Content.ReadAsStringAsync();
            var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
            foreach (var asset in jsonObj["assets"])
            {
                if (asset.name == "EmoteDownloader.exe")
                    return asset.browser_download_url;
            }
            return null;
        }
    }
}
