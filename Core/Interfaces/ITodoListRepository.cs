using Core.Entities.Task;
using Core.Entities.TodoList;
using Core.Enums;

namespace Core.Interfaces;

public interface ITodoListRepository
{
    Task<IEnumerable<TodoListEntity>> GetAllAsync();

    Task<TodoListEntity?> GetByIdAsync(int id);

    Task<List<TodoListEntity>> GetByUserIdAsync(int userId);

    Task<TodoListEntity> AddAsync(TodoListEntity todoList);

    Task<IEnumerable<TaskEntity>> GetTasksByListIdAsync(int todoListId);

    Task<TodoListEntity> UpdateAsync(int id, TodoListEntity todoList);

    Task<bool> CanViewAsync(int listId, int userId);

    Task<bool> CanEditAsync(int listId, int userId);

    Task<bool> DeleteAsync(int id);

    Task<bool> AddAccessAsync(int listId, int userId, TodoListAccessRole role);

    Task<bool> RemoveAccessAsync(int listId, int userId);

    Task<bool> UpdateAccessRoleAsync(int listId, int userId, TodoListAccessRole newRole);
}
