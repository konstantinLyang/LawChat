using System.Text;
using LawChat.Server.ServerData.Model;

namespace LawChat.Server;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        if (!Directory.Exists(@"Client\data\Image\TempFiles\"))
            Directory.CreateDirectory(@"Client\data\Image\TempFiles\");

        Console.WriteLine("Запуск сервера....");

        using (TcpServerModel server = new TcpServerModel(8080))
        {
            Task serverTask = server.ListenAsync();

            await serverTask;
        }

        Console.WriteLine("Остановка сервера...");
    }
}