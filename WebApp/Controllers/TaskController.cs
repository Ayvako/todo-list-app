using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Interfaces;
using WebApp.Models.Tasks;
using TaskStatus = Core.Enums.TaskStatus;

namespace WebApp.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ITaskWebApiService taskService;

    public TaskController(ITaskWebApiService taskService)
    {
        this.taskService = taskService;
    }

    //[HttpGet]
    //public async Task<IActionResult> Index()
    //{
    //    var lists = await this.taskService.GetAllAsync();
    //    return this.View(lists);
    //}

    [HttpGet]
    public async Task<IActionResult> Index(TaskStatus? status = TaskStatus.InProgress)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest("Invalid request");
        }

        var lists = await this.taskService.GetAssignedTasksAsync(status);
        ViewBag.SelectedStatus = status;
        return this.View(lists);
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
        return this.RedirectToAction("Details", "TodoList");
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
            AssigneeName = task.Assignee?.UserName,
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

        return this.RedirectToAction("Details", "TodoList");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, ChangeStatusModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        if (model is null || !Enum.IsDefined(model.NewStatus))
        {
            return this.BadRequest("Invalid status value.");
        }

        var result = await this.taskService.ChangeStatusAsync(id, model);

        if (!result)
        {
            return this.NotFound($"Error ChangeStatus.");
        }

        return this.RedirectToAction("Index", "Task");
    }
}
