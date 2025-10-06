using WebApp.Models.Tasks;
using WebApp.Models.TodoLists;
using Contracts.TodoLists;

namespace Application.Services.Interfaces;

public interface ITodoListService
{
    Task<IEnumerable<TodoListWebApiModel>> GetAllAsync(int currentUserId);

    Task<TodoListWebApiModel?> GetByIdAsync(int id, int currentUserId);

    Task<IEnumerable<TaskWebApiModel>> GetTasksByListIdAsync(int id);

    Task<TodoListWebApiModel> AddAsync(TodoListCreateDto model, int userId);

    Task<TodoListWebApiModel?> UpdateAsync(int id, TodoListUpdateDto model, int userId);

    Task<bool> DeleteAsync(int id, int userId);

    Task<bool> CanEditAsync(int todoListId, int userId);

    Task<bool> CanViewAsync(int todoListId, int userId);
}
