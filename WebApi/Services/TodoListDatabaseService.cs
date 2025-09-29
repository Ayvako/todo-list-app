using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services.Models;

namespace WebApi.Services;

#pragma warning disable IDE0079 // Remove unnecessary suppression

[SuppressMessage("Maintainability", "CA1515", Justification = "Controllers must be public for Swagger to work")]
public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly TodoListDbContext db;

    public TodoListDatabaseService(TodoListDbContext db)
    {
        this.db = db;
    }

    public async Task<IEnumerable<TodoList>> GetAllAsync()
    {
        var entities = await this.db.TodoLists.AsNoTracking().ToListAsync();
        return entities.Select(MapToDto);
    }

    public async Task<TodoList?> GetByIdAsync(int id)
    {
        var entity = await this.db.TodoLists
            .Include(l => l.Tasks)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        return entity == null ? null : MapToDto(entity);
    }

    public async Task<TodoList> AddAsync(TodoListModel model)
    {
        if (model == null)
        {
            return new TodoList();
        }

        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description,
            Tasks = new List<TaskEntity>
            {
                new ()
                {
                    Title = "Default Task",
                    Description = "This is a default task",
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(7),
                    Status = WebApi.Models.TaskStatus.NotStarted,
                    Assignee = "Creator",
                },
            },
        };

        _ = this.db.TodoLists.Add(entity);
        _ = await this.db.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByListIdAsync(int id)
    {
        var entity = await this.db.TodoLists
            .Include(l => l.Tasks)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (entity == null)
        {
            return Enumerable.Empty<TaskDto>();
        }

        return entity.Tasks.Select(MapTaskToDto);
    }

    public async Task<TodoList> UpdateAsync(int id, TodoListModel model)
    {
        if (model == null)
        {
            return new TodoList();
        }

        var entity = await this.db.TodoLists
            .Include(l => l.Tasks)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (entity == null)
        {
            return new TodoList();
        }

        entity.Title = model.Title;
        entity.Description = model.Description;

        _ = await this.db.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await this.db.TodoLists.FindAsync(id);

        if (entity == null)
        {
            return false;
        }

        _ = this.db.TodoLists.Remove(entity);
        _ = await this.db.SaveChangesAsync();

        return true;
    }

    private static TodoList MapToDto(TodoListEntity entity)
    {
        if (entity == null)
        {
            return new TodoList();
        }

        if (entity.Tasks == null)
        {
            return new TodoList();
        }

        TodoList todoList = new TodoList()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Tasks = entity.Tasks.Select(MapTaskToDto).ToList(),
        };

        return todoList;
    }

    private static TaskDto MapTaskToDto(TaskEntity task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Status = task.Status,
            Assignee = task.Assignee,
        };
    }
}
