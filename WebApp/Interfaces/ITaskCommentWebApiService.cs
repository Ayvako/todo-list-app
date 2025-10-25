using WebApp.Models.Comments;

namespace WebApp.Interfaces;

public interface ITaskCommentWebApiService
{
    Task<IEnumerable<TaskCommentModel>> GetCommentsAsync(int taskId);

    Task<TaskCommentModel?> AddCommentAsync(int taskId, TaskCommentCreateModel model);

    Task<TaskCommentModel?> UpdateCommentAsync(int taskId, int commentId, TaskCommentUpdateModel model);

    Task<bool> DeleteCommentAsync(int taskId, int commentId);
}
