using System.Diagnostics.CodeAnalysis;
using WebApi.Models;

namespace WebApi.Services.Models;

#pragma warning disable IDE0079 // Remove unnecessary suppression

[SuppressMessage("Maintainability", "CA1515", Justification = "Controllers must be public for Swagger to work")]
public class TodoList
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();
}
