using System.Diagnostics.CodeAnalysis;

namespace WebApi.Models;

#pragma warning disable IDE0079 // Remove unnecessary suppression

[SuppressMessage("Maintainability", "CA1515", Justification = "Controllers must be public for Swagger to work")]
public class TodoListEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
}
