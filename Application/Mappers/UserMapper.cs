using Contracts.Users;
using Core.Entities.TodoUser;

namespace Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this UserEntity entity) => new()
    {
        Id = entity.Id,
        UserName = entity.UserName,
        Email = entity.Email,
        Role = entity.Role,
        TokenVersion = entity.TokenVersion,
    };
}
