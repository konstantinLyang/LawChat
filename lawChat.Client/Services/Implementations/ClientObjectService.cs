﻿using System;
using System.Text;
using System.Threading;
using LawChat.Network.Abstractions;
using LawChat.Network.Abstractions.Enums;
using LawChat.Network.Abstractions.Models;
using LawChat.Server.Data.Model;
using Newtonsoft.Json;
using PackageMessage = LawChat.Network.Abstractions.Models.PackageMessage;

namespace LawChat.Client.Services.Implementations
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
        
        public PackageMessage SignIn(string? login, string? password)
        {
            try
            {
                var temp = new PackageMessage();

                if (!_connection.IsConnected) _connection.Connect("127.0.0.1", 8080);

                SendMessage(new ()
                {
                    Header = new Header()
                    {
                        MessageType = MessageType.Command,
                        CommandArguments = new[] { "signin" }
                    },
                    Data = Encoding.UTF8.GetBytes(login + ";" + password),
                });

                for (int i = 0; i < 150; i++)
                {
                    if (_answer != null) break;
                    Thread.Sleep(100);
                }

                if (_answer.Header.CommandArguments?[0] == "authorization successfully")
                {
                    _clientData.UserData = JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(_answer.Data));
                    _isAuthorized = true;

                    return _answer;
                }
                else if (_answer.Header.CommandArguments?[0] == "authorization incorrect user data")
                {
                    temp = _answer;
                    _answer = null;
                    return temp;
                }

                temp = _answer;
                _answer = null;
                return temp;

            }
            catch { return new PackageMessage() { Header = new Header() { StatusCode = StatusCode.ServerError } }; }
        }

        public PackageMessage SignUp(byte[] userData)
        {
            try
            {
                if (!_connection.IsConnected) _connection.Connect("127.0.0.1", 8080);

                SendMessage(new()
                {
                    Header = new()
                    {
                        MessageType = MessageType.Command,
                        StatusCode = StatusCode.PUT,
                        CommandArguments = new[] { "signup" }
                    },
                    Data = userData
                });

                WaitForAnswer();

                void WaitForAnswer()
                {
                    if (_answer != null) return;
                    Thread.Sleep(1000);
                    WaitForAnswer();
                }

                if (_answer.Header.StatusCode == StatusCode.Error)
                {
                    _answer = null;
                    return new PackageMessage(){Header = new Header(){StatusCode = StatusCode.Error}};
                }

                if (_answer.Header.CommandArguments?[0] == "signup" && _answer.Header.StatusCode == StatusCode.OK)
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
