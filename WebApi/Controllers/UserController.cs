using Application.Services.Interfaces;
using Contracts.Users;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("register")]
    public async Task<ActionResult<UserLoginResponseDto>> Register([FromBody] UserRegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = await userService.RegisterAsync(model.UserName, model.Email, model.Password);

            var token = jwtService.GenerateToken(user);

            return this.Ok(new { user, token });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResponseDto>> Login([FromBody] UserLoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await userService.LoginAsync(model.Email, model.Password);
        if (user == null)
        {
            return Unauthorized();
        }

        var token = jwtService.GenerateToken(user);

        var response = new UserLoginResponseDto
        {
            Token = token,
            User = user
        };

        return Ok(response);
    }
}
