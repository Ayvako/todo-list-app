using Contracts.Tasks;

namespace Contracts.TodoLists;

public class TodoListDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<TaskDto> Tasks { get; set; } = new();
}
