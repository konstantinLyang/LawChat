using System;
using lawChat.Network.Abstractions.Models;

namespace lawChat.Client.Services
{
    public interface IClientObject
    {
        event EventHandler<PackageMessage> MessageReceived;
        PackageMessage OpenConnection(string login, string password);
        void SendMessage(PackageMessage message);
    }
}
