using System.Net;
using System.Net.Sockets;
using System.Text;
using lawChat.Server.ServerData.Model;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

IPEndPoint endPoint = new(IPAddress.Any, 8080);
Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

List<Client> clientList = new();

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

        client.NickName = receiveMessage.Append(Encoding.Unicode.GetString(buffer, 0, size)).ToString();

        Console.WriteLine(client.NickName + " подключился");

        client.Socket.Send(Encoding.Unicode.GetBytes("Успешное подключение!"));

        try
        {
            while (client.Socket.Connected)
            {
                size = client.Socket.Receive(buffer);
                receiveMessage = new StringBuilder(Encoding.Unicode.GetString(buffer, 0, size));

                int chatId = Convert.ToInt32(receiveMessage.ToString().Split(';')[0]);

                Console.WriteLine(client.NickName + ": " + receiveMessage);

                foreach (var connectedClient in clientList)
                {
                    if(connectedClient != client && connectedClient.Id == chatId ) connectedClient.Socket.Send(Encoding.Unicode.GetBytes(client.NickName + ": " + receiveMessage));
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
    });
}