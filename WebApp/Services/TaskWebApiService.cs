using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
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
            PropertyNameCaseInsensitive = true
        };
        this.jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<TaskWebApiModel?> GetTaskByIdAsync(int id)
    {
        AttachToken();
        return await this.httpClient.GetFromJsonAsync<TaskWebApiModel>($"api/Tasks/{id}", jsonOptions);
    }

    public async Task<TaskWebApiModel?> AddTaskAsync(int todoListId, TaskCreateModel model)
    {
        AttachToken();

        var response = await this.httpClient.PostAsJsonAsync($"api/Tasks/{todoListId}/tasks", model);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskWebApiModel>(jsonOptions);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        AttachToken();

        var response = await this.httpClient.DeleteAsync($"api/Tasks/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<TaskWebApiModel?> UpdateTaskAsync(int id, TaskEditModel model)
    {
        AttachToken();
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
            Assignee = model.Assignee,
        });

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TaskWebApiModel>(jsonOptions);
    }

    private void AttachToken()
    {
        var token = httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
