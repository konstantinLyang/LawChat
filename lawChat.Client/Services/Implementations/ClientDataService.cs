using System.Collections.Generic;
using System.Threading.Tasks;
using lawChat.Client.Model.ClientData;
using Newtonsoft.Json;

namespace lawChat.Client.Services.Implementations
{
    public class ClientDataService : IClientData
    {
        public List<Chat>? ChatList { get; set; } = new();
        public List<Model.ClientData.Client>? FriendList { get; set; } = new();

        public void GetFriendList(string data)
        { 
            Task.Factory.StartNew(() =>
            {
                data = data.Remove(0, "speccommand|getfriendlist.OK".Length);
                FriendList = JsonConvert.DeserializeObject<List<Model.ClientData.Client>>(data);
            });
        }
        public void GetChatList(string data)
        {
            Task.Factory.StartNew(() =>
            {
                data = data.Remove(0, "speccommand|getchathistory.OK".Length);
                ChatList = JsonConvert.DeserializeObject<List<Chat>>(data);
            });
        }
    }
}
