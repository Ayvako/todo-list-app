using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.Services;
using Core.Entities.TodoUser;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Middlewares;

namespace WebApi;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("TodoListDb");
        _ = builder.Services.AddDbContext<TodoListDbContext>(options =>
            options.UseSqlServer(connectionString));
        _ = builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("UserDb")));

        _ = builder.Services.AddScoped<ITodoListRepository, TodoListRepository>();
        _ = builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        _ = builder.Services.AddScoped<ITagRepository, TagRepository>();

        _ = builder.Services.AddScoped<ITodoListService, TodoListService>();
        _ = builder.Services.AddScoped<ITaskService, TaskService>();
        _ = builder.Services.AddScoped<IUserService, UserService>();
        _ = builder.Services.AddScoped<ITagService, TagService>();

        _ = builder.Services.AddScoped<IJwtService, JwtService>();

        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("Jwt:Key not found in config."));

        _ = builder.Services.AddIdentity<UserEntity, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 0;
        })
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();
        _ = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("jwt"))
                    {
                        context.Token = context.Request.Cookies["jwt"];
                    }

                    return Task.CompletedTask;
                },
            };
        });

        _ = builder.Services.AddAuthorization();

        _ = builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        _ = builder.Services.AddControllers();
        _ = builder.Services.AddEndpointsApiExplorer();
        _ = builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoList API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите ваш токен",
            });

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    },
                });
        });
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();
        }

        _ = app.UseHttpsRedirection();

        _ = app.UseMiddleware<ExceptionHandlingMiddleware>();

        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.MapControllers();

        app.Run();
    }
}
