using System.Net;
using System.Net.Sockets;
using System.Text;

IPEndPoint serverEndPoint = new(IPAddress.Parse("10.10.11.47"), 5555);

Console.WriteLine("Введите имя: ");

var userName = Console.ReadLine();

Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

clientSocket.Connect(serverEndPoint);

byte[] buffer = new byte[4026];
int size = 0;
var serverAnswer = new StringBuilder();

clientSocket.Send(Encoding.Unicode.GetBytes(userName));

size = clientSocket.Receive(buffer);

Console.WriteLine(serverAnswer.Append(Encoding.Unicode.GetString(buffer, 0, size)));

while (true)
{
    buffer = new byte[4026];

    string clientMessage = Console.ReadLine();

    clientSocket.Send(Encoding.Unicode.GetBytes(clientMessage));
}