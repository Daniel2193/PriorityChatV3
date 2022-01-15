using System.Collections.Generic;
using System.Drawing;
namespace PriorityChatV3
{
    static class UserManager
    {
        private static readonly List<User> userList = new();
        public static void UpdateUser(string username, Status status, Color color)
        {
            if(userList.Exists(x => x.Username == username))
            {
                userList.Find(x => x.Username == username).Status = status;
                userList.Find(x => x.Username == username).Color = color;
            }
            else
            {
                userList.Add(new User() { Username = username, Status = status, Color = color });
            }
        }
        public static void UpdateUser(User user){
            if(userList.Exists(x => x.Username == user.Username))
            {
                userList.Find(x => x.Username == user.Username).Status = user.Status;
                userList.Find(x => x.Username == user.Username).Color = user.Color;
            }
            else
            {
                userList.Add(user);
            }
        }
        public static void UpdateStatus(string username, Status status)
        {
            if (userList.Exists(x => x.Username == username))
            {
                userList.Find(x => x.Username == username).Status = status;
            }
            else
            {
                userList.Add(new User() { Username = username, Status = status , Color = Color.Blue});
            }
        }
        public static void UpdateStatus(Status status)
        {
            UpdateStatus(ConfigManager.GetConfig().Username, status);
        }
        public static void UpdateColor(string username, Color color)
        {
            if (userList.Exists(x => x.Username == username))
            {
                userList.Find(x => x.Username == username).Color = color;
            }
            else
            {
                userList.Add(new User() { Username = username, Status = Status.ONLINE, Color = color });
            }
        }
        public static void Setup(){
            userList.Clear();
            userList.Add(new User() { Username = ConfigManager.GetConfig().Username, Status = Status.ONLINE, Color = ConfigManager.GetConfig().ColorUsername });
        }
        public static User GetUser(string username)
        {
            if (userList.Exists(x => x.Username == username))
            {
                return userList.Find(x => x.Username == username);
            }
            return null;
        }
        public static List<User> GetAllUsers()
        {
            return userList;
        }
        public static User GetCurrentUser()
        {
            return userList.Find(x => x.Username == ConfigManager.GetConfig().Username);
        }
    }
}
