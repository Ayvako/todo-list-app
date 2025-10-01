namespace WebApi.Models;

public class TaskTagDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int TaskId { get; set; }

    public TaskDto Task { get; set; } = null!;
}
