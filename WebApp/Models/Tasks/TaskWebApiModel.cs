using TaskStatus = Core.Enums.TaskStatus;

namespace WebApp.Models.Tasks;

public class TaskWebApiModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime DueDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public int? AssigneeId { get; set; }

    public string? AssigneeName { get; set; }

    public int TodoListId { get; set; }
}
