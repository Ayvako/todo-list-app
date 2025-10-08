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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListWebApiModel>>> GetAll()
    {
        var userId = this.GetUserId();
        var lists = await this.service.GetAllAsync(userId);
        return this.Ok(lists);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListWebApiModel>> GetById(int id)
    {
        var userId = this.GetUserId();
        var list = await this.service.GetByIdAsync(id, userId);
        return list == null ? this.NotFound() : this.Ok(list);
    }

    [HttpGet("{id}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskWebApiModel>>> GetTasks(int id)
    {
        var userId = this.GetUserId();

        var canView = await this.service.CanViewAsync(id, userId);
        if (!canView)
        {
            return this.Forbid();
        }

        var tasks = await this.service.GetTasksByListIdAsync(id);
        return tasks.Any() ? this.Ok(tasks) : this.NotFound();
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
        catch (UnauthorizedAccessException ex)
        {
            return this.Forbid(ex.Message);
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
        catch (UnauthorizedAccessException ex)
        {
            return this.Forbid(ex.Message);
        }
    }

    private int GetUserId() =>
        int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier)!, CultureInfo.InvariantCulture);
}
