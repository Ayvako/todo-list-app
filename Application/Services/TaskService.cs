using Core.Entities.Task;
using Core.Interfaces;
using Contracts.Tasks;
using Application.Services.Interfaces;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository repository;
    private readonly ITodoListService todoListService;

    public TaskService(ITaskRepository repository, ITodoListService todoListService)
    {
        this.repository = repository;
        this.todoListService = todoListService;
    }

    public async Task<TaskDto> AddTaskAsync(int todoListId, TaskCreateDto dto, int userId)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var canEdit = await todoListService.CanEditAsync(todoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to add tasks to this list.");
        }

        var entity = new TaskEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Assignee = dto.Assignee,
            TodoListId = todoListId
        };

        var added = await repository.AddTaskAsync(todoListId, entity);
        return MapToDto(added);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
        var entity = await repository.GetTaskByIdAsync(id);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<TaskDto?> UpdateTaskAsync(int id, TaskEditDto dto, int userId)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existing = await repository.GetTaskByIdAsync(id);
        if (existing == null)
        {
            return null;
        }

        var canEdit = await todoListService.CanEditAsync(existing.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to edit this task.");
        }

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.DueDate = dto.DueDate;
        existing.Assignee = dto.Assignee;
        existing.Status = dto.Status;

        var updated = await repository.UpdateTaskAsync(id, existing);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteTaskAsync(int id, int userId)
    {
        var existing = await repository.GetTaskByIdAsync(id);
        if (existing == null)
        {
            return false;
        }

        var canEdit = await todoListService.CanEditAsync(existing.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this task.");
        }

        return await repository.DeleteTaskAsync(id);
    }

    private static TaskDto MapToDto(TaskEntity entity)
    {
        return new TaskDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            DueDate = entity.DueDate,
            Status = entity.Status,
            Assignee = entity.Assignee,
            TodoListId = entity.TodoListId
        };
    }
}
