using System.Collections.Generic;
using LawChat.Client.Persistence.Data;
using LawChat.Client.Persistence.Tables;

namespace LawChat.Client.Services.Implementations
{
    public class DataBaseService : IDataBase
    {
        public DataBaseService()
        {
            AppContext = new AppContext();
        }

        public AppContext AppContext { get; set; }

        public void AddMessage(Message message)
        {
            throw new System.NotImplementedException();
        }

        public void AddChat(Chat chat)
        {
            throw new System.NotImplementedException();
        }

        public void AddClient(Contact contact)
        {
            throw new System.NotImplementedException();
        }

        public List<Message> GetMessages()
        {
            throw new System.NotImplementedException();
        }

        public List<Chat> GetChats()
        {
            throw new System.NotImplementedException();
        }

        public List<Contact> GetContacts()
        {
            throw new System.NotImplementedException();
        }

        public List<Message> SyncMessages()
        {
            throw new System.NotImplementedException();
        }

        public List<Chat> SyncChats()
        {
            throw new System.NotImplementedException();
        }

        public List<Contact> SyncContacts()
        {
            throw new System.NotImplementedException();
        }
    }
}
