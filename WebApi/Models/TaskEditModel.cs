using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class TaskEditModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
}
