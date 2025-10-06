using Contracts.Users;

namespace Application.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(UserDto user);
}
