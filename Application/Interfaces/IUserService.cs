using Contracts.Users;

namespace Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int id);

    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<UserDto?> RegisterAsync(string username, string email, string password);

    Task<UserDto?> LoginAsync(string email, string password);

    Task<UserDto?> GetUserByNameAsync(string username);
}
