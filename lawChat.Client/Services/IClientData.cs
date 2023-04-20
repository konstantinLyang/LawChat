using System.Collections.Generic;
using DataBase.Data.Model;

namespace lawChat.Client.Services
{
    public interface IClientData
    {
        public List<Chat>? ChatList { get; set; }
        public DataBase.Data.Model.Client? ClientData { get; set; }
        public List<DataBase.Data.Model.Client>? FriendList { get; set; }
        public void GetChatList();
        public void GetUserData(string login, string password);
        public void GetFriendList();
    }
}
