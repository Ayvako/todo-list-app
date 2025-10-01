using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService service;

    public TasksController(ITaskService taskService)
    {
        this.service = taskService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskModel>> GetTaskById(int id)
    {
        var task = await this.service.GetTaskByIdAsync(id);
        if (task == null)
        {
            return this.NotFound();
        }

        return this.Ok(task);
    }

    [HttpPost("{todoListId}/tasks")]
    public async Task<ActionResult<TaskModel>> AddTask(int todoListId, [FromBody] TaskCreateModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var task = await this.service.AddTaskAsync(todoListId, model);

            return this.CreatedAtAction(nameof(this.GetTaskById), new { id = task.Id }, task);
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var result = await this.service.DeleteTaskAsync(id);

        if (!result)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskModel>> EditTask(int id, [FromBody] TaskEditModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var updated = await this.service.UpdateTaskAsync(id, model);

        if (updated == null)
        {
            return this.NotFound();
        }

        return this.Ok(updated);
    }
}
