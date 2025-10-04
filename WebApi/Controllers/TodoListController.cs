using Application.Services;
using Contracts.TodoLists;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Tasks;
using WebApp.Models.TodoLists;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoListController : ControllerBase
{
    private readonly ITodoListService service;

    public TodoListController(ITodoListService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListWebApiModel>>> GetAll()
        => this.Ok(await this.service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListWebApiModel>> GetById(int id)
    {
        var list = await this.service.GetByIdAsync(id);
        return list == null ? this.NotFound() : this.Ok(list);
    }

    [HttpGet("{id}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskWebApiModel>>> GetTasks(int id)
    {
        var tasks = await this.service.GetTasksByListIdAsync(id);
        return tasks.Any() ? this.Ok(tasks) : this.NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<TodoListWebApiModel>> Add([FromBody] TodoListCreateDto model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var created = await this.service.AddAsync(model);
        return this.CreatedAtAction(nameof(this.GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoListWebApiModel>> Update(int id, [FromBody] TodoListUpdateDto model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var updated = await this.service.UpdateAsync(id, model);
        return updated == null ? this.NotFound() : this.Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await this.service.DeleteAsync(id) ? this.Ok() : this.NotFound();
}
