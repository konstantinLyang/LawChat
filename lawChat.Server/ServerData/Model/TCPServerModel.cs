using System.Net.Sockets;
using System.Net;

namespace lawChat.Server.ServerData.Model
{
    public class TcpServerModel : IDisposable
    {
        private readonly TcpListener _listener;

        private bool _disposed;

        public List<ServerConnection> Clients { get; private set; }

        public TcpServerModel(int port)
        {
            _listener = new(IPAddress.Any, port);

            Clients = new();
        }

        public async Task ListenAsync()
        {
            try
            {
                _listener.Start();

                Console.WriteLine("Сервер стартовал на " + _listener.LocalEndpoint);

                while (true)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();

                    Console.WriteLine("Подключение: " + client.Client.RemoteEndPoint + " > " + client.Client.LocalEndPoint);

                    lock (Clients)
                    {
                        Clients.Add(new ServerConnection(client, Clients, c =>
                        {
                            lock (Clients)
                            {
                                Clients.Remove(c);
                            }
                        }));
                    }
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Сервер остановлен.");
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(TcpServerModel).FullName);
            _disposed = true;
            _listener.Stop();
            if (disposing)
            {
                lock (Clients)
                {
                    if (Clients.Count > 0)
                    {
                        Console.WriteLine("Отключаю клиентов...");

                        foreach (var client in Clients)
                        {
                            client.Dispose();
                        }
                        Console.WriteLine("Клиенты отключены.");
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TcpServerModel() => Dispose(false);
    }
}
