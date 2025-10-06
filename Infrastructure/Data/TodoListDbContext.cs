using Core.Entities;
using Core.Entities.Task;
using Core.Entities.TodoList;
using Core.Entities.TodoUser;
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

    public DbSet<TodoListAccessEntity> TodoListAccesses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        _ = modelBuilder.Entity<TodoListAccessEntity>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<TodoListAccessEntity>()
            .HasOne(a => a.TodoList)
            .WithMany(l => l.AccessList)
            .HasForeignKey(a => a.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
