using System.Net.Sockets;

namespace lawChat.Client.Services
{
    public interface IClientObject
    {
        public string OpenConnection(string login, string password);
        public void SendTextMessage(int chatId, string message);
        public string GetMessageFromServer();
    }
}
