using System.Net.Sockets;
using System.Net;
using lawChat.Server.ServerData.Model;
using System.Text;
using lawChat.Server.Data;
using Newtonsoft.Json;
using lawChat.Server.Data.Model;
using System.Reflection;

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

                if (Authorization(receiveMessage.Append(Encoding.UTF8.GetString(buffer, 0, size)).ToString(), connectedClient))
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

                        connectedClient.Socket.Send(Encoding.UTF8.GetBytes("successful connection;"));

                        return true;
                    }
                    else
                    {
                        connectedClient.Socket.Send(Encoding.UTF8.GetBytes("incorrect user data"));

                        connectedClient.Socket.Shutdown(SocketShutdown.Both);
                        connectedClient.Socket.Close();

                        Users.Remove(connectedClient);

                        return false;
                    }
                }
                else
                {
                    connectedClient.Socket.Send(Encoding.UTF8.GetBytes("user not found"));

                    connectedClient.Socket.Shutdown(SocketShutdown.Both);
                    connectedClient.Socket.Close();

                    Users.Remove(connectedClient);

                    return false;
                }
            }
            catch(Exception ex)
            {
                connectedClient.Socket.Send(Encoding.UTF8.GetBytes("user not found"));

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
                    var receiveMessage = new StringBuilder(Encoding.UTF8.GetString(buffer, 0, size));

                    if (receiveMessage.ToString().Split(';')[0] == "message")
                    {
                        if (receiveMessage.ToString().Split(';')[1] == ("PRIVATE"))
                        {
                            SendPrivateMessage();
                        }
                        else
                        {
                            SendChatMessage();
                        }
                    }
                    else if (receiveMessage.ToString().Split(';')[0] == "command")
                    {
                        string asasd = receiveMessage.ToString().Split(';')[1];

                        if (receiveMessage.ToString().Split(';')[1] == "getuserdata")
                        {
                            var a = JsonConvert.SerializeObject(
                                _context.Clients.FirstOrDefault(x => x.Id == connectedClient.Id));
                            connectedClient.Socket.Send(Encoding.UTF8.GetBytes($"command;userdata;{a}"));
                        }
                        else if (receiveMessage.ToString().Split(';')[1] == "getdialoglist")
                        {
                            connectedClient.Socket.Send(Encoding.UTF8.GetBytes("command;dialogresult;" + JsonConvert.SerializeObject(_context.Clients)));
                        }
                        else if (receiveMessage.ToString().Split(';')[1] == "getmessages")
                        {
                            List<Message> resultMessages = new();

                            foreach (var message in _context.Messages)
                            {
                                try
                                {
                                    if (message.RecipientId == Convert.ToInt32(receiveMessage.ToString().Split(';')[3]) && message.SenderId == Convert.ToInt32(receiveMessage.ToString().Split(';')[2]))
                                        resultMessages.Add(message);
                                    if (message.RecipientId == Convert.ToInt32(receiveMessage.ToString().Split(';')[2]) && message.SenderId == Convert.ToInt32(receiveMessage.ToString().Split(';')[3]))
                                        resultMessages.Add(message);
                                }
                                catch
                                {
                                    throw new Exception();
                                }
                            }

                            connectedClient.Socket.Send(Encoding.UTF8.GetBytes("command;messages;" + JsonConvert.SerializeObject(resultMessages)));
                        }
                    }

                    void SendPrivateMessage()
                    {
                        string command = receiveMessage.ToString().Split(';')[0];
                        string messageType = receiveMessage.ToString().Split(';')[1];
                        string dataType = receiveMessage.ToString().Split(';')[2];
                        int recipient = Convert.ToInt32(receiveMessage.ToString().Split(';')[3]);
                        string messageText = receiveMessage.ToString().Split(';')[4];

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
                            if (dataType == "text")
                            {
                                if (client != connectedClient && client.Id == recipient)
                                {
                                    client.Socket.Send(
                                        Encoding.UTF8.GetBytes(connectedClient.Id + ";" + messageText));
                                }
                            }
                        }
                    }
                    void SendChatMessage()
                    {
                        string command = receiveMessage.ToString().Split(';')[0];
                        string messageType = receiveMessage.ToString().Split(';')[1];
                        string dataType = receiveMessage.ToString().Split(';')[2];
                        int chatId = Convert.ToInt32(receiveMessage.ToString().Split(';')[3]);
                        string messageText = receiveMessage.ToString().Split(';')[4];

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
                            if (dataType == "text")
                            {
                                if (client != connectedClient && client.Id == chatId)
                                {
                                    client.Socket.Send(Encoding.UTF8.GetBytes(client.NickName + ": " + messageText));
                                }
                                else if (chatId == 0 && client.Id == chatId)
                                {
                                    client.Socket.Send(Encoding.UTF8.GetBytes(client.NickName + ": " + messageText));
                                }
                            }
                        }
                    }
                    void SendFriendListCommand()
                    {

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
