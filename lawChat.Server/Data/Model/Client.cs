using System.Net.Sockets;

namespace lawChat.Server.Data.Model
{
    public class Client
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
