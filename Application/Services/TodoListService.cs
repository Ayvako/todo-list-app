using Application.Interfaces;
using Contracts.TodoLists;
using Core.Entities.TodoList;
using Core.Enums;
using Core.Interfaces;
using WebApp.Models.Tasks;
using WebApp.Models.TodoLists;

namespace Application.Services;

public class TodoListService : ITodoListService
{
    private readonly ITodoListRepository repository;

    public TodoListService(ITodoListRepository repository)
    {
        this.repository = repository;
    }

    public async Task<IEnumerable<TodoListWebApiModel>> GetAllAsync(int currentUserId)
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

        return accessibleLists.Select(MapToWebApiModel).ToList();
    }

    public async Task<IEnumerable<TodoListWebApiModel>> GetByUserAsync(int userId)
    {
        var entities = await this.repository.GetByUserIdAsync(userId);
        return entities.Select(MapToWebApiModel).ToList();
    }

    public async Task<TodoListWebApiModel?> GetByIdAsync(int listId, int currentUserId)
    {
        var canView = await this.CanViewAsync(listId, currentUserId);
        if (!canView)
        {
            throw new UnauthorizedAccessException("You don't have permission to view this list.");
        }

        var entity = await this.repository.GetByIdAsync(listId);
        return entity == null ? null : MapToWebApiModel(entity);
    }

    public async Task<IEnumerable<TaskWebApiModel>> GetTasksByListIdAsync(int listId, int currentUserId)
    {
        var canView = await this.CanViewAsync(listId, currentUserId);
        if (!canView)
        {
            throw new UnauthorizedAccessException("You don't have permission to view tasks in this list.");
        }

        var tasks = await this.repository.GetTasksByListIdAsync(listId);
        return tasks.Select(MapTaskToWebApiModel).ToList();
    }

    public async Task<TodoListWebApiModel> AddAsync(TodoListCreateDto model, int userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description,
            OwnerId = userId,
        };

        var added = await this.repository.AddAsync(entity);
        return MapToWebApiModel(added);
    }

    public async Task<TodoListWebApiModel?> UpdateAsync(int listId, TodoListUpdateDto model, int userId)
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
        return updated == null ? null : MapToWebApiModel(updated);
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
        var list = await this.repository.GetByIdAsync(listId);
        if (list == null)
        {
            throw new KeyNotFoundException("List not found.");
        }

        if (list.OwnerId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the owner can share the list.");
        }

        var added = await this.repository.AddAccessAsync(listId, targetUserId, role);
        return added;
    }

    public async Task<bool> RevokeAccessAsync(int listId, int targetUserId, int currentUserId)
    {
        var list = await this.repository.GetByIdAsync(listId);
        if (list == null)
        {
            throw new KeyNotFoundException("List not found.");
        }

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

    private static TodoListWebApiModel MapToWebApiModel(TodoListEntity entity)
    {
        return new TodoListWebApiModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Tasks = entity.Tasks?.Select(MapTaskToWebApiModel).ToList() ?? new List<TaskWebApiModel>(),
        };
    }

    private static TaskWebApiModel MapTaskToWebApiModel(Core.Entities.Task.TaskEntity task)
        => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Status = task.Status,
            AssigneeId = task.AssigneeId,
            TodoListId = task.TodoListId,
            AssigneeName = task.Assignee?.UserName,
        };
}
