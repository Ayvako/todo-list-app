using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class UserController : Controller
{
    private readonly IUserWebApiService userService;

    public UserController(IUserWebApiService userService)
    {
        this.userService = userService;
    }

    [HttpGet]
    public IActionResult Login() => this.View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var response = await this.userService.LoginAsync(model);
        if (response == null)
        {
            this.ModelState.AddModelError(string.Empty, "Invalid credentials");
            return this.View(model);
        }

        this.Response.Cookies.Append("jwt", response.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1),
        });

        this.HttpContext.Session.SetString("AuthToken", response.Token);
        this.HttpContext.Session.SetInt32("UserId", response.User.Id);
        this.HttpContext.Session.SetString("UserName", response.User.UserName);
        this.HttpContext.Session.SetString("UserRole", response.User.Role);

        return this.RedirectToAction("Index", "TodoList");
    }

    [HttpGet]
    public IActionResult Register() => this.View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var response = await this.userService.RegisterAsync(model);

        if (response == null)
        {
            this.ModelState.AddModelError(string.Empty, "Registration error. Check your details or use a different email.");
            return this.View(model);
        }

        this.Response.Cookies.Append("jwt", response.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1),
        });

        this.HttpContext.Session.SetInt32("UserId", response.User.Id);
        this.HttpContext.Session.SetString("UserName", response.User.UserName);
        this.HttpContext.Session.SetString("UserRole", response.User.Role);
        this.HttpContext.Session.SetString("AuthToken", response.Token);

        return this.RedirectToAction("Index", "TodoList");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        this.HttpContext.Session.Clear();
        this.Response.Cookies.Delete("jwt");
        return this.RedirectToAction("Login");
    }
}
