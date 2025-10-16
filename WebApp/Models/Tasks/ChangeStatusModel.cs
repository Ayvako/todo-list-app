using TaskStatus = Core.Enums.TaskStatus;

namespace WebApp.Models.Tasks;

public class ChangeStatusModel
{
    public TaskStatus NewStatus { get; set; }
}
