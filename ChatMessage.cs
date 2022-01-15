using System;

namespace PriorityChatV3
{
    public class ChatMessage
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public DateTime Time { get; set; }
        public ChatMessage(string message, string sender)
        {
            Message = message;
            Sender = sender;
            Time = DateTime.Now;
        }
    }
}
