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
        var entities = await repository.GetAllAsync();
        return entities.Select(MapToWebApiModel).ToList();
    }

    public async Task<TodoListWebApiModel?> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        return entity == null ? null : MapToWebApiModel(entity);
    }

    public async Task<IEnumerable<TaskWebApiModel>> GetTasksByListIdAsync(int id)
    {
        var tasks = await repository.GetTasksByListIdAsync(id);
        return tasks.Select(MapTaskToWebApiModel).ToList();
    }

    public async Task<TodoListWebApiModel> AddAsync(TodoListCreateDto model)
    {
        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description,
            Tasks = new List<TaskEntity>()
        };

        var added = await repository.AddAsync(entity);
        return MapToWebApiModel(added);
    }

    public async Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListUpdateDto model)
    {
        var entity = new TodoListEntity
        {
            Title = model.Title,
            Description = model.Description
        };

        var updated = await repository.UpdateAsync(id, entity);
        return updated == null ? null : MapToWebApiModel(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await repository.DeleteAsync(id);
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
