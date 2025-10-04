using WebApp.Models;

namespace WebApp.Services;

public interface IUserWebApiService
{
    Task<UserWebApiModel?> RegisterAsync(UserRegisterModel model);

    Task<UserWebApiModel?> LoginAsync(UserLoginModel model);
}
