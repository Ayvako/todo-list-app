using Application.Interfaces;
using Contracts.TodoLists;
using Core.Entities.TodoList;
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
        var entities = await this.repository.GetAllAsync();
        return entities.Select(e => MapToWebApiModel(e, currentUserId)).ToList();
    }

    public async Task<IEnumerable<TodoListWebApiModel>> GetByUserAsync(int userId)
    {
        var entities = await this.repository.GetByUserIdAsync(userId);
        return entities.Select(e => MapToWebApiModel(e, userId)).ToList();
    }

    public async Task<TodoListWebApiModel?> GetByIdAsync(int id, int currentUserId)
    {
        var entity = await this.repository.GetByIdAsync(id);
        return entity == null ? null : MapToWebApiModel(entity, currentUserId);
    }

    public async Task<IEnumerable<TaskWebApiModel>> GetTasksByListIdAsync(int id)
    {
        var tasks = await this.repository.GetTasksByListIdAsync(id);
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
            EditorIds = new List<int>(),
            ViewerIds = new List<int>()
        };

        var added = await this.repository.AddAsync(entity);
        return MapToWebApiModel(added, userId);
    }

    public async Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListUpdateDto model, int userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var canEdit = await this.CanEditAsync(id, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to edit this list.");
        }

        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description
        };

        var updated = await this.repository.UpdateAsync(id, entity);
        return updated == null ? null : MapToWebApiModel(updated, userId);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var canEdit = await this.CanEditAsync(id, userId);
        return !canEdit
            ? throw new UnauthorizedAccessException("You don't have permission to delete this list.")
            : await this.repository.DeleteAsync(id);
    }

    public async Task<bool> CanEditAsync(int todoListId, int userId)
    {
        var list = await this.repository.GetByIdAsync(todoListId);
        return list != null && (list.OwnerId == userId || (list.EditorIds?.Contains(userId) ?? false));
    }

    public async Task<bool> CanViewAsync(int todoListId, int userId)
    {
        var list = await this.repository.GetByIdAsync(todoListId);
        return list != null && (list.OwnerId == userId
            || (list.EditorIds?.Contains(userId) ?? false)
            || (list.ViewerIds?.Contains(userId) ?? false));
    }

    private static TodoListWebApiModel MapToWebApiModel(TodoListEntity entity, int userId)
    {
        return new TodoListWebApiModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Tasks = entity.Tasks?.Select(MapTaskToWebApiModel).ToList() ?? new List<TaskWebApiModel>(),
            CanEdit = entity.OwnerId == userId || (entity.EditorIds?.Contains(userId) ?? false),
            CanView = entity.OwnerId == userId || (entity.EditorIds?.Contains(userId) ?? false) || (entity.ViewerIds?.Contains(userId) ?? false)
        };
    }

    private static TaskWebApiModel MapTaskToWebApiModel(Core.Entities.Task.TaskEntity task)
    {
        return new TaskWebApiModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Status = task.Status,
            AssigneeId = task.AssigneeId,
            TodoListId = task.TodoListId,
            AssigneeName = task.Assignee?.UserName ?? null,
        };
    }
}
