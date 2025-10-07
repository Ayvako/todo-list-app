using Application.Services.Interfaces;
using Contracts.Users;
using Core.Entities.TodoUser;
using Core.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly TodoListDbContext dbContext;

    public UserService(TodoListDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await this.dbContext.Users.FindAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await this.dbContext.Users
            .Select(u => MapToDto(u))
            .ToListAsync();
    }

    public async Task<UserDto?> RegisterAsync(string username, string email, string password)
    {
        if (await this.dbContext.Users.AnyAsync(u => u.Email == email))
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var entity = new UserEntity
        {
            UserName = username,
            Email = email,
            PasswordHash = HashPassword(password),
            Role = UserRole.Authorized
        };

        _ = this.dbContext.Users.Add(entity);
        _ = await this.dbContext.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<UserDto?> LoginAsync(string email, string password)
    {
        var hash = HashPassword(password);

        var user = await this.dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hash);

        return user == null ? null : MapToDto(user);
    }

    public async Task<bool> HasAccessToListAsync(int userId, int listId, bool requireEditRights = false)
    {
        var access = await this.dbContext.TodoListAccesses
            .FirstOrDefaultAsync(a => a.UserId == userId && a.TodoListId == listId);

        if (access == null)
        {
            return false;
        }

        if (requireEditRights)
        {
            return access.Role is TodoListAccessRole.Editor or TodoListAccessRole.Owner;
        }

        return true;
    }

    private static UserDto MapToDto(UserEntity entity) => new UserDto
    {
        Id = entity.Id,
        UserName = entity.UserName,
        Email = entity.Email,
        Role = entity.Role
    };

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
