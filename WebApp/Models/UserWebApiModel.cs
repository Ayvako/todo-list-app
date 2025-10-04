using Core.Enums;

namespace WebApp.Models;

public class UserWebApiModel
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Unauthorized;
}
