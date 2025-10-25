using System.Net.Http.Headers;
using Infrastructure.Http;
using WebApp.Interfaces;
using WebApp.Models.Tags;

namespace WebApp.Services;

public class TagWebApiService : ITagWebApiService
{
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ApiClientService apiClientService;

    public TagWebApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ApiClientService apiClientService)
    {
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.apiClientService = apiClientService;
    }

    public async Task<bool> AddTagAsync(int taskId, string tagName)
    {
        this.AttachToken();
        var model = new TagModel { Name = tagName };
        var result = await this.apiClientService.TryRequestAsync<object>(
            () => this.httpClient.PostAsJsonAsync($"api/Tag/{taskId}/tags", model));
        return result.Success;
    }

    public async Task<bool> RemoveTagAsync(int taskId, string tagName)
    {
        this.AttachToken();
        var model = new TagModel { Name = tagName };
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/Tag/{taskId}/tags")
        {
            Content = JsonContent.Create(model),
        };
        var result = await this.apiClientService.TryRequestAsync<object>(
            () => this.httpClient.SendAsync(request));
        return result.Success;
    }

    public async Task<IEnumerable<TagModel?>> GetTagsForCurrentUserAsync()
    {
        this.AttachToken();

        var result = await this.apiClientService.TryRequestAsync<IEnumerable<TagModel?>>(
            () => this.httpClient.GetAsync($"api/Tag/tags"));

        return result.Data ?? Enumerable.Empty<TagModel>();
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
