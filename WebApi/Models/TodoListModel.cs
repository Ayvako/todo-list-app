namespace WebApi.Models;

public class TodoListModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();
}
