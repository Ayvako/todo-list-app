namespace WebApi.Models;

#pragma warning disable CA1002
#pragma warning disable CA2227

public class TaskDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public string Assignee { get; set; } = string.Empty;

    public List<TaskTagDto> Tags { get; } = new List<TaskTagDto>();

    public List<TaskCommentDto> Comments { get; set; } = new List<TaskCommentDto>();
}
