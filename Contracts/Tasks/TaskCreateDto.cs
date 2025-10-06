using System.ComponentModel.DataAnnotations;

namespace Contracts.Tasks;

public class TaskCreateDto
{
    [Required]
    public int TodoListId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);

    [StringLength(100)]
    public string Assignee { get; set; } = string.Empty;
}
