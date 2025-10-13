namespace WebApp.Models.TodoLists;

public class TodoListCreateModel
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;
}
