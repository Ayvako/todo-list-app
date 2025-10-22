using Application.Interfaces;
using Application.Mappers;
using Contracts.Tasks;
using Contracts.TodoLists;
using Contracts.Users;
using Core.Entities.Task;
using Core.Entities.TodoList;
using Core.Enums;
using Core.Interfaces;

namespace Application.Services;

public class TodoListService : ITodoListService
{
    private readonly ITodoListRepository repository;

    public TodoListService(ITodoListRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IEnumerable<TodoListDto>> GetAllAsync(int currentUserId)
    {
        var allLists = await this.repository.GetAllAsync();

        var accessibleLists = new List<TodoListEntity>();
        foreach (var list in allLists)
        {
            if (await this.repository.CanViewAsync(list.Id, currentUserId))
            {
                accessibleLists.Add(list);
            }
        }

        return accessibleLists
            .Select(l => l.ToDto(currentUserId)).ToList();
    }

    public async Task<IEnumerable<TodoListDto>> GetByUserAsync(int userId)
    {
        var entities = await this.repository.GetByUserIdAsync(userId);
        return entities
            .Select(l => l.ToDto(userId)).ToList();
    }

    public async Task<TodoListDto?> GetByIdAsync(int listId, int currentUserId)
    {
        var canView = await this.CanViewAsync(listId, currentUserId);
        if (!canView)
        {
            throw new UnauthorizedAccessException("You don't have permission to view this list.");
        }

        var entity = await this.repository.GetByIdAsync(listId);
        return entity?.ToDto(currentUserId) ?? null;
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByListIdAsync(int listId, int currentUserId)
    {
        var canView = await this.CanViewAsync(listId, currentUserId);
        if (!canView)
        {
            throw new UnauthorizedAccessException("You don't have permission to view tasks in this list.");
        }

        var tasks = await this.repository.GetTasksByListIdAsync(listId);

        return tasks.Select(l => l.ToDto()).ToList();
    }

    public async Task<TodoListDto> AddAsync(TodoListCreateDto model, int userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description,
            OwnerId = userId,
        };

        var added = await this.repository.AddAsync(entity);

        return added.ToDto(userId);
    }

    public async Task<TodoListDto?> UpdateAsync(int listId, TodoListUpdateDto model, int userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var canEdit = await this.CanEditAsync(listId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to edit this list.");
        }

        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description
        };

        var updated = await this.repository.UpdateAsync(listId, entity);

        return updated.ToDto(userId);
    }

    public async Task<bool> DeleteAsync(int listId, int userId)
    {
        var canEdit = await this.CanEditAsync(listId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this list.");
        }

        return await this.repository.DeleteAsync(listId);
    }

    public async Task<bool> ShareAsync(int listId, int targetUserId, TodoListAccessRole role, int currentUserId)
    {
        var list = await this.repository.GetByIdAsync(listId) ?? throw new KeyNotFoundException("List not found.");
        if (list.OwnerId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the owner can share the list.");
        }

        if (targetUserId == list.OwnerId)
        {
            throw new InvalidOperationException("You cannot share the list with its owner.");
        }
        var alreadyShared = list.AccessList.Any(a => a.UserId == targetUserId);

        if (alreadyShared)
        {
            throw new InvalidOperationException("The user already has access to this list.");
        }

        var added = await this.repository.AddAccessAsync(listId, targetUserId, role);
        return added;
    }

    public async Task<bool> RevokeAccessAsync(int listId, int targetUserId, int currentUserId)
    {
        var list = await this.repository.GetByIdAsync(listId) ?? throw new KeyNotFoundException("List not found.");
        if (list.OwnerId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the owner can revoke access.");
        }

        var removed = await this.repository.RemoveAccessAsync(listId, targetUserId);
        return removed;
    }

    public Task<bool> CanEditAsync(int todoListId, int userId)
    => this.repository.CanEditAsync(todoListId, userId);

    public Task<bool> CanViewAsync(int todoListId, int userId)
        => this.repository.CanViewAsync(todoListId, userId);
}
