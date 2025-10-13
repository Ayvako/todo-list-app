using Contracts.Tasks;

namespace Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> AddTaskAsync(int todoListId, TaskCreateDto dto, int userId);

    Task<TaskDto?> GetTaskByIdAsync(int id);

    Task<TaskDto?> UpdateTaskAsync(int id, TaskEditDto dto, int userId);

    Task<bool> DeleteTaskAsync(int id, int userId);

    Task<List<TaskDto>> GetAssignedTasksAsync(int userId);
}
