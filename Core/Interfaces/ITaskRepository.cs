using Core.Entities.Task;

namespace Core.Interfaces;

public interface ITaskRepository
{
    Task<bool> DeleteTaskAsync(int id);

    Task<TaskEntity> AddTaskAsync(int todoListId, TaskEntity task);

    Task<TaskEntity?> GetTaskByIdAsync(int id);

    Task<TaskEntity?> UpdateTaskAsync(int id, TaskEntity updatedTask);

    Task<List<TaskEntity>> GetAssignedTasksAsync(int userId);
}
