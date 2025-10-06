using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities.Task;

namespace Core.Entities.TodoList;

public class TodoListEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int OwnerId { get; set; }

    public List<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();

    [NotMapped]
    public List<int>? EditorIds { get; set; } = new List<int>();

    [NotMapped]
    public List<int>? ViewerIds { get; set; } = new List<int>();

    public List<TodoListAccessEntity> AccessList { get; set; } = new();
}
