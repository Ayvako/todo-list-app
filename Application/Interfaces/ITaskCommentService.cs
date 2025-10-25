using Contracts.Comments;

namespace Application.Interfaces;

public interface ITaskCommentService
{
    Task<IEnumerable<TaskCommentDto>> GetCommentsAsync(int taskId);

    Task<TaskCommentDto> AddCommentAsync(int taskId, int userId, TaskCommentCreateDto dto);

    Task<TaskCommentDto?> UpdateCommentAsync(int commentId, int userId, TaskCommentUpdateDto dto);

    Task<bool> DeleteCommentAsync(int commentId, int userId);
}
