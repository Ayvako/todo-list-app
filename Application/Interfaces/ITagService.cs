using Contracts.Tags;

namespace Application.Interfaces;

public interface ITagService
{
    Task<bool> AddTagAsync(int taskId, string tagName, int userId);

    Task<bool> RemoveTagAsync(int taskId, string tagName, int userId);

    Task<List<TagDto>> GetTagsForUserAsync(int userId);
}
