using Microsoft.AspNetCore.Mvc;
using WebApp.Interfaces;
using WebApp.Models.Users;
using WebApp.Views.User;

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

        return this.RedirectToAction("Index", "Home");
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

        return this.RedirectToAction("Login", "User");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        this.Response.Cookies.Delete("jwt");
        return this.RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ForgotPassword() => this.View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var response = await this.userService.ForgotPasswordAsync(model);

        if (response)
        {
            this.ViewBag.SuccessMessage = "If an account with that email exists, a reset link has been sent.";
            return this.View();
        }

        this.ModelState.AddModelError(string.Empty, "Failed to send reset link.");
        return this.View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        var model = new ResetPasswordModel { Email = email, Token = token };
        return this.View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var result = await this.userService.ResetPasswordAsync(model);

        if (result)
        {
            this.ViewBag.SuccessMessage = "Your password has been reset successfully. You can now log in.";
            return this.View();
        }

        this.ModelState.AddModelError(string.Empty, "Failed to reset password.");
        return this.View(model);
    }
}
