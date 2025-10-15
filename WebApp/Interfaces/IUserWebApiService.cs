using WebApp.Models.Users;

namespace WebApp.Interfaces;

public interface IUserWebApiService
{
    Task<UserRegisterResponseModel?> RegisterAsync(UserRegisterModel model);

    Task<UserLoginResponseModel?> LoginAsync(UserLoginModel model);
}
