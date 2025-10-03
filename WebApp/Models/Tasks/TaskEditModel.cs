using System.ComponentModel.DataAnnotations;
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

    [Required]
    public string Assignee { get; set; } = string.Empty;
}
