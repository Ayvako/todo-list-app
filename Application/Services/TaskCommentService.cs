using Application.Interfaces;
using Contracts.Comments;
using Core.Entities.Comment;
using Core.Interfaces;

namespace Application.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly ITaskRepository taskRepository;
    private readonly ITaskCommentRepository commentRepository;
    private readonly IUserService userService;
    private readonly ITodoListService todoListService;

    public TaskCommentService(
        ITaskRepository taskRepository,
        IUserService userService,
        ITaskCommentRepository commentRepository,
        ITodoListService todoListService)
    {
        this.taskRepository = taskRepository;
        this.userService = userService;
        this.commentRepository = commentRepository;
        this.todoListService = todoListService;
    }

    public async Task<IEnumerable<TaskCommentDto>> GetCommentsAsync(int taskId)
    {
        var comments = await this.commentRepository.GetCommentsByTaskIdAsync(taskId);

        var userIds = comments.Select(c => c.UserId).Distinct();

        var users = await this.userService.GetByIdsAsync(userIds);

        var dtos = comments.Select(c => new TaskCommentDto
        {
            Id = c.Id,
            Text = c.Text,
            AuthorName = users.TryGetValue(c.UserId, out var u) ? u.UserName : "Unknown",
            AuthorId = c.UserId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });

        return dtos;
    }

    public async Task<TaskCommentDto> AddCommentAsync(int taskId, int userId, TaskCommentCreateDto dto)
    {
        ValidateAddCommentParameters(dto);

        var entity = new CommentEntity
        {
            TaskId = taskId,
            UserId = userId,
            Text = dto.Text,
        };

        return await this.AddCommentInternalAsync(entity, userId);
    }

    public async Task<TaskCommentDto?> UpdateCommentAsync(int commentId, int userId, TaskCommentUpdateDto dto)
    {
        ValidateUpdateCommentParameters(dto);

        return await this.UpdateCommentInternalAsync(commentId, userId, dto);
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int userId)
    {
        var comment = await this.commentRepository.GetByIdAsync(commentId);
        if (comment == null || comment.UserId != userId)
        {
            return false;
        }

        await this.commentRepository.DeleteCommentAsync(commentId);
        return true;
    }

    private async Task<TaskCommentDto> AddCommentInternalAsync(CommentEntity entity, int userId)
    {
        var existing = await this.taskRepository.GetTaskByIdAsync(entity.TaskId)
            ?? throw new KeyNotFoundException("Task not found.");

        var canEdit = await this.todoListService.CanEditAsync(existing.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to add comments to this task.");
        }

        _ = await this.commentRepository.AddCommentAsync(entity);

        var user = await this.userService.GetByIdAsync(userId);
        return new TaskCommentDto
        {
            Id = entity.Id,
            Text = entity.Text,
            AuthorName = user?.UserName ?? "Unknown",
            AuthorId = user!.Id,
            UpdatedAt = entity.UpdatedAt,
            CreatedAt = entity.CreatedAt
        };
    }

    private async Task<TaskCommentDto?> UpdateCommentInternalAsync(int commentId, int userId, TaskCommentUpdateDto dto)
    {
        var comment = await this.commentRepository.GetByIdAsync(commentId);
        if (comment == null || comment.UserId != userId)
        {
            return null;
        }

        comment.Text = dto.Text;
        comment.UpdatedAt = DateTime.UtcNow;
        _ = await this.commentRepository.UpdateCommentAsync(comment);

        var user = await this.userService.GetByIdAsync(comment.UserId);
        return new TaskCommentDto
        {
            Id = comment.Id,
            Text = comment.Text,
            AuthorName = user?.UserName ?? "Unknown",
            AuthorId = user!.Id,
            UpdatedAt = comment.UpdatedAt
        };
    }

    private static void ValidateAddCommentParameters(TaskCommentCreateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
    }

    private static void ValidateUpdateCommentParameters(TaskCommentUpdateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
    }
}
