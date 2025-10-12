using Core.Enums;

namespace Contracts.TodoLists;

public class ShareDto
{
    public string UserName { get; set; } = null!;
    public TodoListAccessRole Role { get; set; }
}
