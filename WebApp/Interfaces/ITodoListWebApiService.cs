using WebApp.Models.TodoLists;

namespace WebApp.Interfaces;

public interface ITodoListWebApiService
{
    Task<IEnumerable<TodoListWebApiModel>> GetAllAsync();

    Task<TodoListWebApiModel?> GetByIdAsync(int id);

    Task<IEnumerable<TodoListWebApiModel>> GetByUserAsync();

    Task<TodoListWebApiModel?> AddAsync(TodoListCreateModel model);

    Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListWebApiModel model);

    Task<bool> DeleteAsync(int id);

    Task<bool> RevokeAsync(RevokeModel model);

    Task<bool> ShareAsync(ShareModel model);
}
