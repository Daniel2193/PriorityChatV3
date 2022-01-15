using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PriorityChatV3
{
    static class EmoteManager
    {
        private static readonly Dictionary<string, string> emotes = new();
        public static void AddEmote(string emoteName, string filePath)
        {
            if(!emotes.ContainsKey(emoteName))
            {
                emotes.Add(emoteName, filePath);
            }
        }
        public static void LoadEmotes(){
            emotes.Clear();
            if(!Directory.Exists(ConfigManager.path + "\\emotes\\"))
            {
                return;
            }
            string[] files = Directory.GetFiles(ConfigManager.path + "\\emotes\\");
            foreach(string file in files)
            {
                string[] fileSplit = file.Split('\\');
                string fileName = fileSplit[^1];
                string[] fileNameSplit = fileName.Split('.');
                string emoteName = fileNameSplit[0];
                AddEmote(emoteName, file);
            }
        }
        public static Bitmap GetEmote(string emoteName)
        {
            if(emotes.ContainsKey(emoteName))
            {
                return new Bitmap(emotes[emoteName]);
            }
            return null;
        }
        public static string[] GetEmotes()
        {
            return emotes.Keys.ToArray();
        }
        public static string GetEmotePath(string emoteName)
        {
            if(emotes.ContainsKey(emoteName))
            {
                return emotes[emoteName];
            }
            return null;
        }
    }
}
