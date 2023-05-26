using System;
using lawChat.Network.Abstractions.Models;

namespace lawChat.Client.Services
{
    public interface IClientObject
    {
        event EventHandler<PackageMessage> MessageReceived;

        PackageMessage SignIn(string login, string password);

        PackageMessage SignUp(byte[] userData);

        void CloseConnection();

        void SendMessage(PackageMessage message);
    }
}
