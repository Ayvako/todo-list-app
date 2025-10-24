using Core.Enums;

namespace Core.Entities.TodoList;

public class TodoListAccessEntity
{
    public int UserId { get; set; }

    public int TodoListId { get; set; }

    public TodoListEntity TodoList { get; set; } = null!;

    public TodoListAccessRole Role { get; set; }
}
