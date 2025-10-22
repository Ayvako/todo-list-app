using System.Globalization;
using System.Security.Claims;
using Application.Interfaces;
using Contracts.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagController : ControllerBase
{
    private readonly ITagService tagService;
    private readonly ITaskService taskService;

    public TagController(ITagService tagService, ITaskService taskService)
    {
        this.tagService = tagService;
        this.taskService = taskService;
    }

    [HttpPost("{taskId:int}/tags")]
    public async Task<ActionResult<IEnumerable<TagDto>>> AddTag(int taskId, [FromBody] TagCreateDto model)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.Name) || !this.ModelState.IsValid)
        {
            return this.BadRequest("Tag name is required.");
        }

        var userId = this.GetUserId();
        var success = await this.tagService.AddTagAsync(taskId, model.Name, userId);
        if (!success)
        {
            throw new KeyNotFoundException("Task not found or tag could not be added.");
        }

        var updatedTask = await this.taskService.GetTaskByIdAsync(taskId, userId);
        return this.Ok(updatedTask?.Tags ?? new List<TagDto>());
    }

    [HttpDelete("{taskId:int}/tags")]
    public async Task<IActionResult> RemoveTag(int taskId, [FromBody] TagCreateDto model)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.Name) || !this.ModelState.IsValid)
        {
            return this.BadRequest("Tag name is required.");
        }

        var userId = this.GetUserId();
        var success = await this.tagService.RemoveTagAsync(taskId, model.Name, userId);
        if (!success)
        {
            throw new KeyNotFoundException("Task not found or tag could not be removed.");
        }

        return this.Ok();
    }

    [HttpGet("tags")]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
    {
        var userId = this.GetUserId();
        var tags = await this.tagService.GetTagsForUserAsync(userId);
        return this.Ok(tags);
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
