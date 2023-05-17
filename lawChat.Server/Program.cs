using System.Text;
using lawChat.Server.ServerData.Model;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("Запуск сервера....");

        using (TcpServerModel server = new TcpServerModel(8080))
        {
            Task serverTask = server.ListenAsync();

            await serverTask;
        }

        Console.WriteLine("Остановка сервера...");
    }
}
