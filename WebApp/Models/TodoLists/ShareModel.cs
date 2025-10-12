namespace WebApp.Models.TodoLists;

public class ShareModel
{
    public int TodoListId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public Core.Enums.TodoListAccessRole Role { get; set; }
}
