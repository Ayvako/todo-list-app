using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebApp.Interfaces;
using WebApp.Models.Tasks;

namespace WebApp.Services;

public class TaskWebApiService : ITaskWebApiService
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ApiClientService apiClientService;

    public TaskWebApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ApiClientService apiClientService)
    {
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.apiClientService = apiClientService;
    }

    public async Task<TaskWebApiModel?> GetTaskByIdAsync(int id)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<TaskWebApiModel>(
            () => this.httpClient.GetAsync($"api/Task/{id}"));

        return result.Data;
    }

    public async Task<IEnumerable<TaskWebApiModel?>> GetAssignedTasksAsync()
    {
        this.AttachToken();
        var result = await this.apiClientService.TryRequestAsync<IEnumerable<TaskWebApiModel?>>(
            () => this.httpClient.GetAsync($"api/Task/AssignedTasks"));

        return result.Data ?? Enumerable.Empty<TaskWebApiModel>();
    }

    public async Task<TaskWebApiModel?> AddTaskAsync(int todoListId, TaskCreateModel model)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<TaskWebApiModel>(
            () => this.httpClient.PostAsJsonAsync($"api/Task/{todoListId}/tasks", model));

        return result.Data;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<object>(
            () => this.httpClient.DeleteAsync($"api/Task/{id}"));

        return result.Success;
    }

    public async Task<TaskWebApiModel?> UpdateTaskAsync(int id, TaskEditModel model)
    {
        this.AttachToken();
        if (model == null)
        {
            return null;
        }

        var taskResult = await this.apiClientService.TryRequestAsync<TaskWebApiModel>(
            () => this.httpClient.PutAsJsonAsync($"api/Task/{id}", model));

        if (!taskResult.Success)
        {
            Console.WriteLine($"Error updating task {id}: {taskResult.ErrorMessage}");
        }

        return taskResult.Data;
    }

    public async Task<bool> ChangeStatusAsync(int id, ChangeStatusModel model)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<object>(
            () => this.httpClient.PostAsJsonAsync($"api/Task/{id}/status", model));
        return result.Success;
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
