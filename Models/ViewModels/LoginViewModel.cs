using System.ComponentModel.DataAnnotations;

namespace OmniFlex.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
        public bool Remember { get; set; }
    }
}
