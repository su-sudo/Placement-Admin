using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace CrudCoreAdoTask1.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("DOB")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [DisplayName("Image")]
        [Required]
        public string ProfilePic { get; set; }
        [Required]
        [DisplayName("Password")]
        public string PasswordHash { get; set; }

    }

    public class UserCreateViewModel
    {
        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("DOB")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [DisplayName("Image")]
        [Required]
        public IFormFile ProfilePic { get; set; }
        [Required]
        [DisplayName("Password")]
        public string Password { get; set; }

        

    }
       
           
        
    

}
