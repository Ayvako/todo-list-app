using Application.Interfaces;
using Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Users;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;
    private readonly IJwtService jwtService;

    public UserController(IUserService userService, IJwtService jwtService)
    {
        this.userService = userService;
        this.jwtService = jwtService;
    }

    [HttpGet("by-name/{username}")]
    public async Task<ActionResult<UserWebApiModel?>> GetByName(string username)
    {
        var user = await userService.GetUserByNameAsync(username);
        if (user == null)
        {
            return NotFound();
        }

        return new UserWebApiModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role,
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserLoginResponseDto>> Register([FromBody] UserRegisterDto model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var user = await this.userService.RegisterAsync(model.UserName, model.Email, model.Password);
            ArgumentNullException.ThrowIfNull(user);

            var token = this.jwtService.GenerateToken(user);

            return this.Ok(new { user, token });
        }
        catch (InvalidOperationException ex)
        {
            return this.Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResponseDto>> Login([FromBody] UserLoginDto model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        ArgumentNullException.ThrowIfNull(model);

        var user = await this.userService.LoginAsync(model.Email, model.Password);
        if (user == null)
        {
            return this.Unauthorized();
        }

        var token = this.jwtService.GenerateToken(user);

        var response = new UserLoginResponseDto
        {
            Token = token,
            User = user,
        };

        return this.Ok(response);
    }
}
