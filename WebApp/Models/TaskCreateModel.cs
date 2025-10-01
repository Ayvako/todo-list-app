namespace WebApp.Models;

public class TaskCreateModel
{
    public int TodoListId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);

    public string Assignee { get; set; } = string.Empty;
}
