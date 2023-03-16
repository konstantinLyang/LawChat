using System.Net.Sockets;

namespace lawChat.Server.Model
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public Socket Socket { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
