using System.Text;
using lawChat.Server;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine("Запуск сервера....");

        using (TCPServerModel server = new TCPServerModel(8080))
        {
            Task serverTask = server.ListenAsync();

            await serverTask;
        }

        Console.WriteLine("Остановка сервера...");
    }
}
