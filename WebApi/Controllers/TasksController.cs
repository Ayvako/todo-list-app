using Application.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Tasks;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        if (task == null)
        {
            return this.NotFound();
        }

        return this.Ok(task);
    }

    [HttpPost("{todoListId}/tasks")]
    public async Task<ActionResult<TaskWebApiModel>> AddTask(int todoListId, [FromBody] TaskCreateModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var task = await this.taskService.AddTaskAsync(todoListId, model);
            return this.CreatedAtAction(nameof(this.GetTaskById), new { id = task.Id }, task);
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskWebApiModel>> EditTask(int id, [FromBody] TaskEditModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var updated = await this.taskService.UpdateTaskAsync(id, model);
        if (updated == null)
        {
            return this.NotFound();
        }

        return this.Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var deleted = await this.taskService.DeleteTaskAsync(id);
        if (!deleted)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }
}
