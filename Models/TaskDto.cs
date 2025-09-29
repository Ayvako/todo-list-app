using System.Collections.ObjectModel;

namespace WebApi.Models;

public class TaskDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public string Assignee { get; set; } = string.Empty;

    public ReadOnlyCollection<string> Tags { get; } = new ReadOnlyCollection<string>(new List<string>());

    public ReadOnlyCollection<string> Comments { get; set; } = new ReadOnlyCollection<string>(new List<string>());
}
