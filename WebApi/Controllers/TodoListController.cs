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
        => Ok(await service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListWebApiModel>> GetById(int id)
    {
        var list = await service.GetByIdAsync(id);
        return list == null ? NotFound() : Ok(list);
    }

    [HttpGet("{id}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskWebApiModel>>> GetTasks(int id)
    {
        var tasks = await service.GetTasksByListIdAsync(id);
        return tasks.Any() ? Ok(tasks) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<TodoListWebApiModel>> Add([FromBody] TodoListCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await service.AddAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoListWebApiModel>> Update(int id, [FromBody] TodoListUpdateDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await service.UpdateAsync(id, model);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await service.DeleteAsync(id) ? Ok() : NotFound();
}
