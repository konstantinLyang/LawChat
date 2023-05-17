using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using lawChat.Network.Abstractions;
using lawChat.Network.Abstractions.Models;

namespace lawChat.Client.Services.Implementations
{
    public class ClientObjectService : IClientObject
    {
        public event EventHandler<Message>? MessageReceived;

        private readonly IClientData _clientData;
        
        private readonly IConnection _connection;

        public ClientObjectService(IClientData clientData, IConnection connection)
        {
            _connection = connection;
            _clientData = clientData;

            _connection.MessageReceived += HandlerMessageReceive;
        }

        /*public void SendPrivateFileMessage(int recipient, string filePath, string fileName)
        {
            Stream FileStream = File.OpenRead(filePath);

            byte[] FileBuffer = new byte[FileStream.Length];

            FileStream.Read(FileBuffer, 0, (int)FileStream.Length);

            NetworkStream.Write(FileBuffer, 0, FileBuffer.GetLength(0));

            NetworkStream.Close();
        }*/

        public Task SendMessageAsync(Message message)
        {
            throw new NotImplementedException();
        }

        public string OpenConnection(string login, string password)
        {
            _connection.Connect("127.0.0.1", 8080);

            _connection.SendMessageAsync(new Message()
            {
                Header = new Header()
                {

                },
                Data = Encoding.UTF8.GetBytes($"{login};{password}")
            });

            return "ХУЙ";
        }

        public string GetMessageFromServer()
        {
            throw new NotImplementedException();
        }

        public void SendServerCommandMessage(string commandMessage)
        {
            throw new NotImplementedException();
        }
        private void HandlerMessageReceive(object sender, Message message)
        {
            MessageReceived?.Invoke(this, message);
        }
    }
}
