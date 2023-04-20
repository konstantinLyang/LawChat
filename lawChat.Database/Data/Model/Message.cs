using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Data.Model
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public string? Text { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        [Required]
        [ForeignKey("SenderId")]
        public Client? Sender { get; set; }
        [ForeignKey("RecipientId")]
        public Client? Recipient { get; set; }
        public Chat? Chat { get; set; }
    }
}
