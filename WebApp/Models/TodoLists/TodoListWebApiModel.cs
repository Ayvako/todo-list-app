using WebApp.Models.Tasks;

namespace WebApp.Models.TodoLists;

public class TodoListWebApiModel
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public List<TaskWebApiModel> Tasks { get; set; } = new List<TaskWebApiModel>();

    public bool CanEdit { get; set; }

    public bool CanView { get; set; }
}
