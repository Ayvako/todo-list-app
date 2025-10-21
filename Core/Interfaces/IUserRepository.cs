using Core.Entities.TodoUser;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(int id);

    Task<IEnumerable<UserEntity>> GetAllAsync();

    Task<UserEntity?> GetUserByNameAsync(string? username);

    Task<UserEntity?> RegisterAsync(UserEntity user);

    Task<UserEntity?> GetByEmailAsync(string email);

    Task<bool> UpdateAsync(UserEntity user);
}
