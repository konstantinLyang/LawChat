using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lawChat.Client.Services.Implementations
{
    public class ClientObjectService : IClientObject
    {
        private readonly IClientData _clientData;
        private Socket _clientSocket;

        public ClientObjectService(IClientData clientData)
        {
            _clientData = clientData;
        }

        public string OpenConnection(string login, string password)
        {
            try
            {
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint serverEndPoint = new(IPAddress.Parse("127.0.0.1"), 8080);

                _clientSocket.Connect(serverEndPoint);

                return Authorization(login, password);
            }
            catch
            {
                return "server error";
            }
        }
        private string Authorization(string login, string password)
        {
            _clientSocket.Send(Encoding.Unicode.GetBytes($"{login};{password};"));

            var serverBuffer = new byte[4026];

            var serverSize = _clientSocket.Receive(serverBuffer);

            string result = Encoding.Unicode.GetString(serverBuffer, 0, serverSize);

            if(result.Contains("successful connection")) _clientData.GetUserData(login, password);

            return result;
        }
        public async void SendTextMessage(int chatId, string message)
        {
            await Task.Factory.StartNew(() => { _clientSocket.Send(Encoding.Unicode.GetBytes($"message|CHAT;TYPE|text;{chatId};{message};")); });
        }
        public async void SendPrivateTextMessage(int recipient, string message)
        {
            await Task.Factory.StartNew(() => { _clientSocket.Send(Encoding.Unicode.GetBytes($"message|PRIVATE;TYPE|text;{recipient};{message};")); });
        }
        public string GetMessageFromServer()
        {
            var serverBuffer = new byte[4026];

            var serverSize = _clientSocket.Receive(serverBuffer);

            string result = Encoding.Unicode.GetString(serverBuffer, 0, serverSize);

            if (!result.Contains("speccommand"))
            {
                return Encoding.Unicode.GetString(serverBuffer, 0, serverSize);
            }

            return "gaose12h3ksafhai82t";
        }
    }
}
