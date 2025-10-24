using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Interfaces;
using WebApp.Models.Tags;

namespace WebApp.Controllers;

[Authorize]
public class TagController : Controller
{
    private readonly ITagWebApiService tagService;

    public TagController(ITagWebApiService tagService)
    {
        this.tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tags = await this.tagService.GetTagsForCurrentUserAsync();
        return this.View(tags);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTag(int taskId, TagModel model, string? returnUrl)
    {
        if (!this.ModelState.IsValid || model is null)
        {
            return this.BadRequest("Invalid request");
        }

        _ = await this.tagService.AddTagAsync(taskId, model.Name);
        return this.Redirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveTag(int taskId, TagModel model, string? returnUrl)
    {
        if (!this.ModelState.IsValid || model is null)
        {
            return this.BadRequest("Invalid request");
        }

        _ = await this.tagService.RemoveTagAsync(taskId, model.Name);
        return this.Redirect(returnUrl);
    }
}
