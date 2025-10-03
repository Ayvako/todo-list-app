using Application.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Tasks;

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
        var task = await taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost("{todoListId}/tasks")]
    public async Task<ActionResult<TaskWebApiModel>> AddTask(int todoListId, [FromBody] TaskCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var task = await taskService.AddTaskAsync(todoListId, model);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskWebApiModel>> EditTask(int id, [FromBody] TaskEditModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await taskService.UpdateTaskAsync(id, model);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var deleted = await taskService.DeleteTaskAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
