using Core.Entities.Tag;

namespace Infrastructure.Repositories;

public interface ITagRepository
{
    Task<bool> AddTagAsync(int taskId, string tagName);

    Task<bool> RemoveTagAsync(int taskId, string tagName);

    Task<List<TagEntity>> GetTagsForUserAsync(int userId);
}
