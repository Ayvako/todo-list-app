using Contracts.Users;
using Core.Entities.TodoUser;

namespace Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this UserEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity), "UserEntity cannot be null.");
        }

        return new UserDto
        {
            Id = entity.Id,
            UserName = entity.UserName,
            Email = entity.Email,
        };
    }
}
