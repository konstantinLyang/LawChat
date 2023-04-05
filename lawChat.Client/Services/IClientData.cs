using System.Collections.Generic;
using System.Threading.Tasks;
using lawChat.Client.Model.ClientData;

namespace lawChat.Client.Services
{
    public interface IClientData
    {
        public List<Chat>? ChatList { get; set; }
        public List<Model.ClientData.Client>? FriendList { get; set; }
        public void GetChatHistory(int chatId);
        public void GetFriendList(string data);
    }
}
