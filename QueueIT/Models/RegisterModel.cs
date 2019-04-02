using System.ComponentModel.DataAnnotations;

namespace QueueIT.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "first name is required")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "last name is required")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "an email is required")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "a username is required")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "I need a password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Re-enter the same password")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}