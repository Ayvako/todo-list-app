using WebApi.Models;

namespace WebApi.Services.Models;

public class TodoList
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();
}
