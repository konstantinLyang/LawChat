using System;
using LawChat.Network.Abstractions.Models;

namespace LawChat.Client.Services
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
