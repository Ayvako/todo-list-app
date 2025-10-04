using Application.Services;
using Contracts.Users;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;

    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] UserRegisterDto model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var user = await this.userService.RegisterAsync(model.UserName, model.Email, model.Password);
            return this.Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return this.Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] UserLoginDto model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var user = await this.userService.LoginAsync(model.Email, model.Password);
        return user == null ? this.Unauthorized() : this.Ok(user);
    }
}
