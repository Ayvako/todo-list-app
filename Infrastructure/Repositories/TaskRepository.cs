using Core.Entities.Task;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<TaskEntity>> GetAssignedTasksAsync(int userId)
    {
        return await this.db.Tasks
            .Include(t => t.TodoList)
                .ThenInclude(l => l.Owner)
            .Include(t => t.TodoList)
                .ThenInclude(l => l.AccessList)
            .Include(t => t.Assignee)
            .AsNoTracking()
            .Where(t => t.AssigneeId == userId)
            .ToListAsync();
    }

    public async Task<TaskEntity> AddTaskAsync(int todoListId, TaskEntity task)
    {
        ArgumentNullException.ThrowIfNull(task);

        _ = await this.db.TodoLists
            .Include(l => l.Tasks)

            .FirstOrDefaultAsync(l => l.Id == todoListId) ?? throw new KeyNotFoundException($"TodoList {todoListId} not found");

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

    public async Task<TaskEntity?> UpdateTaskAsync(int id, TaskEntity updatedTask)
    {
        ArgumentNullException.ThrowIfNull(updatedTask);

        var entity = await this.db.Tasks.FindAsync(id);
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
}
