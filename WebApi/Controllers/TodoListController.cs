using System.Globalization;
using System.Security.Claims;
using Application.Interfaces;
using Contracts.Tasks;
using Contracts.TodoLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoListController : ControllerBase
{
    private readonly ITodoListService service;
    private readonly IUserService userService;

    public TodoListController(ITodoListService service, IUserService userService)
    {
        this.service = service;
        this.userService = userService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<TodoListDto>>> GetAll()
    {
        var userId = this.GetUserId();
        var lists = await this.service.GetAllAsync(userId);
        return this.Ok(lists);
    }

    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<TodoListDto>>> GetUserLists()
    {
        var userId = this.GetUserId();
        var lists = await this.service.GetByUserAsync(userId);
        return this.Ok(lists);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoListDto>> GetById(int id)
    {
        var userId = this.GetUserId();
        var list = await this.service.GetByIdAsync(id, userId) ?? throw new KeyNotFoundException("Todo list not found.");
        return this.Ok(list);
    }

    [HttpGet("{id:int}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(int id)
    {
        var userId = this.GetUserId();
        var tasks = await this.service.GetTasksByListIdAsync(id, userId);
        return this.Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TodoListCreateDto>> Add([FromBody] TodoListCreateDto model)
    {
        if (model is null || !this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var userId = this.GetUserId();
        var created = await this.service.AddAsync(model, userId);

        return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, new TodoListCreateDto
        {
            Title = created.Title,
            Description = created.Description,
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoListUpdateDto>> Update(int id, [FromBody] TodoListUpdateDto model)
    {
        if (model is null || !this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var userId = this.GetUserId();
        var updated = await this.service.UpdateAsync(id, model, userId) ?? throw new KeyNotFoundException("Todo list not found.");
        return this.Ok(new TodoListCreateDto
        {
            Title = updated.Title,
            Description = updated.Description,
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = this.GetUserId();
        var deleted = await this.service.DeleteAsync(id, userId);
        if (!deleted)
        {
            throw new KeyNotFoundException("Todo list not found.");
        }

        return this.Ok();
    }

    [HttpPost("{id:int}/share")]
    public async Task<IActionResult> Share(int id, [FromBody] ShareDto model)
    {
        ValidateShareModel(model);
        return await this.ShareInternalAsync(id, model);
    }

    [HttpPost("{id:int}/revoke")]
    public async Task<IActionResult> Revoke(int id, [FromBody] RevokeDto model)
    {
        ValidateRevokeModel(model);
        return await this.RevokeInternalAsync(id, model);
    }

    private static void ValidateRevokeModel(RevokeDto model)
    {
        ArgumentNullException.ThrowIfNull(model);
    }

    private static void ValidateShareModel(ShareDto model)
    {
        ArgumentNullException.ThrowIfNull(model);
    }

    private async Task<IActionResult> RevokeInternalAsync(int id, RevokeDto model)
    {
        var userId = this.GetUserId();
        var targetUser = await this.userService.GetUserByNameAsync(model.UserName)
                         ?? throw new KeyNotFoundException("User not found.");

        var success = await this.service.RevokeAccessAsync(id, targetUser.Id, userId);
        if (!success)
        {
            throw new KeyNotFoundException("Failed to revoke access.");
        }

        return this.Ok();
    }

    private async Task<IActionResult> ShareInternalAsync(int id, ShareDto model)
    {
        var userId = this.GetUserId();
        var targetUser = await this.userService.GetUserByNameAsync(model.UserName)
                         ?? throw new KeyNotFoundException("User not found.");

        var success = await this.service.ShareAsync(id, targetUser.Id, model.Role, userId);
        if (!success)
        {
            throw new InvalidOperationException("Failed to share the list.");
        }

        return this.Ok();
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
