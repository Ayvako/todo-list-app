using WebApp.Models;

namespace WebApp.Services;

public interface IUserWebApiService
{
    Task<UserLoginResponseModel?> RegisterAsync(UserRegisterModel model);

    Task<UserLoginResponseModel?> LoginAsync(UserLoginModel model);
}
