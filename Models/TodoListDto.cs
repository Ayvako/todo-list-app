using System.Collections.ObjectModel;

namespace WebApi.Models;

public class TodoListDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ReadOnlyCollection<TaskDto> Tasks { get; set; } = new ReadOnlyCollection<TaskDto>(new List<TaskDto>());
}
