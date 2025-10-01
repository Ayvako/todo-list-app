namespace WebApi.Models;

public class TaskCommentDto
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TaskId { get; set; }

    public TaskDto Task { get; set; } = null!;
}
