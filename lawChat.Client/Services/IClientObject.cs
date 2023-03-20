namespace lawChat.Client.Services
{
    public delegate void NewMessageFromServer();
    public interface IClientObject
    {
        public event NewMessageFromServer NewMessageFromServerEvent;
        public void OpenConnection(string login, string password);
        public void CloseConnection();
    }
}
