using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Drawing;
using Timer = System.Windows.Forms.Timer;

namespace PriorityChatV3
{
    static class NetworkManager
    {
        private static IPEndPoint serverEP;
        private static bool canSend = false;
        private static readonly Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp){ExclusiveAddressUse = false};
        public static void Setup()
        {
            try
            {
                serverEP = new IPEndPoint(IPAddress.Parse(ConfigManager.GetConfig().Ip), ConfigManager.GetConfig().Port);
                canSend = true;
                Timer timer = new(){Interval = 30000};
                timer.Tick += new EventHandler(delegate{SendUpdate();});
                timer.Start();
                SendUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        public static bool SendMessage(ChatMessage message)
        {
            if (canSend && message.Message.Length >= 1)
            {
                byte[] data = Encoding.ASCII.GetBytes("msg" + Consts.msgSeperator + JsonSerializer.Serialize(message));
                socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
                return true;
            }
            return false;
        }
        public static void SendUpdate(){
            if(canSend){
                byte[] data = Encoding.ASCII.GetBytes("update" + Consts.msgSeperator + JsonSerializer.Serialize(UserManager.GetCurrentUser()));
                socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
            }
        }
    }
}
