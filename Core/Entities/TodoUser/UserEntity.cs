using System.ComponentModel.DataAnnotations;
using Core.Entities.Comment;
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
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpires { get; set; }

    public int TokenVersion { get; set; }

    public UserRole Role { get; set; } = UserRole.Unauthorized;

    public ICollection<CommentEntity> Comments { get; set; } = new HashSet<CommentEntity>();

    public ICollection<TodoListEntity> OwnedLists { get; set; } = new HashSet<TodoListEntity>();

    public ICollection<TaskEntity> AssignedTasks { get; set; } = new HashSet<TaskEntity>();

    public ICollection<TodoListAccessEntity> AccessList { get; set; } = new HashSet<TodoListAccessEntity>();
}
