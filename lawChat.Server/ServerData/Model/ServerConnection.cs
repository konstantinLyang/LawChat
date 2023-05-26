using System.Net.Sockets;
using System.Text;
using lawChat.Network.Abstractions.Models;
using LawChat.Network.Implementations;
using lawChat.Server.Data;
using lawChat.Network.Abstractions.Enums;
using lawChat.Server.Data.Model;
using Newtonsoft.Json;

namespace lawChat.Server.ServerData.Model
{
    public class ServerConnection
    {
        public User _userData;

        public Connection _connection;

        private LawChatDbContext _context;

        private List<ServerConnection> _connectedClients;

        private Action<ServerConnection> _dispose;

        public ServerConnection(TcpClient client, List<ServerConnection> connectedClients, Action<ServerConnection> disposeCallBack)
        {
            _connectedClients = connectedClients;

            _dispose = disposeCallBack;

            _connection = new(() => Dispose());

            _context = new LawChatDbContext();

            _connection.MessageReceived += HandlerReceivedMessage;

            _connection.Connect(client);
        }

        private async void HandlerReceivedMessage(object sender, PackageMessage message)
        {
            switch (message.Header.MessageType)
            {
                case MessageType.Command:
                    switch (message.Header.CommandArguments[0])
                    {
                        case "signin":
                            try
                            {
                                string[] loginData = Encoding.UTF8.GetString(message.Data).Split(';');

                                var client = _context.Clients.FirstOrDefault(x =>
                                    x.Login == loginData[0] && x.Password == loginData[1]);

                                if (client != null)
                                {
                                    _userData = client;

                                    try
                                    {
                                        foreach (var connectedClient in _connectedClients)
                                        {
                                            if (connectedClient._userData.Id != client.Id)
                                            {
                                                connectedClient._connection?.SendMessageAsync(new PackageMessage()
                                                {
                                                    Header = new Header()
                                                    {
                                                        MessageType = MessageType.Command,
                                                        CommandArguments = new[] { "new client connection", _userData.Id.ToString() }
                                                    }
                                                });
                                            }
                                        }
                                    }
                                    catch (Exception ex){}

                                    await _connection.SendMessageAsync(new PackageMessage()
                                    {
                                        Header = new Header()
                                        {
                                            MessageType = MessageType.Command,
                                            StatusCode = StatusCode.OK,
                                            CommandArguments = new [] { "authorization successfully" }
                                        },
                                        Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(client))
                                    });
                                }
                                else
                                {
                                    await _connection.SendMessageAsync(new PackageMessage()
                                    {
                                        Header = new Header()
                                        {
                                            MessageType = MessageType.Command,
                                            StatusCode = StatusCode.Error,
                                            CommandArguments = new [] { "authorization incorrect user data" }
                                        }
                                    });
                                }
                            }
                            catch
                            {
                                await _connection.SendMessageAsync(new PackageMessage()
                                {
                                    Header = new Header()
                                    {
                                        MessageType = MessageType.Command,
                                        StatusCode = StatusCode.ServerError
                                    }
                                });
                            }
                            break;

                        case "friend list":
                            try
                            {
                                _connection.SendMessageAsync(new PackageMessage()
                                {
                                    Header = new Header()
                                    {
                                        MessageType = MessageType.Command,
                                        StatusCode = StatusCode.OK,
                                        CommandArguments = new [] { "friend list" }
                                    },
                                    Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_context.Clients))
                                });
                            }
                            catch { throw new Exception("ошибка"); }
                            break;

