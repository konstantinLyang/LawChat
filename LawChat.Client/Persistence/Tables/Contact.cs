using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LawChat.Client.Persistence.Tables
{
    [Table("Contacts")]
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string NickName { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string FatherName { get; set; }
        
        public string Telephone { get; set; }

        public string Email { get; set; }

        public string PhotoFilePath { get; set; }
    }
}
