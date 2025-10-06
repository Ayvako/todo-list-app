using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities.TodoList;
using Core.Entities.TodoUser;
using Core.Enums;

namespace Core.Entities;

public class TodoListAccessEntity
{
    public int Id { get; set; }

    [ForeignKey("UserEntity")]
    public int UserId { get; set; }

    public UserEntity User { get; set; } = null!;

    [ForeignKey("TodoListEntity")]
    public int TodoListId { get; set; }

    public TodoListEntity TodoList { get; set; } = null!;

    public TodoListAccessRole Role { get; set; }
}
