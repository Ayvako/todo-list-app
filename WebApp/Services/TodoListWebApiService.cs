using System.Net.Http.Headers;
using WebApp.Interfaces;
using WebApp.Models.TodoLists;

namespace WebApp.Services;

public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ApiClientService apiClientService;

    public TodoListWebApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ApiClientService apiClientService)
    {
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.apiClientService = apiClientService;
    }

    public async Task<IEnumerable<TodoListWebApiModel>> GetAllAsync()
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<IEnumerable<TodoListWebApiModel>>(
            () => this.httpClient.GetAsync("api/TodoList/all"));

        if (result.Success && result.Data != null)
        {
            return result.Data;
        }

        return Enumerable.Empty<TodoListWebApiModel>();
    }

    public async Task<IEnumerable<TodoListWebApiModel>> GetByUserAsync()
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<IEnumerable<TodoListWebApiModel>>(
            () => this.httpClient.GetAsync($"api/TodoList/user"));

        if (result.Success && result.Data != null)
        {
            return result.Data;
        }

        return Enumerable.Empty<TodoListWebApiModel>();
    }

    public async Task<TodoListWebApiModel?> GetByIdAsync(int id)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<TodoListWebApiModel>(
            () => this.httpClient.GetAsync($"api/TodoList/{id}"));

        return result.Success ? result.Data : null;
    }

    public async Task<TodoListWebApiModel?> AddAsync(TodoListCreateModel model)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<TodoListWebApiModel>(
        () => this.httpClient.PostAsJsonAsync("api/TodoList", model));

        if (!result.Success)
        {
            Console.WriteLine($"Error adding list: {result.ErrorMessage}");
        }

        return result.Data;
    }

    public async Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListWebApiModel model)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<TodoListWebApiModel>(
            () => this.httpClient.PutAsJsonAsync($"api/TodoList/{id}", model));
        return result.Data;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        this.AttachToken();

        var response = await this.httpClient.DeleteAsync($"api/TodoList/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ShareAsync(ShareModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        this.AttachToken();

        var response = await this.httpClient.PostAsJsonAsync($"api/TodoList/{model.TodoListId}/share", model);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RevokeAsync(RevokeModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        this.AttachToken();

        var response = await this.httpClient.PostAsJsonAsync($"api/TodoList/{model.TodoListId}/revoke", model);
        return response.IsSuccessStatusCode;
    }

    private void AttachToken()
    {
        var token = this.httpContextAccessor.HttpContext?.Request.Cookies["jwt"];
        if (!string.IsNullOrEmpty(token))
        {
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
