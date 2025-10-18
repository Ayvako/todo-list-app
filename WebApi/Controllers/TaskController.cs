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
        var lists = await this.taskService.GetAllAsync(userId);
        return this.Ok(lists);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> Details(int id)
    {
        TaskDto? task = await this.taskService.GetTaskByIdAsync(id);

        return task == null ? this.NotFound() : this.Ok(task);
    }

    [HttpPost("{todoListId}/tasks")]
    public async Task<ActionResult<TaskCreateDto>> AddTask(int todoListId, [FromBody] TaskCreateDto model)
    {
        var userId = this.GetUserId();

        try
        {
            var task = await this.taskService.AddTaskAsync(todoListId, model, userId);

            var responce = new TaskCreateDto()
            {
                Description = task.Description,
                DueDate = task.DueDate,
                Title = task.Title,
            };

            return this.CreatedAtAction(nameof(this.Details), new { id = task.Id }, responce);
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.Forbid(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskEditDto>> EditTask(int id, [FromBody] TaskEditDto model)
    {
        var userId = this.GetUserId();

        try
        {
            var updated = await this.taskService.UpdateTaskAsync(id, model, userId);

            if (model == null)
            {
                return this.BadRequest("Model is null");
            }

            if (updated == null)
            {
                return this.NotFound();
            }

            var responce = new TaskEditDto()
            {
                Description = updated.Description,
                DueDate = updated.DueDate,
                Title = updated.Title,
                Status = updated.Status,
                AssigneeName = model.AssigneeName,
            };

            return this.Ok(responce);
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.Forbid(ex.Message);
        }
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto dto)
    {
        var userId = this.GetUserId();

        try
        {
            var changed = await this.taskService.ChangeStatusAsync(id, userId, dto);
            return changed ? this.Ok(new { success = true }) : this.NotFound();
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
            return deleted ? this.Ok(new { success = true }) : this.NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.Forbid(ex.Message);
        }
    }

    [HttpGet("assigned")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAssignedTasksAsync([FromQuery] TaskStatus? status = TaskStatus.InProgress)
    {
        var userId = this.GetUserId();
        var lists = await this.taskService.GetAssignedTasksAsync(userId, status);
        return this.Ok(lists);
    }

    private int GetUserId() =>
                int.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier)!, CultureInfo.InvariantCulture);
}
