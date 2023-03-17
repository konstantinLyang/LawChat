using System.Net;
using System.Net.Sockets;
using System.Text;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

IPEndPoint serverEndPoint = new(IPAddress.Parse("10.10.11.47"), 8080);

Console.WriteLine("Введите имя: ");

var userName = Console.ReadLine();

using (var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
{
    clientSocket.Connect(serverEndPoint);

    byte[] clientBuffer = new byte[4026];

    if (userName != null) clientSocket.Send(Encoding.Unicode.GetBytes(userName));

    Console.WriteLine(Encoding.Unicode.GetString(clientBuffer, 0, clientSocket.Receive(clientBuffer)));

    Task.Factory.StartNew(() =>
    {
        while (true)
        {
            string serverMessage = "";

            var serverBuffer = new byte[4026];

            var serverSize = clientSocket.Receive(serverBuffer);

            serverMessage = Encoding.Unicode.GetString(serverBuffer, 0, serverSize);

            Console.WriteLine(serverMessage);

            Thread.Sleep(20);
        }
    });

    while (true)
    {
        var clientMessage = Console.ReadLine();

        if (clientMessage != null) clientSocket.Send(Encoding.Unicode.GetBytes(clientMessage));
    }   
}
