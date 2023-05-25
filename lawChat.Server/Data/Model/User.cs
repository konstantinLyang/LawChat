using System.ComponentModel.DataAnnotations;

namespace lawChat.Server.Data.Model
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string NickName { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        public string FatherName { get; set; }

        [Required]
        public string Login { get; set; }
        
        [Required]
        public string Password { get; set; }

        public string Telephone { get; set; }
        
        public string Email { get; set; }

        public List<User> Friends{ get; set; } = new (); 
    }
}
