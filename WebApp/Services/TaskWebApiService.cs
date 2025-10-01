using WebApp.Models;

namespace WebApp.Services;

public class TaskWebApiService : ITaskWebApiService
{
    private readonly HttpClient httpClient;

    public TaskWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<TaskWebApiModel?> GetTaskByIdAsync(int id)
        => await this.httpClient.GetFromJsonAsync<TaskWebApiModel>($"api/Tasks/{id}");

    public async Task<TaskWebApiModel?> AddTaskAsync(int todoListId, TaskCreateModel model)
    {
        var response = await this.httpClient.PostAsJsonAsync($"api/Tasks/{todoListId}/tasks", model);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskWebApiModel>();
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var response = await this.httpClient.DeleteAsync($"api/Tasks/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<TaskWebApiModel?> UpdateTaskAsync(int id, TaskEditModel model)
    {
        if (model == null)
        {
            return null;
        }

        var response = await this.httpClient.PutAsJsonAsync($"api/Tasks/{id}", new TaskWebApiModel
        {
            Title = model.Title,
            Description = model.Description,
            DueDate = model.DueDate,
            Status = model.Status,
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskWebApiModel>();
    }
}
