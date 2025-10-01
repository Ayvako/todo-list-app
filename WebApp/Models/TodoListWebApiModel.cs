namespace WebApp.Models;

public class TodoListWebApiModel
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public List<TaskWebApiModel> Tasks { get; set; } = new List<TaskWebApiModel>();
}
