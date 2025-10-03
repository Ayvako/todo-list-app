using WebApp.Models.Tasks;

namespace Application.Services;

public interface ITaskService
{
    Task<TaskWebApiModel> AddTaskAsync(int todoListId, TaskCreateModel model);

    Task<TaskWebApiModel?> GetTaskByIdAsync(int id);

    Task<TaskWebApiModel?> UpdateTaskAsync(int id, TaskEditModel model);

    Task<bool> DeleteTaskAsync(int id);
}
