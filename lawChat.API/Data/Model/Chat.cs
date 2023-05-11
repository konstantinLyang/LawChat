using System.ComponentModel.DataAnnotations;

namespace lawChat.API.Data.Model
{
    public class Chat
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public List<User> Clients { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
    }
}
