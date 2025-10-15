using Core.Entities.TodoUser;
using Core.Enums;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TodoListDbContext dbContext;

    public UserRepository(TodoListDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<UserEntity?> GetByIdAsync(int id)
    {
        return await this.dbContext.Users
            .Include(u => u.Comments)
            .Include(u => u.OwnedLists)
            .Include(u => u.AssignedTasks)
            .Include(u => u.AccessList)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        return await this.dbContext.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<UserEntity?> GetUserByNameAsync(string? username)
    {
        return await this.dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<UserEntity?> RegisterAsync(string username, string email, string password)
    {
        bool exists = await this.dbContext.Users.AnyAsync(u => u.Email == email);
        if (exists)
        {
            return null;
        }

        string hashed = HashPassword(password);

        var user = new UserEntity
        {
            UserName = username,
            Email = email,
            PasswordHash = hashed,
            Role = UserRole.Authorized
        };

        _ = await this.dbContext.Users.AddAsync(user);
        _ = await this.dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<UserEntity?> LoginAsync(string email, string password)
    {
        var hashed = HashPassword(password);

        var user = await this.dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hashed);

        if (user == null)
        {
            return null;
        }

        return new UserEntity
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = UserRole.Authorized
        };
    }

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
