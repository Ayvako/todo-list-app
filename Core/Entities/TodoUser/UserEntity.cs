using System.ComponentModel.DataAnnotations;
using Core.Entities.Task;
using Core.Entities.TodoList;
using Core.Enums;

namespace Core.Entities.TodoUser;

public class UserEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Authorized;

    public ICollection<TodoListEntity> OwnedLists { get; set; } = new List<TodoListEntity>();
    public ICollection<TaskEntity> AssignedTasks { get; set; } = new List<TaskEntity>();
}
