using Core.Entities.Task;
using Core.Entities.TodoList;
using Core.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
    : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists { get; set; }

    public DbSet<TaskEntity> Tasks { get; set; }

    public DbSet<TaskCommentEntity> Comments { get; set; }

    public DbSet<UserEntity> Users { get; set; }
}
