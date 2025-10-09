using Core.Entities.Task;
using Core.Entities.TodoList;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TodoListRepository : ITodoListRepository
{
    private readonly TodoListDbContext db;

    public TodoListRepository(TodoListDbContext db)
    {
        this.db = db;
    }

    public async Task<IEnumerable<TodoListEntity>> GetAllAsync()
    {
        return await this.db.TodoLists
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Assignee)

            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TodoListEntity?> GetByIdAsync(int id)
    {
        return await this.db.TodoLists
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Assignee)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<TodoListEntity> AddAsync(TodoListEntity todoList)
    {
        _ = this.db.TodoLists.Add(todoList);
        _ = await this.db.SaveChangesAsync();
        return todoList;
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByListIdAsync(int todoListId)
    {
        var entity = await this.db.TodoLists
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Assignee)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == todoListId);

        return entity?.Tasks ?? Enumerable.Empty<TaskEntity>();
    }

    public async Task<TodoListEntity> UpdateAsync(int id, TodoListEntity todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        var entity = await this.db.TodoLists
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Assignee)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (entity == null)
        {
            return todoList;
        }

        entity.Title = todoList.Title;
        entity.Description = todoList.Description;

        _ = await this.db.SaveChangesAsync();
        return entity;
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
}
