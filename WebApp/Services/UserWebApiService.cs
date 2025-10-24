using Infrastructure.Http;
using WebApp.Interfaces;
using WebApp.Models.Users;
using WebApp.Views.User;

namespace WebApp.Services;

public class UserWebApiService : IUserWebApiService
{
    private readonly HttpClient httpClient;
    private readonly ApiClientService apiClientService;

    public UserWebApiService(HttpClient httpClient, ApiClientService apiClientService)
    {
        this.httpClient = httpClient;
        this.apiClientService = apiClientService;
    }

    public async Task<UserLoginResponseModel?> LoginAsync(UserLoginModel model)
    {
        var result = await this.apiClientService.TryRequestAsync<UserLoginResponseModel>(
            () => this.httpClient.PostAsJsonAsync("api/User/login", model));

        if (result.Success && result.Data != null)
        {
            return result.Data;
        }

        Console.WriteLine($"Login error: {result.ErrorMessage}");
        return null;
    }

    public async Task<bool> RegisterAsync(UserRegisterModel model)
    {
        var result = await this.apiClientService.TryRequestAsync<object>(
            () => this.httpClient.PostAsJsonAsync("api/User/register", model));

        if (result.Success && result.Data != null)
        {
            return result.Success;
        }

        Console.WriteLine($"Registration error: {result.ErrorMessage}");
        return false;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordModel model)
    {
        var result = await this.apiClientService.TryRequestAsync<object>(
            () => this.httpClient.PostAsJsonAsync("api/User/forgot-password", model));

        return result.Success;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordModel model)
    {
        var result = await this.apiClientService.TryRequestAsync<object>(
            () => this.httpClient.PostAsJsonAsync("api/users/reset-password", model));

        return result.Success;
    }
}
