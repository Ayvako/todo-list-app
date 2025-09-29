namespace WebApi.Models;

public class TaskModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public string Assignee { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();

    public List<string> Comments { get; set; } = new();
}
