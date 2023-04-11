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
        private IClientData _clientData;

        public ClientObjectService(IServiceProvider serviceProvider, IUserDialog userDialog, IClientData clientData)
        {
            _serviceProvider = serviceProvider;
            _userDialog = userDialog;
            _clientData = clientData;
        }
        public Socket ClientSocket { get; set; }

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

            string result = Encoding.Unicode.GetString(serverBuffer, 0, serverSize);

            return result;
        }
        public async void SendTextMessage(int chatId, string message)
        {
            await Task.Factory.StartNew(() => { ClientSocket.Send(Encoding.Unicode.GetBytes($"message;TYPE|text;{chatId};{message};")); });
        }

        public string GetMessageFromServer()
        {
            var serverBuffer = new byte[4026];

            var serverSize = ClientSocket.Receive(serverBuffer);

            string result = Encoding.Unicode.GetString(serverBuffer, 0, serverSize);

            if (!result.Contains("speccommand"))
            {
                return Encoding.Unicode.GetString(serverBuffer, 0, serverSize);
            }
            else
            {
                if (result.Contains("getfriendlist.OK"))
                {
                    _clientData.GetFriendList(result);
                }
                else if (result.Contains("getchatlist.OK"))
                {
                    _clientData.GetChatList(result);
                }
            }

            return "gaose12h3ksafhai82t";
        }
    }
}
