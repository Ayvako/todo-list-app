namespace WebApi.Models;

public class TodoListEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
}
