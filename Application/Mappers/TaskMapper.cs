using Application.Interfaces;
using Contracts.Tags;
using Contracts.Tasks;
using Contracts.Users;
using Core.Entities.Task;

namespace Application.Mappers;

public static class TaskMapper
{
    public static async Task<TaskDto> ToDtoAsync(this TaskEntity entity, IUserService userService)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(userService);

        UserDto? assignee = null;
        if (entity.AssigneeId.HasValue)
        {
            assignee = await userService.GetByIdAsync(entity.AssigneeId.Value);
        }
        var сomments = new List<TaskCommentDto>();

        foreach (var a in entity.Comments)
        {
            var user = await userService.GetByIdAsync(a.UserId);

            сomments.Add(new TaskCommentDto
            {
                Id = a.Id,
                CreatedAt = a.CreatedAt,
                TaskId = a.TaskId,
                Text = a.Text,
                AuthorName = user!.UserName,
                AuthorId = a.UserId,
                UpdatedAt = a.UpdatedAt
            });
        }

        return new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            DueDate = entity.DueDate,
            Status = entity.Status,
            TodoListId = entity.TodoListId,
            Assignee = assignee,
            Comments = сomments,
            Tags = entity.Tags?.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList() ?? new List<TagDto>(),
        };
    }
}
