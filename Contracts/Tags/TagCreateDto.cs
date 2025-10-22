using System.ComponentModel.DataAnnotations;

namespace Contracts.Tags;

public class TagCreateDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}
