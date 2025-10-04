using Contracts.TodoLists;
using Core.Entities.Task;
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

    public async Task<IEnumerable<TodoListWebApiModel>> GetAllAsync()
    {
        var entities = await this.repository.GetAllAsync();
        return entities.Select(MapToWebApiModel).ToList();
    }

    public async Task<TodoListWebApiModel?> GetByIdAsync(int id)
    {
        var entity = await this.repository.GetByIdAsync(id);
        return entity == null ? null : MapToWebApiModel(entity);
    }

    public async Task<IEnumerable<TaskWebApiModel>> GetTasksByListIdAsync(int id)
    {
        var tasks = await this.repository.GetTasksByListIdAsync(id);
        return tasks.Select(MapTaskToWebApiModel).ToList();
    }

    public async Task<TodoListWebApiModel> AddAsync(TodoListCreateDto model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description,
            Tasks = new List<TaskEntity>()
        };

        var added = await this.repository.AddAsync(entity);
        return MapToWebApiModel(added);
    }

    public async Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListUpdateDto model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description
        };

        var updated = await this.repository.UpdateAsync(id, entity);
        return updated == null ? null : MapToWebApiModel(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await this.repository.DeleteAsync(id);
    }

    public async Task<bool> CanEditAsync(int todoListId, int userId)
    {
        var list = await this.repository.GetByIdAsync(todoListId);
        if (list == null)
        {
            return false;
        }

        return list.OwnerId == userId || (list.EditorIds?.Contains(userId) ?? false);
    }

    public async Task<bool> CanViewAsync(int todoListId, int userId)
    {
        var list = await this.repository.GetByIdAsync(todoListId);
        if (list == null)
        {
            return false;
        }

        return list.OwnerId == userId || (list.EditorIds?.Contains(userId) ?? false) || (list.ViewerIds?.Contains(userId) ?? false);
    }

    private static TodoListWebApiModel MapToWebApiModel(TodoListEntity entity)
    {
        return new TodoListWebApiModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Tasks = entity.Tasks?.Select(MapTaskToWebApiModel).ToList() ?? new List<TaskWebApiModel>()
        };
    }

    private static TaskWebApiModel MapTaskToWebApiModel(TaskEntity task)
    {
        return new TaskWebApiModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Status = task.Status,
            Assignee = task.Assignee
        };
    }
}
