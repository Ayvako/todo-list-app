using WebApp.Models;

namespace WebApp.Services;

public class UserWebApiService : IUserWebApiService
{
    private readonly HttpClient httpClient;

    public UserWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<UserLoginResponseModel?> LoginAsync(UserLoginModel model)
    {
        var response = await this.httpClient.PostAsJsonAsync("api/User/login", model);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(error);
        }

        var result = await response.Content.ReadFromJsonAsync<UserLoginResponseModel>();
        return result;
    }

    public async Task<UserLoginResponseModel?> RegisterAsync(UserRegisterModel model)
    {
        var response = await this.httpClient.PostAsJsonAsync("api/User/register", model);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(error);
        }

        var result = await response.Content.ReadFromJsonAsync<UserLoginResponseModel>();
        return result;
    }
}
