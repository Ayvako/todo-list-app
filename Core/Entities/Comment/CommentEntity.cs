using Core.Entities.Task;

namespace Core.Entities.Comment;

public class CommentEntity
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int TaskId { get; set; }

    public TaskEntity Task { get; set; } = null!;
}
