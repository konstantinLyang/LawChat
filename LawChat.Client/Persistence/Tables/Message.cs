using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LawChat.Client.Persistence.Tables
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("SenderId")]
        public Contact Sender { get; set; }
        public int SenderId { get; set; }

        [Required]
        [ForeignKey("RecipientId")]
        public Contact Recipient { get; set; }
        public int RecipientId { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime ReadDateTime { get; set; }

        public string Text { get; set; }

        public File File { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
