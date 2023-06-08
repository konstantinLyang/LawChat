using System.Collections.Generic;
using LawChat.Client.Persistence.Data;
using LawChat.Client.Persistence.Tables;

namespace LawChat.Client.Services
{
    public interface IDataBase
    {
        public AppContext AppContext { get; set; }

        public void AddMessage(Message message);
        public void AddChat(Chat chat);
        public void AddClient(Contact contact);

        public List<Message> GetMessages();
        public List<Chat> GetChats();
        public List<Contact> GetContacts();

        public List<Message> SyncMessages();
        public List<Chat> SyncChats();
        public List<Contact> SyncContacts();
    }
}
