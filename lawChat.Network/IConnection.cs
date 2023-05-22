using System.Net;
using lawChat.Network.Abstractions.Models;

namespace lawChat.Network.Abstractions
{
    public interface IConnection
    {
        event EventHandler<PackageMessage> MessageReceived;

        Task SendMessageAsync(PackageMessage message);

        void Connect(string ipAddress, int port);

        void CloseConnection();

        public bool IsConnected { get; }
    }
}
