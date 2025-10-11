using Microsoft.AspNetCore.Mvc;
using WebApp.Interfaces;
using WebApp.Models.TodoLists;

namespace WebApp.Controllers;

public class TodoListController : Controller
{
    private readonly ITodoListWebApiService service;

    public TodoListController(ITodoListWebApiService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lists = await this.service.GetByUserAsync();
        return this.View(lists);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View();
        }

        var list = await this.service.GetByIdAsync(id);
        if (list == null)
        {
            this.TempData["ErrorMessage"] = "List not found or access denied.";
            return this.RedirectToAction("Index");
        }

        return this.View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return this.View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoListWebApiModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var created = await this.service.AddAsync(model);

        if (created == null)
        {
            this.ModelState.AddModelError(string.Empty, "Ошибка при создании списка");
            return this.View(model);
        }

        return this.RedirectToAction(nameof(this.Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest("Invalid request");
        }

        var list = await this.service.GetByIdAsync(id);
        if (list == null)
        {
            return this.NotFound();
        }

        return this.View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TodoListWebApiModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        var updated = await this.service.UpdateAsync(id, model);
        if (updated == null)
        {
            this.ModelState.AddModelError(string.Empty, "Ошибка при обновлении списка");
            return this.View(model);
        }

        return this.RedirectToAction(nameof(this.Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest("Invalid request");
        }

        var deleted = await this.service.DeleteAsync(id);
        if (!deleted)
        {
            return this.NotFound();
        }

        return this.RedirectToAction(nameof(this.Index));
    }
}
