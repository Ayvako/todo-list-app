using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.TodoLists;

public class TodoListUpdateWebApiModel
{
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters long")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;
}
