namespace lawChat.Server.Data.Model
{
    public class Client
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Chat> Chats { get; set; } = new List<Chat>(); 
    }
}
