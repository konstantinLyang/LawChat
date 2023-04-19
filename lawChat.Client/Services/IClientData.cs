using System.Collections.Generic;
using DataBase.Data.Model;

namespace lawChat.Client.Services
{
    public interface IClientData
    {
        public List<Chat>? ChatList { get; set; }
        public List<DataBase.Data.Model.Client>? FriendList { get; set; }
        public void GetChatList(string data);
        public void GetFriendList(string data);
    }
}
