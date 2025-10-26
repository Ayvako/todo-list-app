using Core.Entities.Comment;
using Core.Entities.Tag;
using Core.Entities.Task;
using Core.Entities.TodoList;
using Core.Entities.TodoUser;
using Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Core.Enums.TaskStatus;

namespace Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(DbContext context, UserManager<UserEntity> userManager)
    {
        await context.Database.MigrateAsync();

        if (!await userManager.Users.AnyAsync())
        {
            var users = new[]
            {
                new UserEntity { UserName = "alice", Email = "alice@example.com" },
                new UserEntity { UserName = "bob", Email = "bob@example.com" },
                new UserEntity { UserName = "charlie", Email = "charlie@example.com" },
            };

            foreach (var user in users)
            {
                _ = await userManager.CreateAsync(user, "Test123!");
            }

            _ = await context.SaveChangesAsync();
        }

        var allUsers = await userManager.Users.ToListAsync();

        if (!await context.Set<TagEntity>().AnyAsync())
        {
            var tags = new[]
            {
                new TagEntity { Name = "Urgent" },
                new TagEntity { Name = "Work" },
                new TagEntity { Name = "Personal" }
            };
            context.AddRange(tags);
            _ = await context.SaveChangesAsync();
        }

        var allTags = await context.Set<TagEntity>().ToListAsync();

        if (!await context.Set<TodoListEntity>().AnyAsync())
        {
            var list1 = new TodoListEntity
            {
                Title = "Project Alpha",
                Description = "Main company project",
                OwnerId = allUsers[0].Id,
            };

            var list2 = new TodoListEntity
            {
                Title = "Personal Tasks",
                Description = "Alice’s personal stuff",
                OwnerId = allUsers[0].Id,
            };

            context.AddRange(list1, list2);
            _ = await context.SaveChangesAsync();

            var access = new[]
            {
                new TodoListAccessEntity { TodoListId = list1.Id, UserId = allUsers[1].Id, Role = TodoListAccessRole.Editor },
                new TodoListAccessEntity { TodoListId = list1.Id, UserId = allUsers[2].Id, Role = TodoListAccessRole.Viewer },
            };
            context.AddRange(access);
            _ = await context.SaveChangesAsync();

            var tasks = new[]
            {
                new TaskEntity
                {
                    Title = "Setup Backend",
                    Description = "Configure EF Core and services",
                    TodoListId = list1.Id,
                    AssigneeId = allUsers[1].Id,
                    DueDate = DateTime.UtcNow.AddDays(3),
                    Status = TaskStatus.InProgress,
                    Tags = new List<TagEntity> { allTags.First(t => t.Name == "Work"), allTags.First(t => t.Name == "Urgent") }
                },
                new TaskEntity
                {
                    Title = "Frontend UI",
                    Description = "Create main dashboard",
                    TodoListId = list1.Id,
                    AssigneeId = allUsers[2].Id,
                    DueDate = DateTime.UtcNow.AddDays(5),
                    Status = TaskStatus.NotStarted,
                    Tags = new List<TagEntity> { allTags.First(t => t.Name == "Work") }
                },
                new TaskEntity
                {
                    Title = "Buy groceries",
                    Description = "Milk, bread, fruits",
                    TodoListId = list2.Id,
                    AssigneeId = allUsers[0].Id,
                    DueDate = DateTime.UtcNow.AddDays(1),
                    Status = TaskStatus.NotStarted,
                    Tags = new List<TagEntity> { allTags.First(t => t.Name == "Personal") }
                }
            };
            context.AddRange(tasks);
            _ = await context.SaveChangesAsync();

            var comments = new[]
            {
                new CommentEntity { TaskId = tasks[0].Id, UserId = allUsers[0].Id, Text = "Don’t forget connection string setup" },
                new CommentEntity { TaskId = tasks[1].Id, UserId = allUsers[1].Id, Text = "Design mockups are ready" }
            };
            context.AddRange(comments);
            _ = await context.SaveChangesAsync();
        }
    }
}
