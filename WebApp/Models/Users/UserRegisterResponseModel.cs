namespace WebApp.Models.Users;

public class UserRegisterResponseModel
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
