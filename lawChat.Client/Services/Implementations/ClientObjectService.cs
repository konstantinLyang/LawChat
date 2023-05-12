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

                IPEndPoint serverEndPoint = new(IPAddress.Parse("10.10.11.47"), 8080);

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
            _clientSocket.Send(Encoding.UTF8.GetBytes($"{login};{password};"));
            
            return GetMessageFromServer();
        }
        public void SendPrivateTextMessage(int recipient, string message)
        {
            _clientSocket.Send(Encoding.UTF8.GetBytes($"message;PRIVATE;text;{recipient};{message};"));
        }
        public void SendServerCommandMessage(string commandMessage)
        {
            _clientSocket.Send(Encoding.UTF8.GetBytes($"command;{commandMessage}"));
        }
        public string GetMessageFromServer()
        {
            var serverBuffer = new byte[180000];

            var serverSize = _clientSocket.Receive(serverBuffer);

            string result = Encoding.UTF8.GetString(serverBuffer, 0, serverSize);

            return result;
        }
    }
}
