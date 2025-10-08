using Contracts.Users;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(UserDto user);
}
