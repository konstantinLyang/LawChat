using System.Threading.Tasks;

namespace lawChat.Client.Services
{
    public interface IClientObject
    {
        public void OpenConnection(string login, string password);
        public void CloseConnection();
        public void SendTextMessage(int chatId, string message);
        public Task<string> GetMessageFromServer();
    }
}
