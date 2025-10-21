using System.ComponentModel.DataAnnotations;

namespace Contracts.Users;

public class ResetPasswordDto
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
