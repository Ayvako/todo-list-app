using Core.Entities.Task;
using Core.Entities.TodoList;
using Core.Enums;
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
                .ThenInclude(t => t.Tags)
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Comments)
            .Include(l => l.AccessList)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<TodoListEntity>> GetByUserIdAsync(int userId)
    {
        return await this.db.TodoLists
            .Where(l => l.OwnerId == userId || l.AccessList.Any(a => a.UserId == userId))
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Tags)
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Comments)
            .Include(l => l.AccessList)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TodoListEntity?> GetByIdAsync(int id)
    {
        return await this.db.TodoLists
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Tags)
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Comments)
            .Include(l => l.AccessList)
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
                .ThenInclude(t => t.Tags)
            .Include(l => l.Tasks)
                .ThenInclude(t => t.Comments)
            .Include(l => l.AccessList)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == todoListId);

        return entity?.Tasks ?? Enumerable.Empty<TaskEntity>();
    }

    public async Task<TodoListEntity> UpdateAsync(int id, TodoListEntity todoList)
    {
        ArgumentNullException.ThrowIfNull(todoList);

        var entity = await this.db.TodoLists
            .Include(l => l.Tasks)
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

        this.db.TodoListAccesses.RemoveRange(entity.AccessList);

        _ = this.db.TodoLists.Remove(entity);
        _ = await this.db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CanViewAsync(int listId, int userId)
    {
        var list = await this.db.TodoLists
            .Include(l => l.AccessList)
            .FirstOrDefaultAsync(l => l.Id == listId);

        if (list == null)
        {
            return false;
        }

        if (list.OwnerId == userId)
        {
            return true;
        }

        return list.AccessList.Any(a => a.UserId == userId);
    }

    public async Task<bool> CanEditAsync(int listId, int userId)
    {
        var list = await this.db.TodoLists
            .Include(l => l.AccessList)
            .FirstOrDefaultAsync(l => l.Id == listId);

        if (list == null)
        {
            return false;
        }

        if (list.OwnerId == userId)
        {
            return true;
        }

        var canEdit = list.AccessList.Any(a => a.UserId == userId && a.Role == TodoListAccessRole.Editor);
        return canEdit;
    }

    public async Task<bool> AddAccessAsync(int listId, int userId, TodoListAccessRole role)
    {
        var list = await this.db.TodoLists
            .Include(l => l.AccessList)
            .FirstOrDefaultAsync(l => l.Id == listId);

        if (list == null)
        {
            return false;
        }

        if (list.AccessList.Any(a => a.UserId == userId))
        {
            return false;
        }

        list.AccessList.Add(new TodoListAccessEntity
        {
            TodoListId = listId,
            UserId = userId,
            Role = role
        });

        _ = await this.db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAccessAsync(int listId, int userId)
    {
        var access = await this.db.TodoListAccesses
            .FirstOrDefaultAsync(a => a.TodoListId == listId && a.UserId == userId);

        if (access == null)
        {
            return false;
        }

        _ = this.db.TodoListAccesses.Remove(access);
        _ = await this.db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAccessRoleAsync(int listId, int userId, TodoListAccessRole newRole)
    {
        var access = await this.db.TodoListAccesses
            .FirstOrDefaultAsync(a => a.TodoListId == listId && a.UserId == userId);

        if (access == null)
        {
            return false;
        }

        access.Role = newRole;
        _ = await this.db.SaveChangesAsync();
        return true;
    }
}
