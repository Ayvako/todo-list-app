using Core.Entities.TodoUser;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(int id);

    Task<IEnumerable<UserEntity>> GetAllAsync();

    Task<UserEntity?> GetUserByNameAsync(string? username);

    Task<UserEntity?> RegisterAsync(string username, string email, string password);

    Task<UserEntity?> LoginAsync(string email, string password);
}
