namespace lawChat.Server.ServerData.Model
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Client> Clients { get; set; }
    }
}
