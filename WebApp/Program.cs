using WebApp.Services;

namespace WebApp;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Services.AddControllersWithViews();

        _ = builder.Services.AddHttpClient<ITodoListWebApiService, TodoListWebApiService>(client =>
        {
            var baseUrl = builder.Configuration["WebApi:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl);
        });

        _ = builder.Services.AddHttpClient<ITaskWebApiService, TaskWebApiService>(client =>
        {
            var baseUrl = builder.Configuration["WebApi:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl);
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            _ = app.UseExceptionHandler("/Home/Error");
            _ = app.UseHsts();
        }

        _ = app.UseHttpsRedirection();
        _ = app.UseStaticFiles();

        _ = app.UseRouting();

        _ = app.UseAuthorization();

        _ = app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
