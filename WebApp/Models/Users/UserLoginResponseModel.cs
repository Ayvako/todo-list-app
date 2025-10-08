namespace WebApp.Models.Users;

public class UserLoginResponseModel
{
    public string Token { get; set; } = string.Empty;

    public UserWebApiModel User { get; set; } = new();
}
