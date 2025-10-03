namespace Core.Entities.Task;

public class TaskTagEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int TaskId { get; set; }

    public TaskEntity Task { get; set; } = null!;
}
