using System.Net;
using lawChat.Network.Abstractions.Models;

namespace lawChat.Network.Abstractions
{
    public interface IConnection
    {
        event EventHandler<Message> MessageReceived;

        Task SendMessageAsync(Message message);

        Task Connect(string ipAddress, int port);
    }
}
