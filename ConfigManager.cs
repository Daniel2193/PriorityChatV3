using System;
using System.IO;
using System.Text.Json;
using System.Drawing;
using System.Windows.Forms;

namespace PriorityChatV3
{
    static class ConfigManager
    {
        private static Config config = null;
        private static readonly JsonSerializerOptions options = new() { WriteIndented = true };
        public static readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + "Daniel2193" + Path.DirectorySeparatorChar + "PriorityChat" + Path.DirectorySeparatorChar;
        public static readonly string pathConfig = path + "config.json";
        public static Config GetConfig()
        {
            return config;
        }
        public static void SaveConfig()
        {
            string jsonConfig = JsonSerializer.Serialize(config, options);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(pathConfig, jsonConfig);
        }
        public static void Setup()
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                File.WriteAllText(path + ".version.bin", FormChat.version);
                if (File.Exists(pathConfig))
                {
                    string jsonConfig = File.ReadAllText(pathConfig);
                    config = JsonSerializer.Deserialize<Config>(jsonConfig, options);
                    config.ColorMessages = Color.FromArgb(config.ColorMessagesR, config.ColorMessagesG, config.ColorMessagesB);
                    config.ColorMessagesRead = Color.FromArgb(config.ColorMessagesReadR, config.ColorMessagesReadG, config.ColorMessagesReadB);
                    config.ColorUsername = Color.FromArgb(config.ColorUsernameR, config.ColorUsernameG, config.ColorUsernameB);
                }
                else
                {
                    config = new Config();
                    SaveConfig();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in setup: \n" + e.Message);
            }
        }
        public static void UpdateConfig(string ip, int port, string username, bool showNotifications, Color colorMessages, Color colorMessagesRead, Color colorUsername, bool sendOnEnter, int emoteScale)
        {
            config.Ip = ip;
            config.Port = port;
            config.Username = username;
            config.ShowNotifications = showNotifications;
            config.ColorMessages = colorMessages;
            config.ColorMessagesRead = colorMessagesRead;
            config.ColorUsername = colorUsername;
            config.SendOnEnter = sendOnEnter;
            config.EmoteScale = emoteScale;
            config.ColorMessagesR = colorMessages.R;
            config.ColorMessagesG = colorMessages.G;
            config.ColorMessagesB = colorMessages.B;
            config.ColorMessagesReadR = colorMessagesRead.R;
            config.ColorMessagesReadG = colorMessagesRead.G;
            config.ColorMessagesReadB = colorMessagesRead.B;
            config.ColorUsernameR = colorUsername.R;
            config.ColorUsernameG = colorUsername.G;
            config.ColorUsernameB = colorUsername.B;
            UserManager.UpdateColor(username, colorUsername);
            SaveConfig();
            NetworkManager.SendUpdate();
        }
    }
}
