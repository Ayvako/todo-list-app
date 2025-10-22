using Contracts.Tasks;
using Contracts.Users;
using Core.Entities.Task;

namespace Application.Mappers;

public static class TaskMapper
{
    public static TaskDto ToDto(this TaskEntity entity)
        => new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            DueDate = entity.DueDate,
            Status = entity.Status,
            TodoListId = entity.TodoListId,
            Assignee = entity.Assignee == null
                ? null
                : new UserDto
                {
                    Id = entity.Assignee.Id,
                    Email = entity.Assignee.Email,
                    Role = entity.Assignee.Role,
                    UserName = entity.Assignee.UserName
                },
            Tags = entity.Tags?.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList() ?? new List<TagDto>(),
        };
}
