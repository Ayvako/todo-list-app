using Application.Interfaces;
using Contracts.Tasks;
using Contracts.TodoLists;
using Core.Entities.TodoList;
using Core.Enums;

namespace Application.Mappers;

public static class TodoListMapper
{
    public static async Task<TodoListDto> ToDtoAsync(this TodoListEntity entity, int userId, IUserService userService)
    {
        ArgumentNullException.ThrowIfNull(userService);
        ArgumentNullException.ThrowIfNull(entity);

        bool canEdit =
            entity.OwnerId == userId ||
            entity.AccessList?.Any(a => a.UserId == userId && a.Role == TodoListAccessRole.Editor) == true;

        bool isShared = entity.AccessList?.Count > 0;

        string ownerName = "Unknown";

        if (entity.OwnerId != 0)
        {
            var owner = await userService.GetByIdAsync(entity.OwnerId);
            ownerName = owner?.UserName ?? "Unknown";
        }

        var accessList = new List<TodoListAccessDto>();
        if (entity.AccessList != null && entity.AccessList.Count > 0)
        {
            foreach (var a in entity.AccessList)
            {
                var user = await userService.GetByIdAsync(a.UserId);
                accessList.Add(new TodoListAccessDto
                {
                    UserName = user?.UserName ?? "Unknown",
                    Role = a.Role.ToString()
                });
            }
        }

        var tasks = new List<TaskDto>();
        if (entity.Tasks != null)
        {
            foreach (var t in entity.Tasks)
            {
                var taskDto = await t.ToDtoAsync(userService);
                tasks.Add(taskDto);
            }
        }

        return new TodoListDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Tasks = tasks,
            CanEdit = canEdit,
            IsShared = isShared,
            AccessList = accessList,
            OwnerName = ownerName,
        };
    }
}
