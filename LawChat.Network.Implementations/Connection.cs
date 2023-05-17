using lawChat.Network.Abstractions;
using lawChat.Network.Abstractions.Models;
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Channels;
using Newtonsoft.Json;

namespace LawChat.Network.Implementations
{
    public class Connection : IConnection
    {
        public event EventHandler<Message> MessageReceived;

        private TcpClient _client;
        private NetworkStream _stream;
        private EndPoint? _remoteEndPoint;
        private Task _readingTask;
        private Task _writingTask;
        private Channel<string> _channel;
        private bool _disposed;
        private bool _isConnected;

        public Connection()
        {
            
        }

        public async Task Connect(string ipAddress, int port)
        {
            if (_isConnected) throw new Exception("Connection already exist.");

            _client = new TcpClient(ipAddress, port);

            _stream = _client.GetStream();

            _isConnected = true;

            _channel = Channel.CreateUnbounded<string>();

            _readingTask = RunReadingLoop();

            _writingTask = RunWritingLoop();
        }

        private async Task RunReadingLoop()
        {
            try
            {
                byte[] headerBuffer = new byte[4];

                while (true)
                {
                    int bytesReceived = await _stream.ReadAsync(headerBuffer, 0, headerBuffer.Length);

                    if (bytesReceived != 4) break;

                    int length = BinaryPrimitives.ReadInt32LittleEndian(headerBuffer);

                    byte[] buffer = new byte[length];

                    int count = 0;

                    while (count < length)
                    {
                        bytesReceived = await _stream.ReadAsync(buffer, count, buffer.Length - count);
                        count += bytesReceived;
                    }

                    MessageReceived?.Invoke(this, JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(buffer)));
                }
                
                _stream.Close();
            }
            catch (IOException ex)
            {
                throw new IOException();
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task SendMessageAsync(Message message)
        {
            if (!_isConnected) throw new Exception("No connection.");

            await _channel.Writer.WriteAsync(JsonConvert.SerializeObject(message));
        }

        private async Task RunWritingLoop()
        {
            byte[] lengthMessage = new byte[4];

            await foreach (string message in _channel.Reader.ReadAllAsync())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);

                BinaryPrimitives.WriteInt32LittleEndian(lengthMessage, buffer.Length);

                await _stream.WriteAsync(lengthMessage, 0, lengthMessage.Length);

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
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            
            _disposed = true;

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

        ~Connection() => Dispose(false);
    }
}