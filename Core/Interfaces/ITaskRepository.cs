using Core.Entities.Task;
using TaskStatus = Core.Enums.TaskStatus;

namespace Core.Interfaces;

public interface ITaskRepository
{
    Task<bool> DeleteTaskAsync(int id);

    Task<TaskEntity> AddTaskAsync(int todoListId, TaskEntity task);

    Task<TaskEntity?> GetTaskByIdAsync(int id);

    Task<TaskEntity?> UpdateTaskAsync(TaskEntity updatedTask);

    Task<List<TaskEntity>> GetAssignedTasksAsync(int userId, TaskStatus? status = TaskStatus.InProgress);

    Task<List<TaskEntity>> GetAllAsync(int userId);

    Task<bool> UpdateStatusAsync(int id, TaskStatus newStatus);

    Task<bool> AddTagAsync(int taskId, string tagName);

    Task<bool> RemoveTagAsync(int taskId, string tagName);

    Task<List<TagEntity>> GetTagsForUserAsync(int userId);

    Task<List<TaskEntity>> GetTasksByTagAsync(string tagName, int userId);
}
