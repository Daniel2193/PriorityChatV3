using System.Drawing;

namespace PriorityChatV3
{
    public class Config
    {
        public string Ip { get; set; } = "192.168.191.248";
        public int Port { get; set; } = 21930;
        public string Username { get; set; } = "user";
        public bool ShowNotifications { get; set; } = false;
        public int ColorMessagesR { get; set; } = 255;
        public int ColorMessagesG { get; set; } = 255;
        public int ColorMessagesB { get; set; } = 255;
        public Color ColorMessages { get; set; } = Color.FromArgb(255, 255, 255);
        public int ColorMessagesReadR { get; set; } = 150;
        public int ColorMessagesReadG { get; set; } = 150;
        public int ColorMessagesReadB { get; set; } = 150;
        public Color ColorMessagesRead { get; set; } = Color.FromArgb(150, 150, 150);
        public int ColorUsernameR { get; set; } = 0;
        public int ColorUsernameG { get; set; } = 0;
        public int ColorUsernameB { get; set; } = 255;
        public Color ColorUsername { get; set; } = Color.FromArgb(0, 0, 255);
        public bool SendOnEnter { get; set; } = false;
        public int EmoteScale { get; set; } = 100;
    }
}
