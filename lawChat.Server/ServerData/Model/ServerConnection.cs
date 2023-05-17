using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using lawChat.Network.Abstractions.Enums;
using lawChat.Network.Abstractions.Models;
using lawChat.Server.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace lawChat.Server.ServerData.Model
{
    public class ServerConnection
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _stream;
        private readonly EndPoint _remoteEndPoint;
        private readonly Task _readingTask;
        private readonly Task _writingTask;
        private readonly Action<ServerConnection> _disposeCallback;
        private readonly Channel<string> _channel;
        private LawChatDbContext _context;
        bool disposed;
        bool _isSigned;

        public ServerConnection(TcpClient client, Action<ServerConnection> disposeCallback)
        {
            _client = client;
            _stream = client.GetStream();
            _remoteEndPoint = client.Client.RemoteEndPoint;
            _disposeCallback = disposeCallback;
            _channel = Channel.CreateUnbounded<string>();
            _readingTask = RunReadingLoop();
            _writingTask = RunWritingLoop();
            _context = new LawChatDbContext();
        }

        private async Task RunReadingLoop()
        {
            await Task.Yield(); // https://ru.stackoverflow.com/a/1422205/373567
            try
            {
                byte[] lengthMessageHeader = new byte[4];

                while (true)
                {
                    int bytesReceived = await _stream.ReadAsync(lengthMessageHeader, 0, 4);

                    if (bytesReceived != 4)
                        break;

                    int length = BinaryPrimitives.ReadInt32LittleEndian(lengthMessageHeader);

                    byte[] buffer = new byte[length];

                    int count = 0;

                    while (count < length)
                    {
                        bytesReceived = await _stream.ReadAsync(buffer, count, buffer.Length - count);
                        count += bytesReceived;
                    }

                    SendMessageAsync(JsonConvert.SerializeObject(new Message()
                    {
                        Header = new Header()
                        {
                            MessageType = MessageType.Text
                        }
                    }));
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
        public async Task MessageHandler(string message)
        {

        }

        public async Task SendMessageAsync(string message)
        {
            await _channel.Writer.WriteAsync(message);
        }

        private async Task RunWritingLoop()
        {
            byte[] lenghtMessage = new byte[4];

            await foreach (string message in _channel.Reader.ReadAllAsync())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);

                BinaryPrimitives.WriteInt32LittleEndian(lenghtMessage, buffer.Length);

                await _stream.WriteAsync(lenghtMessage, 0, lenghtMessage.Length);
                
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
