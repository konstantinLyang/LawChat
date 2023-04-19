using System.ComponentModel.DataAnnotations;

namespace DataBase.Data.Model
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public byte[]? Data { get; set; }
        [Required]
        public Client? Sender { get; set; }
        public Client? Recipient { get; set; }
        public Chat? Chat { get; set; }
    }
}
