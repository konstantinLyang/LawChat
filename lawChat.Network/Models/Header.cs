using lawChat.Network.Abstractions.Enums;

namespace lawChat.Network.Abstractions.Models
{
    public class Header
    {
        public MessageType MessageType { get; set; }

        public StatusCode StatusCode { get; set; }
    }
}
