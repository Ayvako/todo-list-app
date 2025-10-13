using Contracts.Tasks;
using Contracts.TodoLists;
using Core.Enums;

namespace Application.Interfaces;

public interface ITodoListService
{
    Task<IEnumerable<TodoListDto>> GetAllAsync(int currentUserId);

    Task<TodoListDto?> GetByIdAsync(int listId, int currentUserId);

    Task<IEnumerable<TodoListDto>> GetByUserAsync(int userId);

    Task<IEnumerable<TaskDto>> GetTasksByListIdAsync(int listId, int currentUserId);

    Task<TodoListDto> AddAsync(TodoListCreateDto model, int userId);

    Task<TodoListDto?> UpdateAsync(int listId, TodoListUpdateDto model, int userId);

    Task<bool> DeleteAsync(int listId, int userId);

    Task<bool> CanEditAsync(int todoListId, int userId);

    Task<bool> CanViewAsync(int todoListId, int userId);

    Task<bool> ShareAsync(int listId, int targetUserId, TodoListAccessRole role, int currentUserId);

    Task<bool> RevokeAccessAsync(int listId, int targetUserId, int currentUserId);
}
