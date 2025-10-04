using Core.Entities.TodoUser;

namespace Core.Entities.Task;

public class TaskCommentEntity
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public int UserId { get; set; }

    public UserEntity User { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TaskId { get; set; }

    public TaskEntity Task { get; set; } = null!;
}
