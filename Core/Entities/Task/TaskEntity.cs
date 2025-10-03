using Core.Entities.TodoList;
using TaskStatus = Core.Enums.TaskStatus;

namespace Core.Entities.Task;

public class TaskEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public string Assignee { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public TodoListEntity TodoList { get; set; } = null!;

    public List<TaskTagEntity> Tags { get; } = new List<TaskTagEntity>();

    public List<TaskCommentEntity> Comments { get; } = new List<TaskCommentEntity>();
}
