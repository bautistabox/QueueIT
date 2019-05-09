using System.ComponentModel.DataAnnotations;

namespace QueueIT.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "first name is required")]
        [StringLength(450, ErrorMessage = "First Name cannot be longer than 450 characters.")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "last name is required")]
        [StringLength(450, ErrorMessage = "Last Name cannot be longer than 450 characters.")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "an email is required")]
        [StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "a username is required")]
        [StringLength(255, ErrorMessage = "Username cannot be longer than 255 characters.")]
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