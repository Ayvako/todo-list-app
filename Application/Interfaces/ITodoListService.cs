using WebApp.Models.Tasks;
using WebApp.Models.TodoLists;
using Contracts.TodoLists;

namespace Application.Interfaces;

public interface ITodoListService
{
    Task<IEnumerable<TodoListWebApiModel>> GetAllAsync(int currentUserId);

    Task<TodoListWebApiModel?> GetByIdAsync(int id, int currentUserId);

    Task<IEnumerable<TodoListWebApiModel>> GetByUserAsync(int userId);

    Task<IEnumerable<TaskWebApiModel>> GetTasksByListIdAsync(int id);

    Task<TodoListWebApiModel> AddAsync(TodoListCreateDto model, int userId);

    Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListUpdateDto model, int userId);

    Task<bool> DeleteAsync(int id, int userId);

    Task<bool> CanEditAsync(int todoListId, int userId);

    Task<bool> CanViewAsync(int todoListId, int userId);
}
