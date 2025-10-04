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
    public IActionResult Register() => this.View();

    [HttpPost]
    public async Task<IActionResult> Register(UserRegisterModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var result = await this.userService.RegisterAsync(model);
        if (result == null)
        {
            this.ModelState.AddModelError(string.Empty, "Registration failed.");
            return this.View(model);
        }

        return this.RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login() => this.View();

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var user = await this.userService.LoginAsync(model);
        if (user == null)
        {
            this.ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return this.View(model);
        }

        this.HttpContext.Session.SetInt32("UserId", user.Id);
        this.HttpContext.Session.SetString("UserName", user.UserName);
        this.HttpContext.Session.SetString("UserRole", user.Role.ToString());

        return this.RedirectToAction("Index", "TodoList");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        this.HttpContext.Session.Clear();
        return this.RedirectToAction("Login");
    }
}
