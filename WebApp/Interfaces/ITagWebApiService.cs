using WebApp.Models.Tags;

namespace WebApp.Interfaces;

public interface ITagWebApiService
{
    Task<bool> AddTagAsync(int taskId, string tagName);

    Task<bool> RemoveTagAsync(int taskId, string tagName);

    Task<IEnumerable<TagModel?>> GetTagsForCurrentUserAsync();
}
