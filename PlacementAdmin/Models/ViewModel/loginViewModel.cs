using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlacementAdmin.Models.ViewModel
{
    public class loginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
