namespace Core.Entities.Task;

public class TagEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<TaskEntity> Tasks { get; set; } = new HashSet<TaskEntity>();
}
