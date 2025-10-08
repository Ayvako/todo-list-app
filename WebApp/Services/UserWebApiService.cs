using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Models;
using WebApp.Models.Users;

namespace WebApp.Services;

public class UserWebApiService : IUserWebApiService
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonOptions;

    public UserWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        this.jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<UserLoginResponseModel?> LoginAsync(UserLoginModel model)
    {
        var response = await this.httpClient.PostAsJsonAsync("api/User/login", model);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Ошибка при входе: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<UserLoginResponseModel>(this.jsonOptions);
        return result;
    }

    public async Task<UserLoginResponseModel?> RegisterAsync(UserRegisterModel model)
    {
        var response = await this.httpClient.PostAsJsonAsync("api/User/register", model);

        if (response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Conflict)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Register error: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<UserLoginResponseModel>(this.jsonOptions);
        return result;
    }
}
