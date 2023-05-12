using System.Text;
using lawChat.Server;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

ServerModel server = new();

server.StartServer();