using System.Net.Sockets;

namespace lawChat.Server.ServerData.Model
{
    public class Client
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public Socket Socket { get; set; }
    }
}
