using WebApi.Models;

namespace WebApi.Services;

public interface ITaskService
{
    Task<bool> DeleteTaskAsync(int id);

    Task<TaskModel> AddTaskAsync(int todoListId, TaskCreateModel model);

    Task<TaskModel?> GetTaskByIdAsync(int id);

    Task<TaskModel?> UpdateTaskAsync(int id, TaskEditModel model);
}
