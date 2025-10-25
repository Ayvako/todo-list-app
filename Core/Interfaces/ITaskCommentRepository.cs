using Core.Entities.Comment;

namespace Core.Interfaces;

public interface ITaskCommentRepository
{
    Task<List<CommentEntity>> GetCommentsByTaskIdAsync(int taskId);

    Task<CommentEntity> AddCommentAsync(CommentEntity comment);

    Task DeleteCommentAsync(int commentId);

    Task<CommentEntity?> UpdateCommentAsync(CommentEntity comment);

    Task<CommentEntity?> GetByIdAsync(int commentId);
}
