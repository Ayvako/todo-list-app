using System.ComponentModel.DataAnnotations;

namespace Contracts.Tasks;

public class TaskCommentDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public int TaskId { get; set; }

    public string User { get; set; } = string.Empty;
}