                        case "get connection list":
                            try
                            {
                                if (_userData != null)
                                {
                                    foreach (var connectedClient in _connectedClients)
                                    {
                                        if (connectedClient._userData.Id != _userData.Id)
                                        {
                                            _connection?.SendMessageAsync(new PackageMessage()
                                            {
                                                Header = new Header()
                                                {
                                                    MessageType = MessageType.Command,
                                                    CommandArguments = new[] { "new client connection", connectedClient._userData.Id.ToString() }
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                throw new Exception("ошибка");
                            }
                            break;

                        case "messages":
                            try
                            {
                                int messageSender = _userData.Id;
                                int messageRecipient = Convert.ToInt32(message.Header.CommandArguments[1]);

                                var result = new List<Message>();

                                foreach (var msg in _context.Messages)
                                {
                                    if ((msg.SenderId == messageSender && msg.RecipientId == messageRecipient) ||
                                        (msg.SenderId == messageRecipient && msg.RecipientId == messageSender))
                                    {
                                        result.Add(msg);
                                    }
                                }

                                if (result.Count > 0)
                                {
                                    await _connection.SendMessageAsync(new PackageMessage()
                                    {
                                        Header = new Header()
                                        {
                                            MessageType = MessageType.Command,
                                            StatusCode = StatusCode.OK,
                                            CommandArguments = new[] { "messages", messageRecipient.ToString() }
                                        },
                                        Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result))
                                    });
                                }
                            }
                            catch{ throw new Exception("ошибка"); }
                            break;

                        case "signup":

                            User newClient =
                                JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(message.Data));

                            _context.Clients.Add(newClient);

                            _context.SaveChanges();

                            await _connection.SendMessageAsync(new()
                            {
                                Header = new()
                                {
                                    MessageType = MessageType.Command,
                                    StatusCode = StatusCode.OK,
                                    CommandArguments = new[] { "signup" }
                                },
                                Data = message.Data
                            });

                            try
                            {
                                
                            }
                            catch { throw new Exception("ошибка"); }
                            break;
                    }
                    break;

                case MessageType.Text:
                    try
                    {
                        _context.Messages.Add(new Message()
                        {
                            CreateDate = DateTime.Now,
                            Recipient = _context.Clients.FirstOrDefault(x =>
                                x.Id == Convert.ToInt32(message.Header.CommandArguments[0])),
                            Sender = _context.Clients.FirstOrDefault(x => x.Id == _userData.Id),
                            Text = Encoding.UTF8.GetString(message.Data)
                        });

                        _context.SaveChanges();

                        var recipient = _connectedClients
                            .FirstOrDefault(x => x._userData.Id == Convert.ToInt32(message.Header.CommandArguments[0]));

                        if (recipient != null)
                        {
                            recipient._connection.SendMessageAsync(new PackageMessage()
                            {
                                Header = new Header()
                                {
                                    MessageType = MessageType.Text,
                                    CommandArguments = new[] { _userData.Id.ToString() }
                                },
                                Data = message.Data
                            });
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                    break;

                case MessageType.File:

                    var to_client = _connectedClients.FirstOrDefault(x =>
                        x._userData.Id == Convert.ToInt32(message.Header.CommandArguments[0]));

                    string ServerCopyFile()
                    {
                        try
                        {
                            File.WriteAllBytes(
                                @$"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\TempFiles\{message.Header.CommandArguments[1]}",
                                message.Data);
                            return
                                @$"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\TempFiles\{message.Header.CommandArguments[1]}";
                        }
                        catch
                        {
                            try
                            {
                                File.WriteAllBytes(
                                    @$"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\TempFiles\{DateTime.Now:ssss}{message.Header.CommandArguments[1]}",
                                    message.Data);
                                return
                                    @$"Z:\!!!!!ПОЛЬЗОВАТЕЛИ\!КОНСТАНТИН_ЛЯНГ\PROGRAMMS\ПС для рабочего стола\LawChat\client\data\Image\TempFiles\{DateTime.Now:ssss}{message.Header.CommandArguments[1]}";
                            }
                            catch
                            {
                                return ServerCopyFile();
                            }
                        }
                    }

                    _context.Messages.Add(new()
                    {
                        CreateDate = DateTime.Now,
                        Recipient = _context.Clients.FirstOrDefault(x =>
                            x.Id == Convert.ToInt32(message.Header.CommandArguments[0])),
                        Sender = _context.Clients.FirstOrDefault(x => x.Id == _userData.Id),
                        Text = Encoding.UTF8.GetString(message.Data),
                        File = new()
                        {
                            Name = message.Header.CommandArguments[1],
                            ServerLocalFilePath = ServerCopyFile(),
                            SenderLocalFilePath = message.Header.CommandArguments[2],
                            Sender = _context.Clients.FirstOrDefault(x => x.Id == _userData.Id),
                            Recipient = _context.Clients.FirstOrDefault(x => x.Id == Convert.ToInt32(message.Header.CommandArguments[0])),
                        }
                    });

                    _context.SaveChanges();

                    if (to_client != null)
                    {
                        to_client._connection
                            .SendMessageAsync(new PackageMessage()
                            {
                                Header = new Header()
                                {
                                    MessageType = MessageType.File,
                                    CommandArguments = new[] { _userData.Id.ToString(), message.Header.CommandArguments[1] }
                                },
                                Data = message.Data
                            });
                    }

                   break;
            }
        }

        public void Dispose()
        {
            try
            {
                if (_userData != null)
                {
                    foreach (var connectedClient in _connectedClients)
                    {
                        if (connectedClient._userData.Id != _userData.Id)
                        {
                            connectedClient._connection?.SendMessageAsync(new PackageMessage()
                            {
                                Header = new Header()
                                {
                                    MessageType = MessageType.Command,
                                    CommandArguments = new[] { "client close connection", _userData.Id.ToString() }
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { }

            _connection.Dispose();
            _dispose(this);
        }
    }
}
