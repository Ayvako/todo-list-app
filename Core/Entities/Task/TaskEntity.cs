using Core.Entities.Comment;
using Core.Entities.Tag;
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

    public int? AssigneeId { get; set; }

    public int TodoListId { get; set; }

    public TodoListEntity TodoList { get; set; } = null!;

    public ICollection<TagEntity> Tags { get; set; } = new HashSet<TagEntity>();

    public ICollection<CommentEntity> Comments { get; set; } = new HashSet<CommentEntity>();
}
