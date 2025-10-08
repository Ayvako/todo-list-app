using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Models.TodoLists;

namespace WebApp.Services;

public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly JsonSerializerOptions jsonOptions;

    public TodoListWebApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        this.jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<IEnumerable<TodoListWebApiModel>> GetAllAsync()
    {
        this.AttachToken();

        return await this.httpClient
            .GetFromJsonAsync<IEnumerable<TodoListWebApiModel>>("api/TodoList", this.jsonOptions)
            ?? Enumerable.Empty<TodoListWebApiModel>();
    }

    public async Task<TodoListWebApiModel?> GetByIdAsync(int id)
    {
        this.AttachToken();

        return await this.httpClient
            .GetFromJsonAsync<TodoListWebApiModel>($"api/TodoList/{id}", this.jsonOptions);
    }

    public async Task<TodoListWebApiModel?> AddAsync(TodoListWebApiModel model)
    {
        this.AttachToken();

        if (model == null)
        {
            return null;
        }

        var response = await this.httpClient.PostAsJsonAsync("api/TodoList", model);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TodoListWebApiModel>(this.jsonOptions);
    }

    public async Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListWebApiModel model)
    {
        this.AttachToken();

        if (model == null)
        {
            return null;
        }

        var response = await this.httpClient.PutAsJsonAsync($"api/TodoList/{id}", model);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TodoListWebApiModel>(this.jsonOptions);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        this.AttachToken();
        var response = await this.httpClient.DeleteAsync($"api/TodoList/{id}");
        return response.IsSuccessStatusCode;
    }

    private void AttachToken()
    {
        var token = this.httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        if (!string.IsNullOrEmpty(token))
        {
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
