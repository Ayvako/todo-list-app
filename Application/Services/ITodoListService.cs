using Contracts.TodoLists;
using WebApp.Models.Tasks;
using WebApp.Models.TodoLists;

namespace Application.Services;

public interface ITodoListService
{
    Task<IEnumerable<TodoListWebApiModel>> GetAllAsync();

    Task<TodoListWebApiModel?> GetByIdAsync(int id);

    Task<IEnumerable<TaskWebApiModel>> GetTasksByListIdAsync(int id);

    Task<TodoListWebApiModel> AddAsync(TodoListCreateDto model);

    Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListUpdateDto model);

    Task<bool> DeleteAsync(int id);

    Task<bool> CanEditAsync(int todoListId, int userId);

    Task<bool> CanViewAsync(int todoListId, int userId);
}
