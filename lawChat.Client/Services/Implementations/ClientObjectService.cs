using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lawChat.Client.Services.Implementations
{
    public class ClientObjectService : IClientObject
    {

        private IServiceProvider _serviceProvider;
        private IUserDialog _userDialog;

        public Socket ClientSocket;

        public ClientObjectService(IServiceProvider serviceProvider, IUserDialog userDialog)
        {
            _serviceProvider = serviceProvider;
            _userDialog = userDialog;
        }

        public string OpenConnection(string login, string password)
        {
            try
            {
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint serverEndPoint = new(IPAddress.Parse("127.0.0.1"), 8080);

                ClientSocket.Connect(serverEndPoint);

                return Authorization(login, password);
            }
            catch
            {
                return "server error";
            }
        }
        private string Authorization(string login, string password)
        {
            ClientSocket.Send(Encoding.Unicode.GetBytes($"{login};{password};"));

            var serverBuffer = new byte[4026];

            var serverSize = ClientSocket.Receive(serverBuffer);

            return Encoding.Unicode.GetString(serverBuffer, 0, serverSize);
        }
        public async void SendTextMessage(int chatId, string message)
        {
            await Task.Factory.StartNew(() => { ClientSocket.Send(Encoding.Unicode.GetBytes($"{new Guid()};text;{chatId};{message};")); });
        }

        public string GetMessageFromServer()
        {
            var serverBuffer = new byte[4026];

            var serverSize = ClientSocket.Receive(serverBuffer);

            return Encoding.Unicode.GetString(serverBuffer, 0, serverSize);
        }
    }
}
