using Core.Entities.Tag;

namespace Core.Interfaces;

public interface ITagRepository
{
    Task<bool> AddTagAsync(int taskId, string tagName);

    Task<bool> RemoveTagAsync(int taskId, string tagName);

    Task<List<TagEntity>> GetTagsForUserAsync(int userId);
}
