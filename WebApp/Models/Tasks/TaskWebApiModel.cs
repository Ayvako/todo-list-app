using WebApp.Models.Users;
using TaskStatus = Core.Enums.TaskStatus;

namespace WebApp.Models.Tasks;

public class TaskWebApiModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime DueDate { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;

    public UserWebApiModel? Assignee { get; set; }

    public int TodoListId { get; set; }

    public bool IsAssignee { get; set; }

    public bool CanEdit { get; set; }

    public ICollection<TagModel> Tags { get; set; } = new List<TagModel>();
}
