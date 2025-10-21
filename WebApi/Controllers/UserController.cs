using System.Globalization;
using System.Security.Claims;
using Application.Interfaces;
using Contracts.Users;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<UserRegisterResponseDto>> Register([FromBody] UserRegisterDto model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var user = await this.userService.RegisterAsync(model.UserName, model.Email, model.Password);
            if (user == null)
            {
                return this.BadRequest("Failed to register user");
            }

            var response = new UserRegisterResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
            };

            return this.CreatedAtAction(nameof(this.GetUserById), new { id = user.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return this.Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetUserById()
    {
        var userId = this.GetUserId();

        var user = await this.userService.GetByIdAsync(userId);
        if (user == null)
        {
            return this.NotFound();
        }

        var response = new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
        };

        return this.Ok(response);
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
            User = new UserResponseDto()
            {
                Email = user.Email,
                Id = user.Id,
                UserName = user.UserName,
            },
            Token = token,
        };

        return this.Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        await this.userService.SendPasswordResetAsync(dto.Email);

        return this.Ok(new { Message = "If this email exists, you will receive instructions." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        bool result = await this.userService.ResetPasswordAsync(dto.Email, dto.Token, dto.NewPassword);

        if (!result)
        {
            return this.BadRequest(new { Message = "Invalid token or token expired." });
        }

        return this.Ok(new { Message = "Password successfully reset." });
    }

    private int GetUserId()
    {
        var id = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(id))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        return int.Parse(id, CultureInfo.InvariantCulture);
    }
}
