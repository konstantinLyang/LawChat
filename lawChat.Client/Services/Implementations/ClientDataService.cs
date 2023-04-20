using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataBase.Data;
using DataBase.Data.Model;

namespace lawChat.Client.Services.Implementations
{
    public class ClientDataService : IClientData
    {
        public DataBase.Data.Model.Client ClientData { get; set; } = new();
        public List<Chat>? ChatList { get; set; } = new();
        public List<DataBase.Data.Model.Client>? FriendList { get; set; } = new();
        public void GetUserData(string login, string password)
        {
            Task.Factory.StartNew(() =>
            {
                using (var context = new LawChatDbContext())
                {
                    ClientData = context.Clients.FirstOrDefault(x => x.Login == login && x.Password == password);
                }
            });
        }
        public void GetFriendList()
        {
            using (var context = new LawChatDbContext())
            {
                FriendList = context.Clients.ToList();
            }
        }
        public string GetLastMessage(int chatId)
        {
            using (var context = new LawChatDbContext())
            {
                return context.Messages.LastOrDefault(x => x.Chat.Id == chatId).Text;
            }
        }
        public void GetChatList()
        {
            Task.Factory.StartNew(() =>
            {
                using (var context = new LawChatDbContext())
                {
                    ChatList = context.Chats.ToList();
                }
            });
        }
    }
}
