using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lawChat.Server.Data.Model
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        public int SenderId { get; set; }

        [ForeignKey("RecipientId")]
        public User Recipient { get; set; }

        public int RecipientId { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime ReadDateTime { get; set; }

        public string Text { get; set; }

        public LawChat.Server.Data.Model.File File { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
