using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class TaskEditModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public WebApi.Models.TaskStatus Status { get; set; }

    [Required]
    public int TodoListId { get; set; }
}
