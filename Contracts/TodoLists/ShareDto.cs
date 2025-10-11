using Core.Enums;

namespace Contracts.TodoLists;

public class ShareDto
{
    public int UserId { get; set; }
    public TodoListAccessRole Role { get; set; }
}
