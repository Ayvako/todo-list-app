using System.Diagnostics.CodeAnalysis;
using WebApi.Models;
using WebApi.Services.Models;

namespace WebApi.Services;

#pragma warning disable IDE0079 // Remove unnecessary suppression

[SuppressMessage("Maintainability", "CA1515", Justification = "Controllers must be public for Swagger to work")]
public interface ITodoListDatabaseService
{
    Task<IEnumerable<TodoList>> GetAllAsync();

    Task<TodoList?> GetByIdAsync(int id);

    Task<TodoList> AddAsync(TodoListModel model);

    Task<IEnumerable<TaskDto>> GetTasksByListIdAsync(int id);

    Task<TodoList> UpdateAsync(int id, TodoListModel model);

    Task<bool> DeleteAsync(int id);
}
