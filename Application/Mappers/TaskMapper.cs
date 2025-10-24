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
        UserDto? assignee = null;
        if (entity.AssigneeId.HasValue)
        {
            assignee = await userService.GetByIdAsync(entity.AssigneeId.Value);
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
            Tags = entity.Tags?.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList() ?? new List<TagDto>(),
        };
    }
}
