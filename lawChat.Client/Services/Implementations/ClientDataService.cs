using System.Collections.Generic;
using LawChat.Server.Data.Model;

namespace LawChat.Client.Services.Implementations
{
    public class ClientDataService : IClientData
    {
        public User UserData { get; set; } = new();

        public List<User>? FriendList { get; set; } = new();
    }
}
