using Core.Entities.Task;
using Core.Interfaces;
using WebApp.Models.Tasks;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository repository;

    public TaskService(ITaskRepository repository)
    {
        this.repository = repository;
    }

    public async Task<TaskWebApiModel> AddTaskAsync(int todoListId, TaskCreateModel model)
    {
        var entity = new TaskEntity
        {
            Title = model.Title,
            Description = model.Description,
            DueDate = model.DueDate,
            Assignee = model.Assignee,
        };

        var added = await repository.AddTaskAsync(todoListId, entity);

        return MapToWebApiModel(added);
    }

    public async Task<TaskWebApiModel?> GetTaskByIdAsync(int id)
    {
        var entity = await repository.GetTaskByIdAsync(id);
        return entity == null ? null : MapToWebApiModel(entity);
    }

    public async Task<TaskWebApiModel?> UpdateTaskAsync(int id, TaskEditModel model)
    {
        var entity = new TaskEntity
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            DueDate = model.DueDate,
            Assignee = model.Assignee,
            Status = model.Status
        };

        var updated = await repository.UpdateTaskAsync(id, entity);
        return updated == null ? null : MapToWebApiModel(updated);
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        return await repository.DeleteTaskAsync(id);
    }

    private static TaskWebApiModel MapToWebApiModel(TaskEntity entity)
    {
        return new TaskWebApiModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            DueDate = entity.DueDate,
            CreatedAt = entity.CreatedAt,
            Assignee = entity.Assignee,
            Status = entity.Status
        };
    }
}
