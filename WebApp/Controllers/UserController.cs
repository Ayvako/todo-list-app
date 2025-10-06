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
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await userService.LoginAsync(model);
        if (response == null)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }

        Response.Cookies.Append("jwt", response.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        HttpContext.Session.SetInt32("UserId", response.User.Id);
        HttpContext.Session.SetString("UserName", response.User.UserName);
        HttpContext.Session.SetString("UserRole", response.User.Role);

        return RedirectToAction("Index", "TodoList");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(UserRegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await userService.RegisterAsync(model);

            Response.Cookies.Append("jwt", response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            HttpContext.Session.SetInt32("UserId", response.User.Id);
            HttpContext.Session.SetString("UserName", response.User.UserName);
            HttpContext.Session.SetString("UserRole", response.User.Role);

            return RedirectToAction("Index", "TodoList");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        Response.Cookies.Delete("jwt");
        return RedirectToAction("Login");
    }
}
