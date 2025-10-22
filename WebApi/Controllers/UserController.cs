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
        if (model is null || !this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var user = await this.userService.RegisterAsync(model.UserName, model.Email, model.Password);
        if (user is null)
        {
            throw new InvalidOperationException("Failed to register user.");
        }

        return this.CreatedAtAction(nameof(this.GetCurrentUser), new { id = user.Id }, new UserRegisterResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        var userId = this.GetUserId();
        var user = await this.userService.GetByIdAsync(userId);
        if (user is null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        return this.Ok(new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResponseDto>> Login([FromBody] UserLoginDto model)
    {
        if (model is null || !this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var user = await this.userService.LoginAsync(model.Email, model.Password);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var token = this.jwtService.GenerateToken(user);

        return this.Ok(new UserLoginResponseDto
        {
            User = new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
            },
            Token = token,
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        if (dto is null || !this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        await this.userService.SendPasswordResetAsync(dto.Email);
        return this.Ok(new { Message = "If this email exists, you will receive instructions." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (dto is null || !this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var success = await this.userService.ResetPasswordAsync(dto.Email, dto.Token, dto.NewPassword);
        if (!success)
        {
            throw new ArgumentException("Invalid token or token expired.");
        }

        return this.Ok(new { Message = "Password successfully reset." });
    }

    private int GetUserId()
    {
        var idClaim = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, NumberStyles.Integer, CultureInfo.InvariantCulture, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return userId;
    }
}
