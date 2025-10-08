using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApp.Services;

namespace WebApp;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Services.AddControllersWithViews()
             .AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
             });

        var baseUrl = builder.Configuration["WebApi:BaseUrl"];

        _ = builder.Services.AddHttpClient<ITodoListWebApiService, TodoListWebApiService>(client =>
            client.BaseAddress = new Uri(baseUrl));

        _ = builder.Services.AddHttpClient<ITaskWebApiService, TaskWebApiService>(client =>
            client.BaseAddress = new Uri(baseUrl));

        _ = builder.Services.AddHttpClient<IUserWebApiService, UserWebApiService>(client =>
            client.BaseAddress = new Uri(baseUrl));
        _ = builder.Services.AddHttpContextAccessor();

        _ = builder.Services.AddSession();

        _ = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/User/Login";
                options.LogoutPath = "/User/Logout";
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });

        _ = builder.Services.AddAuthorization();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            _ = app.UseExceptionHandler("/Home/Error");
            _ = app.UseHsts();
        }

        _ = app.UseHttpsRedirection();
        _ = app.UseStaticFiles();

        _ = app.UseRouting();

        _ = app.UseSession();
        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
