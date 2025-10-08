using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities.TodoList;
using Core.Entities.TodoUser;
using Core.Enums;

namespace Core.Entities;

public class TodoListAccessEntity
{
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public UserEntity User { get; set; } = null!;

    public int TodoListId { get; set; }

    [ForeignKey("TodoListId")]
    public TodoListEntity TodoList { get; set; } = null!;

    public TodoListAccessRole Role { get; set; }
}
