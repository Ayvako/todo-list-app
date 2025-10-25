using Core.Entities.Comment;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TaskCommentRepository : ITaskCommentRepository
{
    private readonly TodoListDbContext db;

    public TaskCommentRepository(TodoListDbContext context) => this.db = context;

    public async Task<List<CommentEntity>> GetCommentsByTaskIdAsync(int taskId) =>
        await this.db.Comments.Where(c => c.TaskId == taskId).OrderBy(c => c.CreatedAt).ToListAsync();

    public async Task<CommentEntity> AddCommentAsync(CommentEntity comment)
    {
        _ = this.db.Comments.Add(comment);
        _ = await this.db.SaveChangesAsync();
        return comment;
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await this.db.Comments.FindAsync(commentId);
        if (comment != null)
        {
            _ = this.db.Comments.Remove(comment);
            _ = await this.db.SaveChangesAsync();
        }
    }

    public async Task<CommentEntity?> UpdateCommentAsync(CommentEntity comment)
    {
        ValidateComment(comment);
        return await this.UpdateCommentInternalAsync(comment);
    }

    public async Task<CommentEntity?> GetByIdAsync(int commentId)
    {
        return await this.db.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }

    private async Task<CommentEntity?> UpdateCommentInternalAsync(CommentEntity comment)
    {
        var entity = await this.db.Comments.FindAsync(comment.Id);
        if (entity != null)
        {
            entity.Text = comment.Text;
            entity.UpdatedAt = DateTime.UtcNow;
            _ = await this.db.SaveChangesAsync();
        }
        return entity;
    }

    private static void ValidateComment(CommentEntity comment)
    {
        ArgumentNullException.ThrowIfNull(comment);
    }
}
