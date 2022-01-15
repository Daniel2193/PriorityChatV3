using System.Net;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PriorityChatV3
{
    public partial class FormChat : Form
    {
        private struct UdpState
        {
            public IPEndPoint EndPoint;
            public UdpClient UdpClient;
        }
        public const string version = "2.6.0";
        private readonly string[] changelog =
        {
            "Upcoming/Planned Features:",
            "",
            "",
            "Add Emote Menu",
            "",
            "",
            "Actual changes:",
            "",
            "v3.0.0: Slight code improvements/optimizations for .NET 5",
            "v3.0.0: Added Username Color",
            "v3.0.0: Added Emote Manager",
            "v3.0.0: Full rework in .NET 5",
            "v2.5.1: Fixed UI Scaling",
            "v2.5.0: Added Userlist/Userstatus",
            "v2.5.0: Code cleanup",
            "v2.5.0: Added external auto/self updateing with version history",
            "v2.5.0: Changed message transmition protocol",
            "v2.5.0: Added Message colors to settings",
            "v2.5.0: Fixed Colored Messages",
            "v2.5.0: Changed the way the chat is displayed",
            "v2.5.0: Added support for emotes",
            "v2.5.0: Moved \"Send on Enter\" to the settings window",
            "v2.4.1: Fixed \"Send on Enter\" scaling",
            "v2.4.0: slight changelog upgrade (will be changed)",
            "v2.4.0: Added \"Send message on ENTER\" feature",
            "v2.4.0: Settings window will close automatically after clicking on \"apply\"",
            "v2.4.0: Added message timestamps",
            "v2.4.0: Added adaptive UI scaling",
            "v2.3.7: Removed DLL dependency by shipping it inside the exe file",
            "v2.3.6: Quit button works again",
            "v2.3.6: Fixed usernames not showing up in chat",
            "v2.3.5: Updated Changelog",
            "v2.3.4: Added auto-scroll to chat",
            "v2.3.3: Fixed a bug where messages could not be displayed",
            "v2.3.2: (temporary) removed colored messages",
            "v2.3.1: Flash now includes Windows internal flash system",
            "v2.3.0: Added Changelog",
            "v2.3.0: Added Colors to Messages",
            "v2.3.0: Structural Rework",
            "v2.3.0: Flash works again",
        };
        private readonly FormSettings formSettings = new();
        private readonly FormEmotes formEmotes = new();
        private bool expanded = false;
        private const int expandedDiff = 277;
        public FormChat()
        {
            InitializeComponent();
        }
        private void FormChat_Load(object sender, EventArgs e)
        {
            ConfigManager.Setup();
            UserManager.Setup();
            NetworkManager.Setup();
            EmoteManager.LoadEmotes();
            Text = "PriorityChatV" + version;
            StartListen();
            FormChat_Resize(null, null);
        }
        public void StartListen(int port = 21930){
            IPEndPoint ep = new(IPAddress.Any, port);
            UdpClient listener = new(ep);
            UdpState state = new(){EndPoint = ep, UdpClient = listener};
            listener.BeginReceive(new AsyncCallback(ReceiveCallback), state);
        }
        public void ReceiveCallback(IAsyncResult res){
            UdpState state = (UdpState)res.AsyncState;
            IPEndPoint ep = state.EndPoint;
            UdpClient listener = state.UdpClient;
            byte[] data = listener.EndReceive(res, ref ep);
            string msg = Encoding.ASCII.GetString(data);
                string[] split = Regex.Split(msg, Regex.Escape(Consts.msgSeperator));
                if(split.Length == 2){
                    switch(split[0]){
                        case "msg":
                            ChatMessage message = JsonSerializer.Deserialize<ChatMessage>(split[1]);
                            WriteMessage(message);
                            break;
                        case "update":
                            User user = JsonSerializer.Deserialize<User>(split[1]);
                            UserManager.UpdateUser(user);
                            UpdateUserList();
                            break;
                    }
                }
            listener.BeginReceive(new AsyncCallback(ReceiveCallback), state);
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            ConfigManager.SaveConfig();
            UserManager.UpdateStatus(UserManager.GetCurrentUser().Username, Status.OFFLINE);
            Application.Exit();
            Environment.Exit(0);
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            ChatMessage message = new(textBox1.Text, ConfigManager.GetConfig().Username);
            if (!NetworkManager.SendMessage(message))
            {
                MessageBox.Show("Message could not be send");
            }
            textBox1.Text = "";
            button2.Enabled = true;
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            formSettings.Show();
        }
        public void WriteMessage(ChatMessage message)
        {
            Color color = UserManager.GetUser(message.Sender).Color;
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.Select(richTextBox1.TextLength, 0);
                richTextBox1.SelectionColor = Color.White;
                richTextBox1.AppendText(message.Time.ToString("HH:mm:ss") + " ");
                richTextBox1.SelectionColor = color == Color.Empty ? Color.Blue : color;
                richTextBox1.AppendText(message.Sender);
                richTextBox1.SelectionColor = Color.White;
                richTextBox1.AppendText(": ");
                richTextBox1.SelectionColor = ConfigManager.GetConfig().ColorMessages;
                richTextBox1.AppendText(message.Message + "\n");
                richTextBox1.Rtf = ProcessRTF(richTextBox1.Rtf);
                richTextBox1.ScrollToCaret();
                if (ConfigManager.GetConfig().ShowNotifications && !message.Sender.Equals(ConfigManager.GetConfig().Username))
                    FlashWindowEx(this);
            });
        }
        private static string ProcessRTF(string msg)
        {
            string[] emotes = EmoteManager.GetEmotes();
            foreach (string emote in emotes)
            {
                Bitmap bmp = EmoteManager.GetEmote(emote);
                bmp.MakeTransparent(bmp.GetPixel(0, 0));
                var stream = new MemoryStream();
                byte[] data = File.ReadAllBytes(EmoteManager.GetEmotePath(emote));
                stream.Write(data, 0, data.Length);
                msg = msg.Replace(emote, @"{\pict\pngblip\picw" + bmp.Width * 15 + "\\pich" + bmp.Height * 15 + "\\picwgoal" + 3 * ConfigManager.GetConfig().EmoteScale + "\\pichgoal" + 3 * ConfigManager.GetConfig().EmoteScale + " " + BitConverter.ToString(stream.ToArray()).Replace("-", "") + "}");
            }
            return msg;
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new();
            foreach (string s in changelog)
            {
                sb.Append(s + "\n");
            }
            MessageBox.Show(sb.ToString(), "Changelog");
        }
        private readonly Color[] statusColors = new Color[] {Color.Green, Color.Yellow, Color.Black, Color.Red};
        private void UpdateUserList()
        {
            Invoke((MethodInvoker)delegate
            {
                richTextBox2.Clear();
                richTextBox2.Select(richTextBox2.TextLength, 0);
                foreach (User user in UserManager.GetAllUsers())
                {
                    richTextBox2.SelectionColor = statusColors[(int)user.Status];
                    richTextBox2.AppendText(user.Username + "\n");
                }
            });
        }
        #region flashWindow
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        public const uint FLASHW_ALL = 3;
        public const uint FLASHW_TIMERNOFG = 12;
        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }
        public static bool FlashWindowEx(Form form)
        {
            IntPtr hWnd = form.Handle;
            FLASHWINFO fInfo = new();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.uCount = uint.MaxValue;
            fInfo.dwTimeout = 0;
            return FlashWindowEx(ref fInfo);
        }
        #endregion
        private void FormChat_Resize(object sender, EventArgs e)
        {
            int width = Width;
            if(expanded)
                width -= expandedDiff;
            richTextBox1.Size = new Size(width - (Consts.globalOffset * 3), Height - textBox1.Height - button3.Height - Consts.globalOffset * 6);
            richTextBox2.Location = new Point(width - Consts.globalOffset, richTextBox2.Location.Y);
            
            label1.Location = new Point(width + label1.Width, label1.Location.Y);
            button5.Location = new Point(width - Consts.globalOffset * 2 - button5.Width, Height - (int)(Consts.globalOffset * 3.7f) - button5.Height);
            textBox1.Location = new Point(textBox1.Location.X, Height - textBox1.Height - button3.Height - (int)(Consts.globalOffset * 4.3f));
            textBox1.Size = new Size(width - button2.Width - Consts.globalOffset * 4, textBox1.Height);
            button2.Location = new Point(width - button2.Width - Consts.globalOffset * 2, textBox1.Location.Y);
            richTextBox2.Size = new Size(richTextBox2.Width, button2.Bottom - richTextBox2.Top);
            button1.Location = new Point(button1.Location.X, button5.Location.Y);
            button3.Location = new Point(button3.Location.X, button5.Location.Y);
            button4.Location = new Point(button4.Location.X, button5.Location.Y);
            button6.Location = new Point(width - Consts.globalOffset, button5.Location.Y);
            button7.Location = new Point(width - (int)(Consts.globalOffset * 0.5f) + button6.Width, button5.Location.Y);
            button8.Location = new Point(button8.Location.X, button5.Location.Y);
        }
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (ConfigManager.GetConfig().SendOnEnter && e.KeyCode == Keys.Enter && !e.Shift)
            {
                Button2_Click(null, null);
                e.SuppressKeyPress = true;
            }
            if(e.KeyCode == Keys.E && e.Control)
            {
                e.SuppressKeyPress = true;
            }
        }
        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Button1_Click(null, null);
            e.Cancel = true;
        }
        private void Button5_Click(object sender, EventArgs e)
        {
            expanded = !expanded;
            if (expanded)
            {
                Width += expandedDiff;
                button5.Text = "<<";
            }
            else
            {
                Width -= expandedDiff;
                button5.Text = ">>";
            }
        }
        private void RichTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private void Button6_Click(object sender, EventArgs e)
        {
            UserManager.UpdateStatus(Status.AFK);
            NetworkManager.SendUpdate();
        }
        private void Button7_Click(object sender, EventArgs e)
        {
            UserManager.UpdateStatus(Status.ONLINE);
            NetworkManager.SendUpdate();
        }
        private void Button8_Click(object sender, EventArgs e)
        {
            formEmotes.Show();
        }
    }
}
