using Core.Entities.Task;
using Core.Interfaces;
using Contracts.Tasks;
using Application.Interfaces;

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

        var canEdit = await this.todoListService.CanEditAsync(todoListId, userId);
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

        var added = await this.repository.AddTaskAsync(todoListId, entity);
        return MapToDto(added);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
        var entity = await this.repository.GetTaskByIdAsync(id);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<TaskDto?> UpdateTaskAsync(int id, TaskEditDto dto, int userId)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existing = await this.repository.GetTaskByIdAsync(id);
        if (existing == null)
        {
            return null;
        }

        var canEdit = await this.todoListService.CanEditAsync(existing.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to edit this task.");
        }

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.DueDate = dto.DueDate;
        existing.Assignee = dto.Assignee;
        existing.Status = dto.Status;

        var updated = await this.repository.UpdateTaskAsync(id, existing);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteTaskAsync(int id, int userId)
    {
        var existing = await this.repository.GetTaskByIdAsync(id);
        if (existing == null)
        {
            return false;
        }

        var canEdit = await this.todoListService.CanEditAsync(existing.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this task.");
        }

        return await this.repository.DeleteTaskAsync(id);
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
