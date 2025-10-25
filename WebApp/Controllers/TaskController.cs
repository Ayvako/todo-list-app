using System.Globalization;
using System.Security.Claims;
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

    [HttpGet]
    public async Task<IActionResult> Index(
     string? searchTitle = null,
     string createdRange = "all",
     string dueFilter = "all")
    {
        var tasks = await this.taskService.GetFilteredTasksAsync(searchTitle, createdRange, dueFilter);

        this.ViewBag.SearchTitle = searchTitle;
        this.ViewBag.CreatedRange = createdRange;
        this.ViewBag.DueFilter = dueFilter;

        return this.View(tasks);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, int? editingCommentId = null)
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

        this.ViewData["EditingCommentId"] = editingCommentId;
        this.ViewBag.UserId = this.GetUserId();

        return this.View(task);
    }

    [HttpGet]
    public async Task<IActionResult> AssignedTasks(TaskStatus? status = TaskStatus.InProgress, string? sortBy = "name", string? sortOrder = "asc")
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest("Invalid request");
        }

        var tasks = await this.taskService.GetSortedAssignedTasks(status, sortBy, sortOrder);

        sortBy ??= "name";

        sortOrder ??= "asc";

        this.ViewBag.SelectedStatus = status;
        this.ViewBag.SortBy = sortBy.ToLower(CultureInfo.CurrentCulture);
        this.ViewBag.SortOrder = sortOrder.ToLower(CultureInfo.CurrentCulture);

        return this.View(tasks);
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
    public async Task<IActionResult> Delete(int id, Uri? returnUrl)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest("Invalid request");
        }

        _ = await this.taskService.DeleteTaskAsync(id);

        if (!string.IsNullOrEmpty(returnUrl?.OriginalString))
        {
            return this.Redirect(returnUrl.OriginalString);
        }

        return this.RedirectToAction("Index");
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
            Tags = task.Tags,
        };

        return this.View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TaskEditModel model, Uri? returnUrl)
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

        if (!string.IsNullOrEmpty(returnUrl?.OriginalString))
        {
            return this.Redirect(returnUrl.OriginalString);
        }

        return this.RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, ChangeStatusModel model, Uri? returnUrl)
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

        if (!string.IsNullOrEmpty(returnUrl?.OriginalString))
        {
            return this.Redirect(returnUrl.OriginalString);
        }

        return this.RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> TasksByTag(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return this.BadRequest();
        }

        var tasks = await this.taskService.GetTasksByTagAsync(tagName);
        return this.View(tasks);
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
