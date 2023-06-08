using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LawChat.Client.Persistence.Tables
{
    [Table("Files")]
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [ForeignKey("SenderId")]
        public Contact Sender { get; set; }
        public int SenderId { get; set; }

        [Required]
        [ForeignKey("RecipientId")]
        public Contact Recipient { get; set; }
        public int RecipientId { get; set; }

        [Required]
        public string ServerFilePath { get; set; }

        [Required]
        public string LocalFilePath { get; set; }
    }
}
