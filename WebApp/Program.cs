using WebApp.Services;

namespace WebApp;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Services.AddControllersWithViews();

        var baseUrl = builder.Configuration["WebApi:BaseUrl"];

        _ = builder.Services.AddHttpClient<ITodoListWebApiService, TodoListWebApiService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        _ = builder.Services.AddHttpClient<ITaskWebApiService, TaskWebApiService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        _ = builder.Services.AddHttpClient<IUserWebApiService, UserWebApiService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        _ = builder.Services.AddSession();

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
        _ = app.UseAuthorization();

        _ = app.MapControllerRoute(
            name: "default",
            pattern: "{controller=TodoList}/{action=Index}/{id?}");

        app.Run();
    }
}
