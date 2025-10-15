using Application.Interfaces;
using Contracts.Users;
using Core.Entities.TodoUser;
using Core.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await this.userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var entities = await this.userRepository.GetAllAsync();
        return entities.Select(u => MapToDto(u)).ToList() ?? new List<UserDto>();
    }

    public async Task<UserDto?> GetUserByNameAsync(string username)
    {
        var user = await this.userRepository.GetUserByNameAsync(username);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> RegisterAsync(string username, string email, string password)
    {
        var user = await this.userRepository.RegisterAsync(username, email, password);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> LoginAsync(string email, string password)
    {
        var user = await this.userRepository.LoginAsync(email, password);
        return user == null ? null : MapToDto(user);
    }

    private static UserDto MapToDto(UserEntity entity) => new()
    {
        Id = entity.Id,
        UserName = entity.UserName,
        Email = entity.Email,
        Role = entity.Role
    };
}
