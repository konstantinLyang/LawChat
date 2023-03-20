using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lawChat.Client.Services.Implementations
{
    public class ClientObjectService : IClientObject
    {
        public event NewMessageFromServer NewMessageFromServerEvent;
        public void OnNewMessageFromServer()
        {
            NewMessageFromServerEvent?.Invoke();
        }

        private IServiceProvider _serviceProvider;
        private IUserDialog _userDialog;

        public Socket ClientSocket;

        public ClientObjectService(IServiceProvider serviceProvider, IUserDialog userDialog)
        {
            _serviceProvider = serviceProvider;
            _userDialog = userDialog;

            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void OpenConnection(string login, string password)
        {
            IPEndPoint serverEndPoint = new(IPAddress.Parse("10.10.11.47"), 8080);

            ClientSocket.Connect(serverEndPoint);

            if (Authorization(login, password).Contains("successful connection"))
            {
                _userDialog.ShowMainWindow();
            };
        }

        private string Authorization(string login, string password)
        {
            ClientSocket.Send(Encoding.Unicode.GetBytes($"{login};{password};"));

            var serverBuffer = new byte[4026];

            var serverSize = ClientSocket.Receive(serverBuffer);

            return Encoding.Unicode.GetString(serverBuffer, 0, serverSize);
        }
        private Thread ListenServerMessage()
        {
            while (true)
            {
                if (ClientSocket.Connected)
                {
                    var serverBuffer = new byte[4026];

                    var serverSize = ClientSocket.Receive(serverBuffer);

                    Encoding.Unicode.GetString(serverBuffer, 0, serverSize);

                    NewMessageFromServerEvent.Invoke();
                }
                Thread.Sleep(100);
            }
        }
        public Task<string> SendServerMessage(string clientMessage)
        {
            var serverBuffer = new byte[4026];

            var serverSize = ClientSocket.Receive(serverBuffer);

            ClientSocket.Send(Encoding.Unicode.GetBytes(clientMessage));

            return Task.FromResult(Encoding.Unicode.GetString(serverBuffer, 0, serverSize));
        }
        public void CloseConnection()
        {
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
        }
    }
}
