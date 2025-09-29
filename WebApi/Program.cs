using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Services;

namespace WebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("TodoListDb");

        _ = builder.Services.AddDbContext<TodoListDbContext>(options => options.UseSqlServer(connectionString));

        _ = builder.Services.AddScoped<ITodoListDatabaseService, TodoListDatabaseService>();

        _ = builder.Services.AddControllers();
        _ = builder.Services.AddEndpointsApiExplorer();
        _ = builder.Services.AddSwaggerGen();

        var app = builder.Build();
        Console.WriteLine("ConnectionString: " + builder.Configuration.GetConnectionString("TodoListDb"));

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
