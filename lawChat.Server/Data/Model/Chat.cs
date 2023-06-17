using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LawChat.Server.Data.Model
{
    public class Chat
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        [ForeignKey("OwnerId")]
        public User Owner { get; set; }
        public int OwnerId { get; set; }

        public List<User> Users { get; set; }
    }
}
