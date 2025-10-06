using System.ComponentModel.DataAnnotations;
using TaskStatus = Core.Enums.TaskStatus;

namespace Contracts.Tasks;

public class TaskEditDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public TaskStatus Status { get; set; }

    [Required]
    public int TodoListId { get; set; }

    [StringLength(100)]
    public string Assignee { get; set; } = string.Empty;
}
