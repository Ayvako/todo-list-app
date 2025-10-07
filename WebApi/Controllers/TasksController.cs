using System.Globalization;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskWebApiModel>> GetTaskById(int id)
    {
        var task = await this.taskService.GetTaskByIdAsync(id);
        return task == null ? this.NotFound() : this.Ok(task);
    }

    [HttpPost("{todoListId}/tasks")]
    public async Task<ActionResult<TaskWebApiModel>> AddTask(int todoListId, [FromBody] TaskCreateDto model)
    {
        var userId = this.GetUserId();

        try
        {
            var task = await this.taskService.AddTaskAsync(todoListId, model, userId);
            return this.CreatedAtAction(nameof(this.GetTaskById), new { id = task.Id }, task);
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.Forbid(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskWebApiModel>> EditTask(int id, [FromBody] TaskEditDto model)
    {
        var userId = this.GetUserId();

        try
        {
            var updated = await this.taskService.UpdateTaskAsync(id, model, userId);
            return updated == null ? this.NotFound() : this.Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.Forbid(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = this.GetUserId();

        try
        {
            var deleted = await this.taskService.DeleteTaskAsync(id, userId);
            return deleted ? this.NoContent() : this.NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.Forbid(ex.Message);
        }
    }

    private int GetUserId() =>
                int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier)!, CultureInfo.InvariantCulture);
}
