using System.Net.Sockets;
using System.Net;
using lawChat.Server.Data;

namespace lawChat.Server.ServerData.Model
{
    public class TcpServerModel : IDisposable
    {
        private readonly TcpListener _listener;

        private bool disposed;

        public List<ServerConnection> _clients { get; private set; }

        public TcpServerModel(int port)
        {
            _listener = new(IPAddress.Any, port);

            _clients = new();
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

                    lock (_clients)
                    {
                        _clients.Add(new ServerConnection(client, c =>
                        {
                            lock (_clients)
                            {
                                _clients.Remove(c);
                            }
                            c.Dispose();
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
            if (disposed)
                throw new ObjectDisposedException(typeof(TcpServerModel).FullName);
            disposed = true;
            _listener.Stop();
            if (disposing)
            {
                lock (_clients)
                {
                    if (_clients.Count > 0)
                    {
                        Console.WriteLine("Отключаю клиентов...");

                        foreach (var client in _clients)
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
