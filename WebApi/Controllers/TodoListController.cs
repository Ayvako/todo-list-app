using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;
using WebApi.Services.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
#pragma warning disable IDE0079 // Remove unnecessary suppression
[SuppressMessage("Maintainability", "CA1515", Justification = "Controllers must be public for Swagger to work")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListDatabaseService service;

    public TodoListController(ITodoListDatabaseService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoList>>> GetAll()
    {
        var result = await this.service.GetAllAsync();
        return this.Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoList>> GetById(int id)
    {
        var result = await this.service.GetByIdAsync(id);
        if (result == null)
        {
            return this.NotFound();
        }

        return this.Ok(result);
    }

    [HttpGet("{id}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks(int id)
    {
        var tasks = await this.service.GetTasksByListIdAsync(id);

        if (!tasks.Any())
        {
            return this.NotFound();
        }

        return this.Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TodoList>> Add([FromBody] TodoListModel model)
    {
        var created = await this.service.AddAsync(model);
        return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoList>> Update(int id, [FromBody] TodoListModel model)
    {
        var updated = await this.service.UpdateAsync(id, model);

        if (updated == null)
        {
            return this.NotFound();
        }

        return this.Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await this.service.DeleteAsync(id);

        if (!deleted)
        {
            return this.NotFound();
        }

        return this.Ok();
    }
}
