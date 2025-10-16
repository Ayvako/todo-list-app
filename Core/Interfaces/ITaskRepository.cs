using Core.Entities.Task;
using TaskStatus = Core.Enums.TaskStatus;

namespace Core.Interfaces;

public interface ITaskRepository
{
    Task<bool> DeleteTaskAsync(int id);

    Task<TaskEntity> AddTaskAsync(int todoListId, TaskEntity task);

    Task<TaskEntity?> GetTaskByIdAsync(int id);

    Task<TaskEntity?> UpdateTaskAsync(TaskEntity updatedTask);

    Task<List<TaskEntity>> GetAssignedTasksAsync(int userId);

    Task<bool> UpdateStatusAsync(int id, TaskStatus newStatus);
}
