using System;
using System.Text;
using System.Threading;
using lawChat.Network.Abstractions;
using lawChat.Network.Abstractions.Enums;
using lawChat.Network.Abstractions.Models;
using lawChat.Server.Data.Model;
using Newtonsoft.Json;
using PackageMessage = lawChat.Network.Abstractions.Models.PackageMessage;

namespace lawChat.Client.Services.Implementations
{
    public class ClientObjectService : IClientObject
    {
        public event EventHandler<PackageMessage>? MessageReceived;

        private readonly IClientData _clientData;
        
        private readonly IConnection _connection;

        private PackageMessage _answer;

        private bool _isAuthorized;
        

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
        
        public PackageMessage OpenConnection(string login, string password)
        {
            if (!_connection.IsConnected) _connection.Connect("10.10.11.47", 8080);

            try
            {
                SendMessage(new PackageMessage()
                {
                    Header = new Header()
                    {
                        MessageType = MessageType.Command,
                        CommandArguments = new[] { "authorization" }
                    },
                    Data = Encoding.UTF8.GetBytes(login + ";" + password),
                });

                WaitForAnswer();

                void WaitForAnswer()
                {
                    if (_answer != null) return;
                    Thread.Sleep(100);
                    WaitForAnswer();
                }

                if (_answer.Header.CommandArguments?[0] == "authorization successfully")
                {
                    _clientData.UserData = JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(_answer.Data));
                    _isAuthorized = true;
                }

                return _answer;
            }
            catch { return new PackageMessage() { Header = new Header() { StatusCode = StatusCode.ServerError } }; }
        }

        public void CloseConnection()
        {
            if(_connection.IsConnected) _connection.CloseConnection();
        }

        public void SendMessage(PackageMessage message)
        {
            _connection.SendMessageAsync(message);
        }
        private void HandlerMessageReceive(object sender, PackageMessage message)
        {
            if(_isAuthorized) MessageReceived?.Invoke(this, message);
            else _answer = message;
        }
    }
}
