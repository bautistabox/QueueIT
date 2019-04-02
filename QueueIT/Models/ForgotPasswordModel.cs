using System.ComponentModel.DataAnnotations;

namespace QueueIT.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}