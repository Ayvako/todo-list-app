using WebApp.Interfaces;
using WebApp.Models.Users;

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

    public async Task<UserRegisterResponseModel?> RegisterAsync(UserRegisterModel model)
    {
        var result = await this.apiClientService.TryRequestAsync<UserRegisterResponseModel>(
            () => this.httpClient.PostAsJsonAsync("api/User/register", model));

        if (result.Success && result.Data != null)
        {
            return result.Data;
        }

        Console.WriteLine($"Registration error: {result.ErrorMessage}");
        return null;
    }
}
