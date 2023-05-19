using System.Collections.Generic;
using lawChat.Server.Data.Model;

namespace lawChat.Client.Services
{
    public interface IClientData
    {
        public List<Chat>? ChatList { get; set; }
        public User? UserData { get; set; }
        public List<User>? FriendList { get; set; }
    }
}
