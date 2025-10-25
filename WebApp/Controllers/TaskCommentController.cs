using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Interfaces;
using WebApp.Models.Comments;

namespace WebApp.Controllers
{
    [Authorize]
    public class TaskCommentController : Controller
    {
        private readonly ITaskCommentWebApiService commentService;

        public TaskCommentController(ITaskCommentWebApiService commentService)
        {
            this.commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int taskId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var comments = await this.commentService.GetCommentsAsync(taskId);
            this.ViewBag.TaskId = taskId;
            return this.View(comments);
        }

        [HttpGet]
        public IActionResult Add(int taskId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var model = new TaskCommentCreateModel();
            this.ViewBag.TaskId = taskId;
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int taskId, TaskCommentCreateModel dto)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.TaskId = taskId;
                return this.View(dto);
            }

            _ = await this.commentService.AddCommentAsync(taskId, dto);

            return this.RedirectToAction("Details", "Task", new { id = taskId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int taskId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var comments = await this.commentService.GetCommentsAsync(taskId);
            var comment = comments.FirstOrDefault(c => c.Id == id);
            if (comment == null)
            {
                return this.NotFound();
            }

            return this.RedirectToAction("Details", "Task", new { id = taskId, editingCommentId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int taskId, int commentId, TaskCommentUpdateModel dto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            _ = await this.commentService.UpdateCommentAsync(taskId, commentId, dto);

            return this.RedirectToAction("Details", "Task", new { id = taskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int commentId, int taskId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            _ = await this.commentService.DeleteCommentAsync(taskId, commentId);

            return this.RedirectToAction("Details", "Task", new { id = taskId });
        }
    }
}
