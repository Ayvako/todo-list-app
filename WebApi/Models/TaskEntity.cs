using System.Diagnostics.CodeAnalysis;

namespace WebApi.Models;

#pragma warning disable IDE0079 // Remove unnecessary suppression

[SuppressMessage("Maintainability", "CA1515", Justification = "Controllers must be public for Swagger to work")]
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

    public List<TaskTagEntity> Tags { get; set; } = new();

    public List<TaskCommentEntity> Comments { get; set; } = new();
}
