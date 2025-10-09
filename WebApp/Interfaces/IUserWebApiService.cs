using WebApp.Models;
using WebApp.Models.Users;

namespace WebApp.Interfaces;

public interface IUserWebApiService
{
    Task<UserLoginResponseModel?> RegisterAsync(UserRegisterModel model);

    Task<UserLoginResponseModel?> LoginAsync(UserLoginModel model);
}
