namespace lawChat.Server.ServerData.Model
{
    public class ServerChatModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ServerClientModel> Clients { get; set; }
    }
}
