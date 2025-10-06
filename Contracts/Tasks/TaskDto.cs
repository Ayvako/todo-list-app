using TaskStatus = Core.Enums.TaskStatus;

namespace Contracts.Tasks;

public class TaskDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public string Assignee { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public List<TaskTagDto> Tags { get; set; } = new();

    public List<TaskCommentDto> Comments { get; set; } = new();
}
