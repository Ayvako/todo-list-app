using System.Security.Claims;
using Application.Services.Interfaces;
using Contracts.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Tasks;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService taskService;

    public TasksController(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskWebApiModel>> GetTaskById(int id)
    {
        var task = await taskService.GetTaskByIdAsync(id);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpPost("{todoListId}/tasks")]
    public async Task<ActionResult<TaskWebApiModel>> AddTask(int todoListId, [FromBody] TaskCreateDto model)
    {
        var userId = GetUserId();

        try
        {
            var task = await taskService.AddTaskAsync(todoListId, model, userId);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskWebApiModel>> EditTask(int id, [FromBody] TaskEditDto model)
    {
        var userId = GetUserId();

        try
        {
            var updated = await taskService.UpdateTaskAsync(id, model, userId);
            return updated == null ? NotFound() : Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = GetUserId();

        try
        {
            var deleted = await taskService.DeleteTaskAsync(id, userId);
            return deleted ? NoContent() : NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
