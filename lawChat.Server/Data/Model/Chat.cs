namespace lawChat.Server.Data.Model
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
