using WebApp.Models.Tasks;
using TaskStatus = Core.Enums.TaskStatus;

namespace WebApp.Interfaces;

public interface ITaskWebApiService
{
    Task<TaskWebApiModel?> GetTaskByIdAsync(int id);

    Task<TaskWebApiModel?> AddTaskAsync(int todoListId, TaskCreateModel model);

    Task<bool> DeleteTaskAsync(int id);

    Task<TaskWebApiModel?> UpdateTaskAsync(int id, TaskEditModel model);

    Task<IEnumerable<TaskWebApiModel?>> GetAssignedTasksAsync(TaskStatus? status = TaskStatus.InProgress);

    Task<bool> ChangeStatusAsync(int id, ChangeStatusModel model);

    Task<IEnumerable<TaskWebApiModel?>> GetAllAsync();

    Task<List<TaskWebApiModel?>> GetFilteredTasksAsync(
        string? searchTitle = null,
        string createdRange = "all",
        string dueFilter = "all");

    Task<List<TaskWebApiModel?>> GetSortedAssignedTasks(
        TaskStatus? status = TaskStatus.InProgress,
        string? sortBy = "name",
        string? sortOrder = "asc");

    Task<bool> AddTagAsync(int taskId, string tagName);

    Task<bool> RemoveTagAsync(int taskId, string tagName);
}
