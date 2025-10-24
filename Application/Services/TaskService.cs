using Application.Interfaces;
using Application.Mappers;
using Contracts.Tasks;
using Core.Entities.Task;
using Core.Interfaces;
using TaskStatus = Core.Enums.TaskStatus;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository repository;
    private readonly ITodoListService todoListService;
    private readonly IUserService userService;

    public TaskService(ITaskRepository repository, ITodoListService todoListService, IUserService userService)
    {
        this.repository = repository;
        this.todoListService = todoListService;
        this.userService = userService;
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
            TodoListId = todoListId,
            AssigneeId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = TaskStatus.NotStarted,
        };

        var added = await this.repository.AddTaskAsync(todoListId, entity);

        //added.Assignee ??= await this.userRepository.GetByIdAsync(userId);

        return await this.MapToDtoAsync(added, userId);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id, int userId)
    {
        var entity = await this.repository.GetTaskByIdAsync(id) ?? throw new KeyNotFoundException("Task not found.");
        return await this.MapToDtoAsync(entity, userId);
    }

    public async Task<List<TaskDto>> GetAllAsync(int userId)
    {
        var entities = await this.repository.GetAllAsync(userId);
        var list = new List<TaskDto>();
        foreach (var entity in entities)
        {
            list.Add(await this.MapToDtoAsync(entity, userId));
        }
        return list;
    }

    public async Task<List<TaskDto>> GetAssignedTasksAsync(int userId, TaskStatus? status = TaskStatus.InProgress)
    {
        var entities = await this.repository.GetAssignedTasksAsync(userId, status);
        var list = new List<TaskDto>();
        foreach (var entity in entities)
        {
            list.Add(await this.MapToDtoAsync(entity, userId));
        }

        return list;
    }

    public async Task<TaskDto?> UpdateTaskAsync(int id, TaskEditDto dto, int userId)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existing = await this.repository.GetTaskByIdAsync(id) ?? throw new KeyNotFoundException("Task not found.");

        var canEdit = await this.todoListService.CanEditAsync(existing.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to edit this task.");
        }
        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.DueDate = dto.DueDate;
        existing.Status = dto.Status;

        if (!string.IsNullOrEmpty(dto.AssigneeName))
        {
            var user = await this.userService.GetUserByNameAsync(dto.AssigneeName);
            existing.AssigneeId = user?.Id;
        }

        var updated = await this.repository.UpdateTaskAsync(existing);
        return updated == null ? null : await this.MapToDtoAsync(updated, userId);
    }

    public async Task<bool> DeleteTaskAsync(int id, int userId)
    {
        var existing = await this.repository.GetTaskByIdAsync(id) ?? throw new KeyNotFoundException("Task not found.");

        var canEdit = await this.todoListService.CanEditAsync(existing.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this task.");
        }

        return await this.repository.DeleteTaskAsync(id);
    }

    public async Task<bool> ChangeStatusAsync(int id, int userId, ChangeStatusDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existing = await this.repository.GetTaskByIdAsync(id) ?? throw new KeyNotFoundException("Task not found.");
        var isAssignee = existing.AssigneeId == userId;
        var canEditList = await this.todoListService.CanEditAsync(existing.TodoListId, userId);

        if (!canEditList && !isAssignee)
        {
            throw new UnauthorizedAccessException("You don't have permission to change the status of this task.");
        }

        return await this.repository.UpdateStatusAsync(id, dto.NewStatus);
    }

    public async Task<List<TaskDto?>> GetTasksByTagAsync(string tagName, int userId)
    {
        var entities = await this.repository.GetTasksByTagAsync(tagName, userId);
        var list = new List<TaskDto?>();
        foreach (var entity in entities)
        {
            list.Add(await this.MapToDtoAsync(entity, userId));
        }

        return list;
    }

    private async Task<TaskDto> MapToDtoAsync(TaskEntity entity, int userId)
    {
        var dto = await entity.ToDtoAsync(this.userService);
        dto.CanEdit = await this.todoListService.CanEditAsync(entity.TodoListId, userId);
        dto.IsAssignee = entity.AssigneeId == userId;
        return dto;
    }
}
