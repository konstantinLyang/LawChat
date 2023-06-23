using LawChat.Network.Abstractions.Enums;

namespace LawChat.Network.Abstractions.Models
{
    public class Header
    {
        public MessageType MessageType { get; set; }

        public StatusCode StatusCode { get; set; }

        public string[]? CommandArguments { get; set; } = new string[5];
    }
}
