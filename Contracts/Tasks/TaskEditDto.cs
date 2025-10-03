using TaskStatus = Core.Enums.TaskStatus;

namespace Contracts.Tasks;

public class TaskEditDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public TaskStatus Status { get; set; }

    public int TodoListId { get; set; }
}
