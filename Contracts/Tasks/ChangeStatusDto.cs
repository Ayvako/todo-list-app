using TaskStatus = Core.Enums.TaskStatus;

namespace Contracts.Tasks;

public class ChangeStatusDto
{
    public TaskStatus NewStatus { get; set; }
}
