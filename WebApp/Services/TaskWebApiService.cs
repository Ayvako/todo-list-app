using System.Globalization;
using System.Net.Http.Headers;
using WebApp.Interfaces;
using WebApp.Models.Tasks;
using TaskStatus = Core.Enums.TaskStatus;

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

    public async Task<IEnumerable<TaskWebApiModel?>> GetAllAsync()
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<IEnumerable<TaskWebApiModel?>>(
            () => this.httpClient.GetAsync($"api/Task"));

        return result.Data ?? Enumerable.Empty<TaskWebApiModel>();
    }

    public async Task<List<TaskWebApiModel?>> GetSortedAssignedTasks(TaskStatus? status = TaskStatus.InProgress, string? sortBy = "name", string? sortOrder = "asc")
    {
        var tasks = await this.GetAssignedTasksAsync(status);

        sortBy ??= "name";

        sortOrder ??= "asc";

        tasks = (sortBy.ToLower(CultureInfo.CurrentCulture), sortOrder.ToLower(CultureInfo.CurrentCulture)) switch
        {
            ("duedate", "asc") => tasks.OrderBy(t => t?.DueDate).ToList(),
            ("duedate", "desc") => tasks.OrderByDescending(t => t?.DueDate).ToList(),
            ("name", "desc") => tasks.OrderByDescending(t => t?.Title).ToList(),
            _ => tasks.OrderBy(t => t?.Title).ToList()
        };

        return tasks.ToList();
    }

    public async Task<List<TaskWebApiModel?>> GetFilteredTasksAsync(string? searchTitle = null, string createdRange = "all", string dueFilter = "all")
    {
        var now = DateTime.UtcNow;
        var tasks = await this.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(searchTitle))
        {
            tasks = tasks
                .Where(t => t?.Title.Contains(searchTitle, StringComparison.OrdinalIgnoreCase) ?? false)
                .ToList();
        }

        if (!string.IsNullOrEmpty(createdRange) && !createdRange.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            DateTime createdFrom = createdRange.ToLower(CultureInfo.CurrentCulture) switch
            {
                "today" => now.AddDays(-1),
                "week" => now.AddDays(-7),
                "month" => now.AddMonths(-1),
                "year" => now.AddYears(-1),
                _ => DateTime.MinValue
            };
            tasks = tasks.Where(t => t?.CreatedAt >= createdFrom).ToList();
        }

        if (!string.IsNullOrEmpty(dueFilter) && !dueFilter.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            if (dueFilter.Equals("overdue", StringComparison.OrdinalIgnoreCase))
            {
                tasks = tasks.Where(t => t?.DueDate < now && t.Status != TaskStatus.Completed).ToList();
            }
            else
            {
                DateTime dueTo = dueFilter.ToLower(CultureInfo.CurrentCulture) switch
                {
                    "day" => now.AddDays(1),
                    "week" => now.AddDays(7),
                    "month" => now.AddMonths(1),
                    "year" => now.AddYears(1),
                    _ => DateTime.MaxValue
                };

                tasks = tasks.Where(t => t?.DueDate >= now && t.DueDate <= dueTo && t.Status != TaskStatus.Completed).ToList();
            }
        }

        return tasks.ToList() ?? new List<TaskWebApiModel?>();
    }

    public async Task<TaskWebApiModel?> GetTaskByIdAsync(int id)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<TaskWebApiModel>(
            () => this.httpClient.GetAsync($"api/Task/{id}"));

        return result.Data;
    }

    public async Task<IEnumerable<TaskWebApiModel?>> GetAssignedTasksAsync(TaskStatus? status = TaskStatus.InProgress)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<IEnumerable<TaskWebApiModel?>>(
            () => this.httpClient.GetAsync($"api/Task/assigned?status={status!.Value}"));

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

    public async Task<bool> AddTagAsync(int taskId, string tagName)
    {
        this.AttachToken();
        var model = new TagModel { Name = tagName };
        var result = await this.apiClientService.TryRequestAsync<object>(
            () => this.httpClient.PostAsJsonAsync($"api/Task/{taskId}/tags", model));
        return result.Success;
    }

    public async Task<bool> RemoveTagAsync(int taskId, string tagName)
    {
        this.AttachToken();
        var model = new TagModel { Name = tagName };
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/Task/{taskId}/tags")
        {
            Content = JsonContent.Create(model),
        };
        var result = await this.apiClientService.TryRequestAsync<object>(
            () => this.httpClient.SendAsync(request));
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
