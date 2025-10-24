using Core.Entities.Comment;
using Core.Entities.Tag;
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

    public DbSet<CommentEntity> Comments { get; set; }

    public DbSet<TagEntity> Tags { get; set; }

    public DbSet<TodoListAccessEntity> TodoListAccesses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        _ = modelBuilder.Entity<TodoListAccessEntity>()
            .HasKey(t => new { t.UserId, t.TodoListId });

        _ = modelBuilder.Entity<TaskEntity>()
            .HasOne(t => t.TodoList)
            .WithMany(l => l.Tasks)
            .HasForeignKey(t => t.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<TodoListAccessEntity>()
            .HasOne(a => a.TodoList)
            .WithMany(l => l.AccessList)
            .HasForeignKey(a => a.TodoListId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<TaskEntity>()
            .HasMany(t => t.Tags)
            .WithMany(tg => tg.Tasks)
            .UsingEntity(j => j.ToTable("TaskTags"));

        _ = modelBuilder.Entity<CommentEntity>()
            .HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
