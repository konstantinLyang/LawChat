using System.Net.Sockets;
using System.Net;
using lawChat.Server.ServerData.Model;
using System.Text;
using lawChat.Server.Data;

namespace lawChat.Server
{
    public class ServerModel : IDisposable
    {
        readonly IPEndPoint _endPoint = new(IPAddress.Any, 8080);
        readonly Socket _serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private LawChatDbContext _context;

        public List<ServerChatModel> Chats { get; private set; }
        public List<ServerClientModel> Users { get; private set; }

        public void StartServer()
        {
            _context = new();

            Users = new();

            Chats = new();
            
            _serverSocket.Bind(_endPoint);

            _serverSocket.Listen(5);

            Console.WriteLine("Start server");

            while (true)
            {
                _serverSocket.Listen(5);

                ServerClientModel connectedClient = new();

                connectedClient.Socket = _serverSocket.Accept();

                OnUserAccepted(connectedClient);
            }
        }

        private void OnUserAccepted(ServerClientModel connectedClient)
        {
            Task.Factory.StartNew(() =>
            {
                Users.Add(connectedClient);

                var buffer = new byte[4026];
                int size = 0;
                var receiveMessage = new StringBuilder();

                size = connectedClient.Socket.Receive(buffer);

                if (Authorization(receiveMessage.Append(Encoding.Unicode.GetString(buffer, 0, size)).ToString(), connectedClient))
                {
                    MessagesHandler(connectedClient);
                }
            });
        }

        private bool Authorization(string loginData, ServerClientModel connectedClient)
        {
            string login = loginData.Split(';')[0];
            string password = loginData.Split(';')[1];

            try
            {
                var user = _context.Clients.FirstOrDefault(c => c.Login == login);
                if (user != null)
                {
                    if (login == user.Login && password == user.Password)
                    {
                        connectedClient.Id = user.Id;
                        connectedClient.NickName = user.NickName;
                        connectedClient.Login = user.Login;

                        Console.WriteLine($"[{DateTime.Now:dd.MM.yyyy HH:mm}] " + user.NickName + ": подключился");

                        connectedClient.Socket.Send(Encoding.Unicode.GetBytes("successful connection;"));

                        return true;
                    }
                    else
                    {
                        connectedClient.Socket.Send(Encoding.Unicode.GetBytes("incorrect user data"));

                        connectedClient.Socket.Shutdown(SocketShutdown.Both);
                        connectedClient.Socket.Close();

                        Users.Remove(connectedClient);

                        return false;
                    }
                }
                else
                {
                    connectedClient.Socket.Send(Encoding.Unicode.GetBytes("user not found"));

                    connectedClient.Socket.Shutdown(SocketShutdown.Both);
                    connectedClient.Socket.Close();

                    Users.Remove(connectedClient);

                    return false;
                }
            }
            catch(Exception ex)
            {
                connectedClient.Socket.Send(Encoding.Unicode.GetBytes("user not found"));

                connectedClient.Socket.Shutdown(SocketShutdown.Both);
                connectedClient.Socket.Close();

                Users.Remove(connectedClient);

                return false;
            }
        }

        private void MessagesHandler(ServerClientModel connectedClient)
        {
            try
            {
                while (connectedClient.Socket.Connected)
                {
                    byte[] buffer = new byte[4026];
                    int size = connectedClient.Socket.Receive(buffer);
                    var receiveMessage = new StringBuilder(Encoding.Unicode.GetString(buffer, 0, size));

                    if (receiveMessage.ToString().Contains("message"))
                    {
                        if (receiveMessage.ToString().Contains("PRIVATE"))
                        {
                            SendPrivateMessage();
                        }
                        else
                        {
                            SendChatMessage();
                        }
                    }

                    void SendPrivateMessage()
                    {
                        string messageType = receiveMessage.ToString().Split(';')[0];
                        string typeType = receiveMessage.ToString().Split(';')[1];
                        int recipient = Convert.ToInt32(receiveMessage.ToString().Split(';')[2]);
                        string messageText = receiveMessage.ToString().Split(';')[3];

                        Console.WriteLine(connectedClient.NickName + ": " + messageText);

                        _context.Messages.Add(new()
                        {
                            CreateDate = DateTime.Now,
                            Recipient = _context.Clients.FirstOrDefault(x => x.Id == recipient),
                            Sender = _context.Clients.FirstOrDefault(x => x.Id == connectedClient.Id),
                            Text = messageText
                        });
                        _context.SaveChanges();

                        foreach (var client in Users)
                        {
                            if (typeType == "TYPE|text")
                            {
                                if (client != connectedClient && client.Id == recipient)
                                {
                                    client.Socket.Send(
                                        Encoding.Unicode.GetBytes(connectedClient.Id + ";" + messageText));
                                }
                            }
                        }
                    }

                    void SendChatMessage()
                    {
                        string messageType = receiveMessage.ToString().Split(';')[0];
                        string typeType = receiveMessage.ToString().Split(';')[1];
                        int chatId = Convert.ToInt32(receiveMessage.ToString().Split(';')[2]);
                        string messageText = receiveMessage.ToString().Split(';')[3];

                        Console.WriteLine(connectedClient.NickName + ": " + messageText);

                        _context.Messages.Add(new()
                        {
                            CreateDate = DateTime.Now,
                            Chat = _context.Chats.FirstOrDefault(x => x.Id == chatId),
                            Sender = _context.Clients.FirstOrDefault(x => x.Id == connectedClient.Id),
                            Text = messageText
                        });

                        foreach (var client in Users)
                        {
                            if (typeType == "TYPE|text")
                            {
                                if (client != connectedClient && client.Id == chatId)
                                {
                                    client.Socket.Send(Encoding.Unicode.GetBytes(client.NickName + ": " + messageText));
                                }
                                else if (chatId == 0 && client.Id == chatId)
                                {
                                    client.Socket.Send(Encoding.Unicode.GetBytes(client.NickName + ": " + messageText));
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine(connectedClient.NickName + " : разрыв соединения");
            }

            connectedClient.Socket.Shutdown(SocketShutdown.Both);
            connectedClient.Socket.Close();

            Users.Remove(connectedClient);
        }

        public void Dispose()
        {
            _serverSocket?.Dispose();
        }
    }
}
