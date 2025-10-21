using System.ComponentModel.DataAnnotations;

namespace Contracts.Users;

public class ForgotPasswordDto
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
