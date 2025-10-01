using WebApp.Models;

namespace WebApp.Services;

public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient httpClient;

    public TodoListWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IEnumerable<TodoListWebApiModel>> GetAllAsync()
    {
        return await this.httpClient
            .GetFromJsonAsync<IEnumerable<TodoListWebApiModel>>("api/TodoList")
            ?? Enumerable.Empty<TodoListWebApiModel>();
    }

    public async Task<TodoListWebApiModel?> GetByIdAsync(int id)
    {
        return await this.httpClient
            .GetFromJsonAsync<TodoListWebApiModel>($"api/TodoList/{id}");
    }

    public async Task<TodoListWebApiModel?> AddAsync(TodoListWebApiModel model)
    {
        if (model == null)
        {
            return null;
        }

        var response = await this.httpClient.PostAsJsonAsync("api/TodoList", model);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TodoListWebApiModel>();
    }

    public async Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListWebApiModel model)
    {
        if (model == null)
        {
            return null;
        }

        var response = await this.httpClient.PutAsJsonAsync($"api/TodoList/{id}", model);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TodoListWebApiModel>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await this.httpClient.DeleteAsync($"api/TodoList/{id}");
        return response.IsSuccessStatusCode;
    }
}
