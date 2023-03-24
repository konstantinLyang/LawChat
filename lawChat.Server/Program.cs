using System.Net;
using System.Net.Sockets;
using System.Text;
using lawChat.Server.Data;
using lawChat.Server.ServerData.Model;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

IPEndPoint endPoint = new(IPAddress.Any, 8080);
Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

List<Client> clientList = new();
List<Chat> chatList = new();

serverSocket.Bind(endPoint);

while (true)
{
    serverSocket.Listen(5);

    Client client = new();

    client.Socket = serverSocket.Accept();

    Task.Factory.StartNew(() =>
    {
        clientList.Add(client);

        var buffer = new byte[4026];
        int size = 0;
        var receiveMessage = new StringBuilder();

        size = client.Socket.Receive(buffer);

        if (size > 0)
        {
            string login = receiveMessage.Append(Encoding.Unicode.GetString(buffer, 0, size)).ToString().Split(';')[0];
            string password = receiveMessage.Append(Encoding.Unicode.GetString(buffer, 0, size)).ToString().Split(';')[1];

            using (var context = new LawChatDbContext())
            {
                var user = context.Clients.FirstOrDefault(c => c.Login == login);
                if (user != null)
                {
                    if (login == user.Login && password == user.Password)
                    {
                        client.NickName = user.NickName;

                        Console.WriteLine($"[{DateTime.Now:dd.MM.yyyy HH:mm}] " + client.NickName + ": подключился");

                        client.Socket.Send(Encoding.Unicode.GetBytes("successful connection"));

                        try
                        {
                            while (client.Socket.Connected)
                            {
                                buffer = new byte[4026];
                                size = client.Socket.Receive(buffer);
                                receiveMessage = new StringBuilder(Encoding.Unicode.GetString(buffer, 0, size));

                                Guid messageGuid = Guid.Parse(receiveMessage.ToString().Split(';')[0]);
                                string messageType = receiveMessage.ToString().Split(';')[1];
                                int chatId = Convert.ToInt32(receiveMessage.ToString().Split(';')[2]);
                                string messageText = receiveMessage.ToString().Split(';')[3];

                                Console.WriteLine(client.NickName + ": " + messageText);

                                foreach (var connectedClient in clientList)
                                {
                                    if (messageType == "text")
                                    {
                                        if (connectedClient != client && connectedClient.Id == chatId) connectedClient.Socket.Send(Encoding.Unicode.GetBytes(client.NickName + ": " + messageText));
                                        else if (chatId == 0) connectedClient.Socket.Send(Encoding.Unicode.GetBytes(client.NickName + ": " + messageText));
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine(client.NickName + ": разрыв соединения");
                        }

                        client.Socket.Shutdown(SocketShutdown.Both);
                        client.Socket.Close();

                        clientList.Remove(client);
                    }
                    else
                    {
                        client.Socket.Send(Encoding.Unicode.GetBytes("incorrect user data"));
                    }
                }
                else
                {
                    client.Socket.Send(Encoding.Unicode.GetBytes("user not found"));
                }
            }
        }
    });
}