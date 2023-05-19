using System.Net.Sockets;
using System.Text;
using lawChat.Network.Abstractions.Models;
using LawChat.Network.Implementations;
using lawChat.Server.Data;
using lawChat.Network.Abstractions.Enums;
using lawChat.Server.Data.Model;
using Newtonsoft.Json;
using System.Linq;

namespace lawChat.Server.ServerData.Model
{
    public class ServerConnection
    {
        public User _userData;

        private Connection _connection;

        private LawChatDbContext _context;

        private List<ServerConnection> _connectedClients;

        public ServerConnection(TcpClient client, List<ServerConnection> connectedClients)
        {
            _connectedClients = connectedClients;

            _connection = new();

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
                        case "authorization":
                            try
                            {
                                string[] loginData = Encoding.UTF8.GetString(message.Data).Split(';');

                                var client = _context.Clients.FirstOrDefault(x =>
                                    x.Login == loginData[0] && x.Password == loginData[1]);

                                if (client != null)
                                {
                                    _userData = client;

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
                                await _connection.SendMessageAsync(new PackageMessage()
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
                            catch
                            {
                                throw new Exception("ошибка");
                            }
                            break;

                        case "messages":

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
                            break;
                    }
                    break;
                case MessageType.Text:

                    _context.Messages.Add(new Message()
                    {
                        CreateDate = DateTime.Now,
                        Recipient = _context.Clients.FirstOrDefault(x => x.Id == Convert.ToInt32(message.Header.CommandArguments[0])),
                        Sender = _context.Clients.FirstOrDefault(x => x.Id == _userData.Id),
                        Text = Encoding.UTF8.GetString(message.Data)
                    });

                    _context.SaveChanges();

                    _connectedClients
                        .FirstOrDefault(x => x._userData.Id == Convert.ToInt32(message.Header.CommandArguments[0]))
                        ._connection.SendMessageAsync(new PackageMessage()
                        {
                            Header = new Header()
                            {
                                MessageType = MessageType.Text,
                                CommandArguments = new []{ _userData.Id.ToString() }
                            },
                            Data = message.Data
                        });
                    break;
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
