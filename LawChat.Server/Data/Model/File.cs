using System.ComponentModel.DataAnnotations;

namespace LawChat.Server.Data.Model
{
    public class File
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public User Sender { get; set; }

        [Required]
        public User Recipient { get; set; }

        [Required]
        public string ServerLocalFilePath { get; set; }

        [Required]
        public string SenderLocalFilePath { get; set; }

        public string RecipientLocalFilePath { get; set; }
    }
}
