using Core.Entities.Task;
using Core.Entities.TodoList;

namespace Core.Interfaces;

public interface ITodoListRepository
{
    Task<IEnumerable<TodoListEntity>> GetAllAsync();

    Task<TodoListEntity?> GetByIdAsync(int id);

    Task<TodoListEntity> AddAsync(TodoListEntity todoList);

    Task<IEnumerable<TaskEntity>> GetTasksByListIdAsync(int todoListId);

    Task<TodoListEntity> UpdateAsync(int id, TodoListEntity todoList);

    Task<bool> DeleteAsync(int id);
}
