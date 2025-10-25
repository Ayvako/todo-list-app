using Contracts.Users;

namespace Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int id);

    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<UserDto?> RegisterAsync(string username, string email, string password);

    Task<UserDto?> LoginAsync(string email, string password);

    Task<UserDto?> GetUserByNameAsync(string username);

    Task<Dictionary<int, UserDto>> GetByIdsAsync(IEnumerable<int> userIds);

    Task SendPasswordResetAsync(string email);

    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
}
