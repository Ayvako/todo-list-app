using System.ComponentModel.DataAnnotations;
using WebApp.Models.Tags;
using WebApp.Models.Users;
using TaskStatus = Core.Enums.TaskStatus;

namespace WebApp.Models.Tasks;

public class TaskEditModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public TaskStatus Status { get; set; }

    public UserWebApiModel? Assignee { get; set; }

    public string? AssigneeName { get; set; }

    public int? AssigneeId { get; set; }

    public ICollection<TagModel> Tags { get; set; } = new List<TagModel>();
}
