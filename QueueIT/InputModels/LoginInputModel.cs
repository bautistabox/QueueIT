using System.ComponentModel.DataAnnotations;

namespace QueueIT.InputModels
{
    public class LoginInputModel
    {
        [Required]
        public string LoginUserName { get; set; }
        [Required]
        public string LoginPassword { get; set; }
    }
}