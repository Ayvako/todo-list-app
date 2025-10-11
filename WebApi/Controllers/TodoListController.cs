using System.Globalization;
using System.Security.Claims;
using Application.Interfaces;
using Contracts.TodoLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Tasks;
using WebApp.Models.TodoLists;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoListController : ControllerBase
{
    private readonly ITodoListService service;

    public TodoListController(ITodoListService service)
    {
        this.service = service;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<TodoListWebApiModel>>> GetAll()
    {
        var userId = this.GetUserId();
        var lists = await this.service.GetAllAsync(userId);
        return this.Ok(lists);
    }

    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<TodoListWebApiModel>>> GetUserLists()
    {
        var userId = this.GetUserId();
        var lists = await this.service.GetByUserAsync(userId);
        return this.Ok(lists);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListWebApiModel>> GetById(int id)
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
    public async Task<ActionResult<IEnumerable<TaskWebApiModel>>> GetTasks(int id)
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
    public async Task<ActionResult<TodoListWebApiModel>> Add([FromBody] TodoListCreateDto model)
    {
        var userId = this.GetUserId();
        var created = await this.service.AddAsync(model, userId);
        return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoListWebApiModel>> Update(int id, [FromBody] TodoListUpdateDto model)
    {
        var userId = this.GetUserId();

        try
        {
            var updated = await this.service.UpdateAsync(id, model, userId);
            return updated == null ? this.NotFound() : this.Ok(updated);
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

        try
        {
            var success = await this.service.ShareAsync(id, model.UserId, model.Role, userId);
            return success ? this.Ok() : this.BadRequest("Пользователь уже имеет доступ");
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

    [HttpPost("{id}/revoke")]
    public async Task<IActionResult> Revoke(int id, [FromBody] RevokeDto model)
    {
        var userId = this.GetUserId();

        try
        {
            var success = await this.service.RevokeAccessAsync(id, model.UserId, userId);
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

    private int GetUserId() =>
        int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier)!, CultureInfo.InvariantCulture);
}
