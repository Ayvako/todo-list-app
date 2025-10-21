using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Users
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
