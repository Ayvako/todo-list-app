using Contracts.TodoLists;
using Core.Enums;
using WebApp.Models.Tasks;
using WebApp.Models.TodoLists;

namespace Application.Interfaces;

public interface ITodoListService
{
    Task<IEnumerable<TodoListWebApiModel>> GetAllAsync(int currentUserId);

    Task<TodoListWebApiModel?> GetByIdAsync(int listId, int currentUserId);

    Task<IEnumerable<TodoListWebApiModel>> GetByUserAsync(int userId);

    Task<IEnumerable<TaskWebApiModel>> GetTasksByListIdAsync(int listId, int currentUserId);

    Task<TodoListWebApiModel> AddAsync(TodoListCreateDto model, int userId);

    Task<TodoListWebApiModel?> UpdateAsync(int listId, TodoListUpdateDto model, int userId);

    Task<bool> DeleteAsync(int listId, int userId);

    Task<bool> CanEditAsync(int todoListId, int userId);

    Task<bool> CanViewAsync(int todoListId, int userId);

    Task<bool> ShareAsync(int listId, int targetUserId, TodoListAccessRole role, int currentUserId);

    Task<bool> RevokeAccessAsync(int listId, int targetUserId, int currentUserId);
}
