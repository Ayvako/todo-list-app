using Core.Entities.TodoUser;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<UserEntity?> RegisterAsync(UserEntity user)
    {
        bool exists = await this.dbContext.Users.AnyAsync(u => u.Email == user.Email);
        if (exists)
        {
            return null;
        }

        _ = await this.dbContext.Users.AddAsync(user);
        _ = await this.dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        return await this.dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UpdateAsync(UserEntity user)
    {
        var existing = await this.dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id) ?? null;
        if (existing == null)
        {
            return false;
        }

        _ = this.dbContext.Users.Update(user);
        _ = await this.dbContext.SaveChangesAsync();
        return true;
    }
}
