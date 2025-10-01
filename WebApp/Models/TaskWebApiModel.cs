namespace WebApp.Models;

public class TaskWebApiModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime DueDate { get; set; }

    public WebApi.Models.TaskStatus Status { get; set; } = WebApi.Models.TaskStatus.NotStarted;

    public string Assignee { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public TodoListWebApiModel TodoList { get; set; }
}
