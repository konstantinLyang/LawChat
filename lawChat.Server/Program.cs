using System.Text;
using lawChat.Server;

Console.OutputEncoding = Encoding.Unicode;
Console.InputEncoding = Encoding.Unicode;

ServerModel server = new();

server.StartServer();