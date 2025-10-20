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

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDto>> GetById(int id)
    {
        var userId = this.GetUserId();

        try
        {
            var list = await this.service.GetByIdAsync(id, userId);
            return list == null ? this.NotFound() : this.Ok(list);
        }
        catch (UnauthorizedAccessException)
        {
            return this.Forbid();
        }
    }

    [HttpGet("{id}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(int id)
    {
        var userId = this.GetUserId();

        try
        {
            var tasks = await this.service.GetTasksByListIdAsync(id, userId);
            return this.Ok(tasks);
        }
        catch (UnauthorizedAccessException)
        {
            return this.Forbid();
        }
    }

    [HttpPost]
    public async Task<ActionResult<TodoListCreateDto>> Add([FromBody] TodoListCreateDto model)
    {
        var userId = this.GetUserId();
        var created = await this.service.AddAsync(model, userId);

        var responce = new TodoListCreateDto()
        {
            Description = created.Description,
            Title = created.Title,
        };

        return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, responce);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoListUpdateDto>> Update(int id, [FromBody] TodoListUpdateDto model)
    {
        var userId = this.GetUserId();

        try
        {
            var updated = await this.service.UpdateAsync(id, model, userId);

            if (updated == null)
            {
                return this.NotFound();
            }

            var responce = new TodoListCreateDto()
            {
                Description = updated.Description,
                Title = updated.Title,
            };

            return this.Ok(responce);
        }
        catch (UnauthorizedAccessException)
        {
            return this.Forbid();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = this.GetUserId();

        try
        {
            var deleted = await this.service.DeleteAsync(id, userId);
            return deleted ? this.Ok() : this.NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return this.Forbid();
        }
    }

    [HttpPost("{id}/share")]
    public async Task<IActionResult> Share(int id, [FromBody] ShareDto model)
    {
        var userId = this.GetUserId();

        ArgumentNullException.ThrowIfNull(model);

        var targetUser = await this.userService.GetUserByNameAsync(model.UserName);
        if (targetUser == null)
        {
            return this.NotFound("User not found");
        }

        try
        {
            var success = await this.service.ShareAsync(id, targetUser.Id, model.Role, userId);
            return success ? this.Ok() : this.BadRequest("Failed to share the list.");
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/revoke")]
    public async Task<IActionResult> Revoke(int id, [FromBody] RevokeDto model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var userId = this.GetUserId();
        var user = await this.userService.GetUserByNameAsync(model.UserName);
        if (user == null)
        {
            return this.NotFound("User not found");
        }

        try
        {
            var success = await this.service.RevokeAccessAsync(id, user.Id, userId);
            return success ? this.Ok() : this.NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return this.Forbid();
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
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
