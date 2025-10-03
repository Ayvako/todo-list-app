using WebApp.Models.Tasks;

namespace WebApp.Services;

public interface ITaskWebApiService
{
    Task<TaskWebApiModel?> GetTaskByIdAsync(int id);

    Task<TaskWebApiModel?> AddTaskAsync(int todoListId, TaskCreateModel model);

    Task<bool> DeleteTaskAsync(int id);

    Task<TaskWebApiModel?> UpdateTaskAsync(int id, TaskEditModel model);
}
