namespace lawChat.Server.Data.Model
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public byte[] Data { get; set; }
        public Client Client { get; set; }
        public Chat Chat { get; set; }
    }
}
