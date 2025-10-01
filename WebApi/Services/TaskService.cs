using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services;

public class TaskService : ITaskService
{
    private readonly TodoListDbContext db;

    public TaskService(TodoListDbContext db)
    {
        this.db = db;
    }

    public async Task<TaskModel?> GetTaskByIdAsync(int id)
    {
        var entity = await this.db.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (entity == null)
        {
            return null;
        }

        return MapToModel(entity);
    }

    public async Task<TaskModel> AddTaskAsync(int todoListId, TaskCreateModel model)
    {
        _ = await this.db.TodoLists
            .Include(l => l.Tasks)
            .FirstOrDefaultAsync(l => l.Id == todoListId) ?? throw new KeyNotFoundException($"TodoList {todoListId} not found");
        ArgumentNullException.ThrowIfNull(model);

        var taskEntity = new TaskEntity
        {
            Title = model.Title,
            Description = model.Description,
            CreatedAt = DateTime.UtcNow,
            DueDate = model.DueDate,
            Status = WebApi.Models.TaskStatus.NotStarted,
            Assignee = model.Assignee,
            TodoListId = todoListId,
        };

        _ = this.db.Tasks.Add(taskEntity);
        _ = await this.db.SaveChangesAsync();

        return MapToModel(taskEntity);
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

    public async Task<TaskModel?> UpdateTaskAsync(int id, TaskEditModel model)
    {
        var entity = await this.db.Tasks.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        ArgumentNullException.ThrowIfNull(model);

        entity.Title = model.Title;
        entity.Description = model.Description;
        entity.DueDate = model.DueDate;
        entity.Status = model.Status;

        _ = await this.db.SaveChangesAsync();

        return MapToModel(entity);
    }

    private static TaskModel MapToModel(TaskEntity entity)
    {
        return new TaskModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            DueDate = entity.DueDate,
            Status = entity.Status,
            Assignee = entity.Assignee,
            Tags = entity.Tags?.Select(t => t.Name).ToList() ?? new List<string>(),
            Comments = entity.Comments?.Select(c => new TaskCommentModel
            {
                User = c.User,
                Text = c.Text,
            }).ToList() ?? new List<TaskCommentModel>(),
        };
    }
}
