using System.Collections.Generic;
using lawChat.Server.Data.Model;
using Newtonsoft.Json;

namespace lawChat.Client.Services.Implementations
{
    public class ClientDataService : IClientData
    {
        public User UserData { get; set; } = new();
        public List<Chat>? ChatList { get; set; } = new();
        public List<User>? FriendList { get; set; } = new();
        
    }
}
