using WebApp.Models.Users;
using WebApp.Views.User;

namespace WebApp.Interfaces;

public interface IUserWebApiService
{
    Task<UserRegisterResponseModel?> RegisterAsync(UserRegisterModel model);

    Task<UserLoginResponseModel?> LoginAsync(UserLoginModel model);

    Task<bool> ForgotPasswordAsync(ForgotPasswordModel model);

    Task<bool> ResetPasswordAsync(ResetPasswordModel model);
}
