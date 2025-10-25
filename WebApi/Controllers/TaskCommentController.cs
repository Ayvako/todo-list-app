using System.Globalization;
using System.Security.Claims;
using Application.Interfaces;
using Contracts.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/Task/{taskId}/comments")]
    [Authorize]
    public class TaskCommentController : ControllerBase
    {
        private readonly ITaskCommentService commentService;

        public TaskCommentController(ITaskCommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int taskId)
        {
            var comments = await this.commentService.GetCommentsAsync(taskId);
            return this.Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int taskId, [FromBody] TaskCommentCreateDto dto)
        {
            var userId = this.GetUserId();
            var comment = await this.commentService.AddCommentAsync(taskId, userId, dto);
            return this.Ok(comment);
        }

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] TaskCommentUpdateDto dto)
        {
            var userId = this.GetUserId();
            var updated = await this.commentService.UpdateCommentAsync(commentId, userId, dto);
            if (updated == null)
            {
                return this.NotFound();
            }

            return this.Ok(updated);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = this.GetUserId();
            var success = await this.commentService.DeleteCommentAsync(commentId, userId);
            if (!success)
            {
                return this.Forbid();
            }

            return this.NoContent();
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
}
