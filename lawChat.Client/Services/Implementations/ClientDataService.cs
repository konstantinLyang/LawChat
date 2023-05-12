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

        public void CommandServerReceivedHandler(string commandMessage)
        {
            string commandType = commandMessage.Split(';')[1];

            if (commandType == "userdata")
            {
                UserData = JsonConvert.DeserializeObject<User>(commandMessage.Split(';')[2]);
            }
            else if (commandType == "dialogresult")
            {
                FriendList = JsonConvert.DeserializeObject<List<User>>(commandMessage.Split(';')[2]);
            }
        }
        
    }
}
