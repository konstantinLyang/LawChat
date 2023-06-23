namespace LawChat.Network.Abstractions.Models
{
    [Serializable]
    public class PackageMessage
    {
        public Header Header { get; set; }
        
        public byte[] Data { get; set; }
    }
}
