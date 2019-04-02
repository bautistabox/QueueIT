using System.ComponentModel.DataAnnotations;

namespace QueueIT.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "UserName or Email Address")]
        public string UserNameOrEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}