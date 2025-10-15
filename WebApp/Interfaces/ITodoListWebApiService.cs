using WebApp.Models.TodoLists;

namespace WebApp.Interfaces;

public interface ITodoListWebApiService
{
    Task<IEnumerable<TodoListWebApiModel>> GetAllAsync();

    Task<TodoListWebApiModel?> GetByIdAsync(int id);

    Task<IEnumerable<TodoListWebApiModel>> GetByUserAsync();

    Task<TodoListCreateModel?> AddAsync(TodoListCreateModel model);

    Task<TodoListUpdateWebApiModel?> UpdateAsync(int id, TodoListUpdateWebApiModel model);

    Task<bool> DeleteAsync(int id);

    Task<bool> RevokeAsync(RevokeModel model);

    Task<bool> ShareAsync(ShareModel model);
}
