using WebApp.Models.Tasks;

namespace WebApp.Interfaces;

public interface ITaskWebApiService
{
    Task<TaskWebApiModel?> GetTaskByIdAsync(int id);

    Task<TaskWebApiModel?> AddTaskAsync(int todoListId, TaskCreateModel model);

    Task<bool> DeleteTaskAsync(int id);

    Task<TaskWebApiModel?> UpdateTaskAsync(int id, TaskEditModel model);

    Task<IEnumerable<TaskWebApiModel?>> GetAssignedTasksAsync();

    Task<bool> ChangeStatusAsync(int id, ChangeStatusModel model);
}
