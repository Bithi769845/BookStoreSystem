using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // For displaying API error or success messages
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
