using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Interfaces;
using WebApp.Models.Tasks;
using WebApp.Models.Users;

namespace WebApp.Services;

public class TaskWebApiService : ITaskWebApiService
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ApiClientService apiClientService;
    private readonly JsonSerializerOptions jsonOptions;

    public TaskWebApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ApiClientService apiClientService)
    {
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.apiClientService = apiClientService;

        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        this.jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<TaskWebApiModel?> GetTaskByIdAsync(int id)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<TaskWebApiModel>(
            () => this.httpClient.GetAsync($"api/Task/{id}"));

        return result.Data;
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

        int? assigneeId = null;

        if (!string.IsNullOrWhiteSpace(model.AssigneeName))
        {
            var userResult = await this.apiClientService.TryRequestAsync<UserWebApiModel>(
                () => this.httpClient.GetAsync($"api/User/by-name/{model.AssigneeName}")
            );

            if (userResult.Success && userResult.Data != null)
            {
                assigneeId = userResult.Data.Id;
            }
            else
            {
                Console.WriteLine($"Unable to find user {model.AssigneeName}: {userResult.ErrorMessage}");
            }
        }

        var taskResult = await this.apiClientService.TryRequestAsync<TaskWebApiModel>(
            () => this.httpClient.PutAsJsonAsync($"api/Task/{id}", new TaskWebApiModel
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                Status = model.Status,
                AssigneeName = model.AssigneeName,
                AssigneeId = assigneeId,
            })
        );

        if (!taskResult.Success)
        {
            Console.WriteLine($"❌ Ошибка при обновлении задачи {id}: {taskResult.ErrorMessage}");
        }

        return taskResult.Data;
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
