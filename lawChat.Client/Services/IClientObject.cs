using System;
using lawChat.Network.Abstractions.Models;

namespace lawChat.Client.Services
{
    public interface IClientObject
    {
        event EventHandler<Message> MessageReceived;
        string OpenConnection(string login, string password);
        void SendServerCommandMessage(string commandMessage);
    }
}
