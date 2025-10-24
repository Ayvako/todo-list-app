using Core.Entities.TodoUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class UserDbContext : IdentityDbContext<UserEntity, IdentityRole<int>, int>
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }
}
