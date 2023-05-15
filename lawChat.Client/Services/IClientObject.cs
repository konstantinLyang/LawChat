namespace lawChat.Client.Services
{
    public interface IClientObject
    {
        public string OpenConnection(string login, string password);
        public string GetMessageFromServer();
        public void SendPrivateTextMessage(int recipient, string message);
        public void SendPrivateFileMessage(int recipient, string filePath);
        public void SendServerCommandMessage(string commandMessage);
    }
}
