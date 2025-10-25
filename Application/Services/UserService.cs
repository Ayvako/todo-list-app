using Application.Interfaces;
using Application.Mappers;
using Contracts.Users;
using Core.Entities.TodoUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<UserEntity> userManager;
    private readonly SignInManager<UserEntity> signInManager;

    public UserService(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await this.userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return user == null ? null : user.ToDto();
    }

    public async Task<Dictionary<int, UserDto>> GetByIdsAsync(IEnumerable<int> userIds)
    {
        var users = await this.userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            })
            .ToDictionaryAsync(u => u.Id);

        return users;
    }

    public async Task<UserDto?> GetUserByNameAsync(string username)
    {
        var user = await this.userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        return user?.ToDto();
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await this.userManager.Users.Select(u => u.ToDto()).ToListAsync();
    }

    public async Task<UserDto?> RegisterAsync(string username, string email, string password)
    {
        var user = new UserEntity { UserName = username, Email = email };
        var result = await this.userManager.CreateAsync(user, password);
        return result.Succeeded ? user.ToDto() : null;
    }

    public async Task<UserDto?> LoginAsync(string email, string password)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        var result = await this.signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        return result.Succeeded ? user.ToDto() : null;
    }

    public async Task SendPasswordResetAsync(string email)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return;
        }

        _ = await this.userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: интегрировать EmailService
        // await _emailService.SendAsync(user.Email, "Password reset",
        //    $"Click here to reset: https://yourapp.com/reset-password?token={token}&email={user.Email}");
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        var result = await this.userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }
}
