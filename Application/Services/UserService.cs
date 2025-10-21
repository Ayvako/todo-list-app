using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Contracts.Users;
using Core.Entities.TodoUser;
using Core.Enums;
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
        var hashed = HashPassword(password);
        var user = new UserEntity
        {
            UserName = username,
            Email = email,
            PasswordHash = hashed,
            Role = UserRole.Authorized
        };

        var createdUser = await this.userRepository.RegisterAsync(user);
        return createdUser == null ? null : MapToDto(createdUser);
    }

    public async Task<UserDto?> LoginAsync(string email, string password)
    {
        var hashed = HashPassword(password);
        var user = await this.userRepository.GetByEmailAsync(email);

        if (user == null || user.PasswordHash != hashed)
        {
            return null;
        }

        return MapToDto(user);
    }

    public async Task SendPasswordResetAsync(string email)
    {
        var user = await this.userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return;
        }

        user.ResetToken = Guid.NewGuid().ToString("N");
        user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(30);

        _ = await this.userRepository.UpdateAsync(user);

        // TODO: тут интегрировать EmailService
        // await _emailService.SendAsync(user.Email, "Password reset",
        //    $"Click here to reset: https://yourapp.com/reset-password?token={user.ResetToken}&email={user.Email}");
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await this.userRepository.GetByEmailAsync(email);
        if (user == null || user.ResetToken != token || user.ResetTokenExpires < DateTime.UtcNow)
        {
            return false;
        }

        user.PasswordHash = HashPassword(newPassword);
        user.ResetToken = null;
        user.ResetTokenExpires = null;
        user.TokenVersion++;

        _ = await this.userRepository.UpdateAsync(user);
        return true;
    }

    private static UserDto MapToDto(UserEntity entity) => new()
    {
        Id = entity.Id,
        UserName = entity.UserName,
        Email = entity.Email,
        Role = entity.Role,
        TokenVersion = entity.TokenVersion,
    };

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
