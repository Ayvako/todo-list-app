using WebApp.Models;

namespace WebApp.Services;

public class UserWebApiService : IUserWebApiService
{
    private readonly HttpClient httpClient;

    public UserWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<UserWebApiModel?> RegisterAsync(UserRegisterModel model)
    {
        var response = await httpClient.PostAsJsonAsync("api/User/register", model);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UserWebApiModel>();
    }

    public async Task<UserWebApiModel?> LoginAsync(UserLoginModel model)
    {
        var response = await httpClient.PostAsJsonAsync("api/User/login", model);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UserWebApiModel>();
    }
}
