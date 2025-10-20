using Contracts.Users;
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

    public UserDto? Assignee { get; set; }

    public int TodoListId { get; set; }

    public bool CanEdit { get; set; }

    public bool IsAssignee { get; set; }

    public List<TagDto> Tags { get; set; } = new();

    public List<TaskCommentDto> Comments { get; set; } = new();
}
