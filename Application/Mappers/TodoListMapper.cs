using Contracts.Tasks;
using Contracts.TodoLists;
using Core.Entities.TodoList;
using Core.Enums;

namespace Application.Mappers;

public static class TodoListMapper
{
    public static TodoListDto ToDto(this TodoListEntity entity, int userId)
    {
        bool canEdit =
            entity.OwnerId == userId ||
            entity.AccessList?.Any(a => a.UserId == userId && a.Role == TodoListAccessRole.Editor) == true;

        bool isShared = entity.AccessList?.Count > 0;

        return new TodoListDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Tasks = entity.Tasks?.Select(t => t.ToDto()).ToList() ?? new List<TaskDto>(),
            CanEdit = canEdit,
            IsShared = isShared,
            AccessList = entity.AccessList != null && entity.AccessList.Count != 0
                ? entity.AccessList.Select(a => new TodoListAccessDto
                {
                    UserName = a.User?.UserName ?? "Unknown",
                    Role = a.Role.ToString()
                }).ToList()
                : new List<TodoListAccessDto>(),
            OwnerName = entity.Owner?.UserName ?? "Unknown"
        };
    }
}
