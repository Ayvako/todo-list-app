using Application.Services.Interfaces;
using Contracts.TodoLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListWebApiModel>>> GetAll()
    {
        var userId = GetUserId();
        var lists = await service.GetAllAsync(userId);
        return Ok(lists);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListWebApiModel>> GetById(int id)
    {
        var userId = GetUserId();
        var list = await service.GetByIdAsync(id, userId);
        return list == null ? NotFound() : Ok(list);
    }

    [HttpGet("{id}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskWebApiModel>>> GetTasks(int id)
    {
        var userId = GetUserId();

        var canView = await service.CanViewAsync(id, userId);
        if (!canView)
        {
            return Forbid();
        }

        var tasks = await service.GetTasksByListIdAsync(id);
        return tasks.Any() ? Ok(tasks) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<TodoListWebApiModel>> Add([FromBody] TodoListCreateDto model)
    {
        var userId = GetUserId();
        var created = await service.AddAsync(model, userId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoListWebApiModel>> Update(int id, [FromBody] TodoListUpdateDto model)
    {
        var userId = GetUserId();

        try
        {
            var updated = await service.UpdateAsync(id, model, userId);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();

        try
        {
            var deleted = await service.DeleteAsync(id, userId);
            return deleted ? Ok() : NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
