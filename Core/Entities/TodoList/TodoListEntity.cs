using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities.Task;
using Core.Entities.TodoUser;

namespace Core.Entities.TodoList;

public class TodoListEntity
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int OwnerId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public UserEntity Owner { get; set; } = null!;

    public List<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();

    public List<TodoListAccessEntity> AccessList { get; set; } = new();
}
