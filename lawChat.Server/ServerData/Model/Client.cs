using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lawChat.Server.ServerData.Model
{
    internal class Client
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public Socket Socket { get; set; }
    }
}
