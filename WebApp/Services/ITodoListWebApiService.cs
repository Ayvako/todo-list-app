using WebApp.Models;

namespace WebApp.Services;

public interface ITodoListWebApiService
{
    Task<IEnumerable<TodoListWebApiModel>> GetAllAsync();

    Task<TodoListWebApiModel?> GetByIdAsync(int id);

    Task<TodoListWebApiModel?> AddAsync(TodoListWebApiModel model);

    Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListWebApiModel model);

    Task<bool> DeleteAsync(int id);
}
