using System.Collections.Generic;
using lawChat.Server.Data.Model;

namespace lawChat.Client.Services
{
    public interface IClientData
    {
        public List<Chat>? ChatList { get; set; }
        public Server.Data.Model.User? UserData { get; set; }
        public List<Server.Data.Model.User>? FriendList { get; set; }
        public void GetChatList();
        public void GetUserData(string login, string password);
        public void GetFriendList();
    }
}
