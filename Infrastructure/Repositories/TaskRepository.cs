using Core.Entities.Task;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

using TaskStatus = Core.Enums.TaskStatus;

namespace Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TodoListDbContext db;

    public TaskRepository(TodoListDbContext db)
    {
        this.db = db;
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(int id)
    {
        return await this.db.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Include(t => t.Assignee)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<TaskEntity>> GetAssignedTasksAsync(int userId, TaskStatus? status = TaskStatus.InProgress)
    {
        var tasks = await this.GetAllAsync(userId);
        if (status.HasValue)
        {
            tasks = tasks.Where(t => t.Status == status.Value && t.Assignee.Id == userId).ToList();
        }
        status ??= TaskStatus.NotStarted;

        return tasks;
    }

    public async Task<List<TaskEntity>> GetAllAsync(int userId)
    {
        return await this.db.Tasks
            .Include(t => t.TodoList)
                .ThenInclude(l => l.Owner)
            .Include(t => t.TodoList)
                .ThenInclude(l => l.AccessList)
            .Include(t => t.Assignee)
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .AsNoTracking()
            .Where(t =>
                t.TodoList.OwnerId == userId ||
                t.TodoList.AccessList.Any(a => a.UserId == userId) ||
                t.AssigneeId == userId
            )
            .ToListAsync();
    }

    public async Task<TaskEntity> AddTaskAsync(int todoListId, TaskEntity task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var todoListExists = await this.db.TodoLists.AnyAsync(l => l.Id == todoListId);

        if (!todoListExists)
        {
            throw new KeyNotFoundException($"TodoList {todoListId} not found");
        }

        task.TodoListId = todoListId;
        task.CreatedAt = DateTime.UtcNow;

        _ = this.db.Tasks.Add(task);
        _ = await this.db.SaveChangesAsync();

        return task;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await this.db.Tasks.FindAsync(id);
        if (task == null)
        {
            return false;
        }

        _ = this.db.Tasks.Remove(task);
        _ = await this.db.SaveChangesAsync();
        return true;
    }

    public async Task<TaskEntity?> UpdateTaskAsync(TaskEntity updatedTask)
    {
        ArgumentNullException.ThrowIfNull(updatedTask);

        var entity = await this.db.Tasks.FindAsync(updatedTask.Id);
        if (entity == null)
        {
            return null;
        }

        entity.Title = updatedTask.Title;
        entity.Description = updatedTask.Description;
        entity.DueDate = updatedTask.DueDate;
        entity.Status = updatedTask.Status;
        entity.AssigneeId = updatedTask.AssigneeId;

        _ = await this.db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateStatusAsync(int id, TaskStatus newStatus)
    {
        var entity = await this.db.Tasks.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        entity.Status = newStatus;
        _ = await this.db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddTagAsync(int taskId, string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            throw new ArgumentException("Tag name cannot be empty.", nameof(tagName));
        }

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

    public async Task<List<TaskEntity>> GetTasksByTagAsync(string tagName, int userId)
    {
        return await this.db.Tasks
            .Where(t =>
                (t.TodoList.OwnerId == userId ||
                 t.TodoList.AccessList.Any(a => a.UserId == userId))
                && t.Tags.Any(tag => tag.Name == tagName))
            .Include(t => t.Assignee)
            .Include(t => t.Tags)
            .ToListAsync();
    }
}
