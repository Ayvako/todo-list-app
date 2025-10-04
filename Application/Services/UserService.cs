using Contracts.Users;
using Core.Entities.TodoUser;
using Core.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
        var user = await dbContext.Users.FindAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await dbContext.Users
            .Select(u => MapToDto(u))
            .ToListAsync();
    }

    public async Task<UserDto?> RegisterAsync(string username, string email, string password)
    {
        if (await dbContext.Users.AnyAsync(u => u.Email == email))
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var entity = new UserEntity
        {
            UserName = username,
            Email = email,
            PasswordHash = password, // ⚠️ позже заменить на хеширование!
            Role = UserRole.Authorized
        };

        _ = dbContext.Users.Add(entity);
        _ = await dbContext.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<UserDto?> LoginAsync(string email, string password)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);

        return user == null ? null : MapToDto(user);
    }

    public async Task<bool> HasAccessToListAsync(int userId, int listId, bool requireEditRights = false)
    {
        var access = await dbContext.TodoListAccesses
            .FirstOrDefaultAsync(a => a.UserId == userId && a.TodoListId == listId);

        if (access == null)
        {
            return false;
        }

        if (requireEditRights)
        {
            return access.AccessLevel is UserRole.Editor or UserRole.Owner;
        }

        return true;
    }

    private static UserDto MapToDto(UserEntity entity) => new()
    {
        Id = entity.Id,
        UserName = entity.UserName,
        Email = entity.Email,
        Role = entity.Role
    };
}
