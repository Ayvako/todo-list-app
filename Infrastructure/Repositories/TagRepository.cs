using Core.Entities.Tag;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly TodoListDbContext db;

    public TagRepository(TodoListDbContext db)
    {
        this.db = db;
    }

    public async Task<bool> AddTagAsync(int taskId, string tagName)
    {
        var task = await this.db.Tasks
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            return false;
        }

        var tag = await this.db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);

        if (tag == null)
        {
            tag = new TagEntity { Name = tagName };
            _ = this.db.Tags.Add(tag);
            _ = await this.db.SaveChangesAsync();
        }

        if (!task.Tags.Any(t => t.Id == tag.Id))
        {
            task.Tags.Add(tag);
            _ = await this.db.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> RemoveTagAsync(int taskId, string tagName)
    {
        var task = await this.db.Tasks
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            return false;
        }

        var tag = task.Tags.FirstOrDefault(t => t.Name == tagName);
        if (tag == null)
        {
            return false;
        }

        _ = task.Tags.Remove(tag);
        _ = await this.db.SaveChangesAsync();
        return true;
    }

    public async Task<List<TagEntity>> GetTagsForUserAsync(int userId)
    {
        return await this.db.Tasks
            .Where(t =>
                t.TodoList.OwnerId == userId ||
                t.TodoList.AccessList.Any(a => a.UserId == userId))
            .SelectMany(t => t.Tags)
            .Distinct()
            .ToListAsync();
    }
}
