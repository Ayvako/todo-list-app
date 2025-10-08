using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Models.Tasks;

namespace WebApp.Services;

public class TaskWebApiService : ITaskWebApiService
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly JsonSerializerOptions jsonOptions;

    public TaskWebApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        this.jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<TaskWebApiModel?> GetTaskByIdAsync(int id)
    {
        this.AttachToken();
        return await this.httpClient.GetFromJsonAsync<TaskWebApiModel>($"api/Task/{id}", this.jsonOptions);
    }

    public async Task<TaskWebApiModel?> AddTaskAsync(int todoListId, TaskCreateModel model)
    {
        this.AttachToken();

        var response = await this.httpClient.PostAsJsonAsync($"api/Task/{todoListId}/tasks", model);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskWebApiModel>(this.jsonOptions);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        this.AttachToken();

        var response = await this.httpClient.DeleteAsync($"api/Task/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<TaskWebApiModel?> UpdateTaskAsync(int id, TaskEditModel model)
    {
        this.AttachToken();
        if (model == null)
        {
            return null;
        }

        var response = await this.httpClient.PutAsJsonAsync($"api/Task/{id}", new TaskWebApiModel
        {
            Title = model.Title,
            Description = model.Description,
            DueDate = model.DueDate,
            Status = model.Status,
            Assignee = model.Assignee,
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskWebApiModel>(this.jsonOptions);
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
