using System.ComponentModel.DataAnnotations;

namespace Contracts.Tasks;

public class TaskTagDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int TaskId { get; set; }
}
