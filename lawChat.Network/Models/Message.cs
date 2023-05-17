namespace lawChat.Network.Abstractions.Models
{
    [Serializable]
    public class Message
    {
        public Header Header { get; set; }
        
        public byte[] Data { get; set; }
    }
}
