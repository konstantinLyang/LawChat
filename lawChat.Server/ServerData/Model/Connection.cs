using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;

namespace lawChat.Server.ServerData.Model
{
    public class Connection
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly EndPoint _remoteEndPoint;
        private readonly Task _readingTask;
        private readonly Task _writingTask;
        private readonly Action<Connection> _disposeCallback;
        private readonly Channel<string> _channel;
        bool disposed;

        public Connection(TcpClient client, Action<Connection> disposeCallback)
        {
            _client = client;
            _stream = client.GetStream();
            _remoteEndPoint = client.Client.RemoteEndPoint;
            _disposeCallback = disposeCallback;
            _channel = Channel.CreateUnbounded<string>();
            _readingTask = RunReadingLoop();
            _writingTask = RunWritingLoop();
        }

        private async Task RunReadingLoop()
        {
            await Task.Yield(); // https://ru.stackoverflow.com/a/1422205/373567
            try
            {
                byte[] headerBuffer = new byte[4];
                while (true)
                {
                    int bytesReceived = await _stream.ReadAsync(headerBuffer, 0, 4);
                    if (bytesReceived != 4)
                        break;
                    int length = BinaryPrimitives.ReadInt32LittleEndian(headerBuffer);
                    byte[] buffer = new byte[length];
                    int count = 0;
                    while (count < length)
                    {
                        bytesReceived = await _stream.ReadAsync(buffer, count, buffer.Length - count);
                        count += bytesReceived;
                    }
                    string message = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine($"<< {_remoteEndPoint}: {message}");
                    await SendMessageAsync($"Echo: {message}");
                }
                Console.WriteLine($"Клиент {_remoteEndPoint} отключился.");
                _stream.Close();
            }
            catch (IOException)
            {
                Console.WriteLine($"Подключение к {_remoteEndPoint} закрыто сервером.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().Name + ": " + ex.Message);
            }
        }

        public async Task SendMessageAsync(string message)
        {
            Console.WriteLine($">> {_remoteEndPoint}: {message}");
            await _channel.Writer.WriteAsync(message);
        }

        private async Task RunWritingLoop()
        {
            byte[] header = new byte[4];
            await foreach (string message in _channel.Reader.ReadAllAsync())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                BinaryPrimitives.WriteInt32LittleEndian(header, buffer.Length);
                await _stream.WriteAsync(header, 0, header.Length);
                await _stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
            disposed = true;
            if (_client.Connected)
            {
                _channel.Writer.Complete();
                _stream.Close();
                Task.WaitAll(_readingTask, _writingTask);
            }

            if (disposing)
            {
                _client.Dispose();
            }
        }
    }
}
