using Application.Interfaces;
using Contracts.Tasks;
using Contracts.Users;
using Core.Entities.Task;
using Core.Interfaces;
using TaskStatus = Core.Enums.TaskStatus;

namespace Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository repository;
    private readonly IUserRepository userRepository;
    private readonly ITodoListService todoListService;

    public TaskService(ITaskRepository repository, ITodoListService todoListService, IUserRepository userRepository)
    {
        this.repository = repository;
        this.todoListService = todoListService;
        this.userRepository = userRepository;
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

        added.Assignee ??= await this.userRepository.GetByIdAsync(userId);

        return await this.MapToDto(added, userId);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id, int userId)
    {
        var entity = await this.repository.GetTaskByIdAsync(id) ?? throw new KeyNotFoundException("Task not found.");
        return await this.MapToDto(entity, userId);
    }

    public async Task<List<TaskDto>> GetAllAsync(int userId)
    {
        var entities = await this.repository.GetAllAsync(userId);
        var list = new List<TaskDto>();
        foreach (var entity in entities)
        {
            list.Add(await this.MapToDto(entity, userId));
        }
        return list;
    }

    public async Task<List<TaskDto>> GetAssignedTasksAsync(int userId, TaskStatus? status = TaskStatus.InProgress)
    {
        var entities = await this.repository.GetAssignedTasksAsync(userId, status);
        var list = new List<TaskDto>();
        foreach (var entity in entities)
        {
            list.Add(await this.MapToDto(entity, userId));
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

        if (!string.Equals(existing.Assignee?.UserName ?? string.Empty, dto.AssigneeName ?? string.Empty, StringComparison.OrdinalIgnoreCase))
        {
            existing.Assignee = await this.userRepository.GetUserByNameAsync(dto.AssigneeName);
        }

        var updated = await this.repository.UpdateTaskAsync(existing);
        return updated == null ? null : await this.MapToDto(updated, userId);
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

    public async Task<bool> AddTagAsync(int taskId, string tagName, int userId)
    {
        var task = await this.repository.GetTaskByIdAsync(taskId) ?? throw new KeyNotFoundException("Task not found.");
        var canEdit = await this.todoListService.CanEditAsync(task.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to add tags to this task.");
        }

        return await this.repository.AddTagAsync(taskId, tagName);
    }

    public async Task<bool> RemoveTagAsync(int taskId, string tagName, int userId)
    {
        var task = await this.repository.GetTaskByIdAsync(taskId) ?? throw new KeyNotFoundException("Task not found.");
        var canEdit = await this.todoListService.CanEditAsync(task.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to remove tags from this task.");
        }

        return await this.repository.RemoveTagAsync(taskId, tagName);
    }

    public async Task<List<TagDto>> GetTagsForUserAsync(int userId)
    {
        var tags = await this.repository.GetTagsForUserAsync(userId);
        return tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
        }).ToList();
    }

    public async Task<List<TaskDto?>> GetTasksByTagAsync(string tagName, int userId)
    {
        var entities = await this.repository.GetTasksByTagAsync(tagName, userId);
        var list = new List<TaskDto>();
        foreach (var entity in entities)
        {
            list.Add(await this.MapToDto(entity, userId));
        }

        return list;
    }

    private async Task<TaskDto> MapToDto(TaskEntity entity, int userId)
    {
        var canEdit = await this.todoListService.CanEditAsync(entity.TodoListId, userId);

        return new TaskDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            DueDate = entity.DueDate,
            Status = entity.Status,
            TodoListId = entity.TodoListId,
            CanEdit = canEdit,
            IsAssignee = entity.AssigneeId == userId,
            Assignee = entity.Assignee == null
                ? null
                : new UserDto
                {
                    Id = entity.Assignee.Id,
                    Email = entity.Assignee.Email,
                    Role = entity.Assignee.Role,
                    UserName = entity.Assignee.UserName
                },
            Tags = entity.Tags?.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList() ?? new List<TagDto>(),
        };
    }
}
