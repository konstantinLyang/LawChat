using System.ComponentModel.DataAnnotations;

namespace lawChat.Server.Data.Model
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        public string? NickName { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? FatherName { get; set; }
        public string? Telephone { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Login { get; set; }
        [Required]
        public string? Password { get; set; }
        public List<Chat> Chats { get; set; } = new List<Chat>(); 
        public List<Client> Friends{ get; set; } = new List<Client>(); 
    }
}
