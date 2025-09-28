using Microsoft.EntityFrameworkCore;

namespace WebApi.Models;

internal class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
    : base(options)
    {
    }
}
