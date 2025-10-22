using System.Globalization;
using System.Security.Claims;
using Application.Interfaces;
using Contracts.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = Core.Enums.TaskStatus;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService taskService;

    public TaskController(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll()
    {
        var userId = this.GetUserId();
        var tasks = await this.taskService.GetAllAsync(userId);
        return this.Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetById(int id)
    {
        var userId = this.GetUserId();
        var task = await this.taskService.GetTaskByIdAsync(id, userId)
            ?? throw new KeyNotFoundException("Task not found.");
        return this.Ok(task);
    }

    [HttpPost("{todoListId:int}/tasks")]
    public async Task<ActionResult<TaskDto>> AddTask(int todoListId, [FromBody] TaskCreateDto dto)
    {
        if (dto is null || !this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var userId = this.GetUserId();
        var created = await this.taskService.AddTaskAsync(todoListId, dto, userId);
        return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskDto>> Update(int id, [FromBody] TaskEditDto dto)
    {
        if (dto is null || !this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var userId = this.GetUserId();
        var updated = await this.taskService.UpdateTaskAsync(id, dto, userId)
            ?? throw new KeyNotFoundException("Task not found.");
        return this.Ok(updated);
    }

    [HttpPost("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto dto)
    {
        if (dto is null)
        {
            return this.BadRequest("Status data is required.");
        }

        var userId = this.GetUserId();
        var changed = await this.taskService.ChangeStatusAsync(id, userId, dto);
        if (!changed)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        return this.Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = this.GetUserId();
        var deleted = await this.taskService.DeleteTaskAsync(id, userId);
        if (!deleted)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        return this.Ok();
    }

    [HttpGet("assigned")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAssigned([FromQuery] TaskStatus? status = null)
    {
        var userId = this.GetUserId();
        var tasks = await this.taskService.GetAssignedTasksAsync(userId, status);
        return this.Ok(tasks);
    }

    [HttpGet("{tagName}")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetByTag(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return this.BadRequest("Tag name is required.");
        }

        var userId = this.GetUserId();
        var tasks = await this.taskService.GetTasksByTagAsync(tagName, userId);
        return this.Ok(tasks);
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
