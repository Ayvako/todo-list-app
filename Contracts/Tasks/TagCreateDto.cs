using System.ComponentModel.DataAnnotations;

namespace Contracts.Tasks;

public class TagCreateDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}
