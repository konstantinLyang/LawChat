using System.Net.Sockets;
using System.Text;
using lawChat.Network.Abstractions.Models;
using LawChat.Network.Implementations;
using LawChat.Server.Data;
using lawChat.Network.Abstractions.Enums;
using LawChat.Server.Data.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using File = LawChat.Server.Data.Model.File;

namespace lawChat.Server.ServerData.Model
{
    public class ServerConnection
    {
        public User UserData;

        public Connection Connection;

        private readonly LawChatDbContext _context;

        private List<ServerConnection> _connectedClients;

        private Action<ServerConnection> _dispose;

        public ServerConnection(TcpClient client, List<ServerConnection> connectedClients, Action<ServerConnection> disposeCallBack)
        {
            _connectedClients = connectedClients;

            _dispose = disposeCallBack;

            Connection = new(() => Dispose());

            _context = new LawChatDbContext();

            Connection.MessageReceived += HandlerReceivedMessage;

            Connection.Connect(client);
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
                                    UserData = client;

                                    try
                                    {
                                        foreach (var connectedClient in _connectedClients)
                                        {
                                            if (connectedClient.UserData.Id != client.Id)
                                            {
                                                connectedClient.Connection?.SendMessageAsync(new PackageMessage()
                                                {
                                                    Header = new Header()
                                                    {
                                                        MessageType = MessageType.Command,
                                                        CommandArguments = new[] { "new client connection", UserData.Id.ToString() }
                                                    }
                                                });
                                            }
                                        }
                                    }
                                    catch (Exception ex){}

                                    await Connection.SendMessageAsync(new PackageMessage()
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
                                    await Connection.SendMessageAsync(new PackageMessage()
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
                                await Connection.SendMessageAsync(new PackageMessage()
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
                                Connection.SendMessageAsync(new PackageMessage()
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
                                if (UserData != null)
                                {
                                    foreach (var connectedClient in _connectedClients)
                                    {
                                        if (connectedClient.UserData.Id != UserData.Id)
                                        {
                                            Connection?.SendMessageAsync(new PackageMessage()
                                            {
                                                Header = new Header()
                                                {
                                                    MessageType = MessageType.Command,
                                                    CommandArguments = new[] { "new client connection", connectedClient.UserData.Id.ToString() }
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
                                if (_context.Messages.Any())
                                {
                                    int messageSender = UserData.Id;
                                    int messageRecipient = Convert.ToInt32(message.Header.CommandArguments[1]);

                                    var result = new List<Message>();

                                    var messages = _context.Messages.Include(u => u.File);
                                    
                                    foreach (var msg in messages)
                                    {
                                        if ((msg.SenderId == messageSender && msg.RecipientId == messageRecipient) ||
                                            (msg.SenderId == messageRecipient && msg.RecipientId == messageSender))
                                        {
                                            result.Add(msg);
                                        }
                                    }

                                    if (result.Count > 0)
                                    {
                                        await Connection.SendMessageAsync(new PackageMessage()
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
                            }
                            catch{ throw new Exception("ошибка"); }
                            break;

                        case "signup":

                            User newClient =
                                JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(message.Data));

                            if (_context.Clients.FirstOrDefault(x => x.NickName == newClient.NickName) == null &&
                                _context.Clients.FirstOrDefault(x => x.Login == newClient.Login) == null)
                            {
                                _context.Clients.Add(newClient);

                                _context.SaveChanges();

                                UserData = newClient;

                                await Connection.SendMessageAsync(new()
                                {
                                    Header = new()
                                    {
                                        MessageType = MessageType.Command,
                                        StatusCode = StatusCode.OK,
                                        CommandArguments = new[] { "signup" }
                                    },
                                    Data = message.Data
                                });
                            }
                            else
                            {
                                await Connection.SendMessageAsync(new() { Header = new() { StatusCode = StatusCode.Error } });
                            }
                            break;
                    }
                    switch (message.Header.StatusCode)
                    {
                        case StatusCode.UPDATE:
                            switch (message.Header.CommandArguments[0])
                            {
                                case "message recipient filepath":

                                    _context.Files
                                        .FirstOrDefault(x => x.Id == Convert.ToInt32(message.Header.CommandArguments[1]))!
                                        .RecipientLocalFilePath = message.Header.CommandArguments[2];

                                    _context.SaveChanges();

                                    break;
                                case "isread":
                                    try
                                    {
                                        var readMessage = _context.Messages.FirstOrDefault(x =>
                                                x.Id == Convert.ToInt32(message.Header.CommandArguments[1]));
                                        readMessage.IsRead = true;
                                        readMessage.ReadDateTime = DateTime.Now;
                                        
                                        _context.SaveChanges();
                                    }
                                    catch (Exception ex) {}
                                    break;
                            }
                            break;
                    }
                    break;

                case MessageType.Text:
                    try
                    {
                        var sendTextMessage = new Message()
                        {
                            CreateDate = DateTime.Now,
                            Recipient = _context.Clients.FirstOrDefault(x =>
                                x.Id == Convert.ToInt32(message.Header.CommandArguments[0])),
                            Sender = _context.Clients.FirstOrDefault(x => x.Id == UserData.Id),
                            Text = Encoding.UTF8.GetString(message.Data)
                        };
                        _context.Messages.Add(sendTextMessage);

                        _context.SaveChanges();

                        var recipient = _connectedClients
                            .FirstOrDefault(x => x.UserData.Id == Convert.ToInt32(message.Header.CommandArguments[0]));

                        if (recipient != null)
                        {
                            recipient.Connection.SendMessageAsync(new PackageMessage()
                            {
                                Header = new Header()
                                {
                                    MessageType = MessageType.Text,
                                    CommandArguments = new[] { UserData.Id.ToString(), sendTextMessage.Id.ToString() }
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

                    var toClient = _connectedClients.FirstOrDefault(x =>
                        x.UserData.Id == Convert.ToInt32(message.Header.CommandArguments[0])); // получатель

                    string ServerCopyFile()
                    {
                        try
                        {
                            System.IO.File.WriteAllBytes(
                                @$"Client\data\Image\TempFiles\{message.Header.CommandArguments[1]}",
                                message.Data);
                            return
                                @$"Client\data\Image\TempFiles\{message.Header.CommandArguments[1]}";
                        }
                        catch(IOException)
                        {
                            try
                            {
                                System.IO.File.WriteAllBytes(
                                    @$"Client\data\Image\TempFiles\{DateTime.Now:ssss}{message.Header.CommandArguments[1]}",
                                    message.Data);
                                return
                                    @$"Client\data\Image\TempFiles\{DateTime.Now:ssss}{message.Header.CommandArguments[1]}";
                            }
                            catch (IOException)
                            {
                                return ServerCopyFile();
                            }
                        }
                    }

                    string filepath = Path.GetFullPath(ServerCopyFile());

                    var newMessage = new Message()
                    {
                        CreateDate = DateTime.Now,
                        Recipient = _context.Clients.FirstOrDefault(x =>
                            x.Id == Convert.ToInt32(message.Header.CommandArguments[0])),
                        Sender = _context.Clients.FirstOrDefault(x => x.Id == UserData.Id),
                        File = new()
                        {
                            Name = message.Header.CommandArguments[1],
                            ServerLocalFilePath = filepath,
                            SenderLocalFilePath = message.Header.CommandArguments[2],
                            Sender = _context.Clients.FirstOrDefault(x => x.Id == UserData.Id),
                            Recipient = _context.Clients.FirstOrDefault(x =>
                                x.Id == Convert.ToInt32(message.Header.CommandArguments[0])),
                        }
                    };

                    _context.Messages.Add(newMessage);

                    _context.SaveChanges();

                    if (toClient != null)
                    {
                        toClient.Connection
                            .SendMessageAsync(new PackageMessage()
                            {
                                Header = new Header()
                                {
                                    MessageType = MessageType.File,
                                    CommandArguments = new[] { UserData.Id.ToString(), message.Header.CommandArguments[1], newMessage.File.Id.ToString() } // отправитель, имя файла, айди файла
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
                if (UserData != null)
                {
                    foreach (var connectedClient in _connectedClients)
                    {
                        if (connectedClient.UserData.Id != UserData.Id)
                        {
                            connectedClient.Connection?.SendMessageAsync(new PackageMessage()
                            {
                                Header = new Header()
                                {
                                    MessageType = MessageType.Command,
                                    CommandArguments = new[] { "client close connection", UserData.Id.ToString() }
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { }

            Connection.Dispose();
            _dispose(this);
        }
    }
}
