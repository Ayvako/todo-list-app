using Microsoft.AspNetCore.Mvc;
using WebApp.Interfaces;
using WebApp.Models.Tasks;

namespace WebApp.Controllers;

public class TaskController : Controller
{
    private readonly ITaskWebApiService taskService;

    public TaskController(ITaskWebApiService taskService)
    {
        this.taskService = taskService;
    }

    [HttpGet]
    public IActionResult Add(int todoListId)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest("Invalid request");
        }

        var model = new TaskCreateModel
        {
            TodoListId = todoListId,
        };

        return this.View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(TaskCreateModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        if (model == null)
        {
            return this.BadRequest("Model is null");
        }

        var created = await this.taskService.AddTaskAsync(model.TodoListId, model);

        if (created == null)
        {
            this.ModelState.AddModelError(string.Empty, "Ошибка при создании задачи");
            return this.View(model);
        }

        return this.RedirectToAction("Details", "TodoList", new { id = model.TodoListId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest("Invalid request");
        }

        _ = await this.taskService.DeleteTaskAsync(id);
        return this.RedirectToAction("Index", "TodoList");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest("Invalid request");
        }

        var task = await this.taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return this.NotFound();
        }

        TaskEditModel model = new TaskEditModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            AssigneeName = task.AssigneeName,
        };

        return this.View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TaskEditModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        if (model == null)
        {
            return this.BadRequest("Model is null");
        }

        var updated = await this.taskService.UpdateTaskAsync(model.Id, model);

        if (updated == null)
        {
            this.ModelState.AddModelError(string.Empty, "Ошибка при обновлении задачи");
            return this.View(model);
        }

        return this.RedirectToAction("Index", "TodoList");
    }
}
