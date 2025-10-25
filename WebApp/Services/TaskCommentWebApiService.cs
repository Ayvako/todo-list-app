using System.Net.Http.Headers;
using Infrastructure.Http;
using WebApp.Interfaces;
using WebApp.Models.Comments;

namespace WebApp.Services;

public class TaskCommentWebApiService : ITaskCommentWebApiService
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ApiClientService apiClientService;

    public TaskCommentWebApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ApiClientService apiClientService)
    {
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.apiClientService = apiClientService;
    }

    public async Task<IEnumerable<TaskCommentModel>> GetCommentsAsync(int taskId)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<IEnumerable<TaskCommentModel>>(
            () => this.httpClient.GetAsync($"api/Task/{taskId}/comments"));

        return result.Data ?? Enumerable.Empty<TaskCommentModel>();
    }

    public async Task<TaskCommentModel?> AddCommentAsync(int taskId, TaskCommentCreateModel model)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<TaskCommentModel>(
            () => this.httpClient.PostAsJsonAsync($"api/Task/{taskId}/comments", model));

        return result.Data;
    }

    public async Task<TaskCommentModel?> UpdateCommentAsync(int taskId, int commentId, TaskCommentUpdateModel model)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<TaskCommentModel>(
            () => this.httpClient.PutAsJsonAsync($"api/Task/{taskId}/comments/{commentId}", model));

        return result.Data;
    }

    public async Task<bool> DeleteCommentAsync(int taskId, int commentId)
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<bool>(
            () => this.httpClient.DeleteAsync($"api/Task/{taskId}/comments/{commentId}"));

        return result.Data;
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
