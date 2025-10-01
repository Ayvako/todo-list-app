using WebApi.Models;
using WebApi.Services.Models;

namespace WebApi.Services;

public interface ITodoListDatabaseService
{
    Task<IEnumerable<TodoList>> GetAllAsync();

    Task<TodoList?> GetByIdAsync(int id);

    Task<TodoList> AddAsync(TodoListModel model);

    Task<IEnumerable<TaskDto>> GetTasksByListIdAsync(int id);

    Task<TodoList> UpdateAsync(int id, TodoListModel model);

    Task<bool> DeleteAsync(int id);
}
