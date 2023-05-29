using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LawChat.Server.Data.Model
{
    public class File
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("SenderId")]
        public User Sender { get; set; }
        public int SenderId { get; set; }

        [Required]
        [ForeignKey("RecipientId")]
        public User Recipient { get; set; }
        public int RecipientId { get; set; }

        [Required]
        public string ServerLocalFilePath { get; set; }

        [Required]
        public string SenderLocalFilePath { get; set; }

        public string RecipientLocalFilePath { get; set; }
    }
}
