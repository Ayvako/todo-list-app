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
            Status = Core.Enums.TaskStatus.NotStarted,
        };

        var added = await this.repository.AddTaskAsync(todoListId, entity);

        added.Assignee ??= await this.userRepository.GetByIdAsync(userId);

        return MapToDto(added);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
        var entity = await this.repository.GetTaskByIdAsync(id);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<List<TaskDto>> GetAllAsync(int userId)
    {
        var entities = await this.repository.GetAllAsync(userId);
        return entities?.Select(l => MapToDto(l))
                   .ToList() ?? new List<TaskDto>();
    }

    public async Task<List<TaskDto>> GetAssignedTasksAsync(int userId, TaskStatus? status = TaskStatus.InProgress)
    {
        var entities = await this.repository.GetAssignedTasksAsync(userId, status);
        return entities?.Select(l => MapToDto(l))
                   .ToList() ?? new List<TaskDto>();
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
        existing.Status = dto.Status;

        if (!string.Equals(existing.Assignee?.UserName ?? string.Empty, dto.AssigneeName ?? string.Empty, StringComparison.OrdinalIgnoreCase))
        {
            existing.Assignee = await this.userRepository.GetUserByNameAsync(dto.AssigneeName);
        }

        var updated = await this.repository.UpdateTaskAsync(existing);
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

    public async Task<bool> ChangeStatusAsync(int id, int userId, ChangeStatusDto dto)
    {
        var existing = await this.repository.GetTaskByIdAsync(id);
        if (existing == null)
        {
            return false;
        }

        var isAssignee = existing.AssigneeId == userId;
        var canEditList = await this.todoListService.CanEditAsync(existing.TodoListId, userId);

        if (!canEditList && !isAssignee)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this task.");
        }

        return await this.repository.UpdateStatusAsync(id, dto.NewStatus);
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
            TodoListId = entity.TodoListId,
            Assignee = entity.Assignee == null
                ? null
                : new UserDto
                {
                    Id = entity.Assignee.Id,
                    Email = entity.Assignee.Email,
                    Role = entity.Assignee.Role,
                    UserName = entity.Assignee.UserName
                }
        };
    }
}
