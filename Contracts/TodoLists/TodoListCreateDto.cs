using System.ComponentModel.DataAnnotations;

namespace Contracts.TodoLists;

public class TodoListCreateDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; } = string.Empty;
}
