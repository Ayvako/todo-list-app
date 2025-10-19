using Contracts.Tasks;
using TaskStatus = Core.Enums.TaskStatus;

namespace Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> AddTaskAsync(int todoListId, TaskCreateDto dto, int userId);

    Task<TaskDto?> GetTaskByIdAsync(int id);

    Task<TaskDto?> UpdateTaskAsync(int id, TaskEditDto dto, int userId);

    Task<bool> DeleteTaskAsync(int id, int userId);

    Task<List<TaskDto>> GetAssignedTasksAsync(int userId, TaskStatus? status = TaskStatus.InProgress);

    Task<List<TaskDto>> GetAllAsync(int userId);

    Task<bool> ChangeStatusAsync(int id, int userId, ChangeStatusDto dto);

    Task<bool> AddTagAsync(int taskId, string tagName, int userId);

    Task<bool> RemoveTagAsync(int taskId, string tagName, int userId);
}
