using Application.Services;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("TodoListDb");

        _ = builder.Services.AddDbContext<TodoListDbContext>(options => options.UseSqlServer(connectionString));

        _ = builder.Services.AddScoped<ITodoListRepository, TodoListRepository>();
        _ = builder.Services.AddScoped<ITaskRepository, TaskRepository>();

        _ = builder.Services.AddScoped<ITodoListService, TodoListService>();
        _ = builder.Services.AddScoped<ITaskService, TaskService>();
        _ = builder.Services.AddScoped<IUserService, UserService>();

        _ = builder.Services.AddControllers();

        _ = builder.Services.AddEndpointsApiExplorer();
        _ = builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
        }

        _ = app.UseHttpsRedirection();

        _ = app.UseAuthorization();

        _ = app.MapControllers();

        app.Run();
    }
}
