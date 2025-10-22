using Application.Interfaces;
using Contracts.Tags;
using Core.Interfaces;
using Infrastructure.Repositories;

namespace Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository tagRepository;
    private readonly ITodoListService todoListService;
    private readonly ITaskRepository taskRepository;

    public TagService(ITagRepository repository, ITodoListService todoListService, ITaskRepository taskRepository)
    {
        this.tagRepository = repository;
        this.todoListService = todoListService;
        this.taskRepository = taskRepository;
    }

    public async Task<bool> AddTagAsync(int taskId, string tagName, int userId)
    {
        var task = await this.taskRepository.GetTaskByIdAsync(taskId) ?? throw new KeyNotFoundException("Task not found.");
        var canEdit = await this.todoListService.CanEditAsync(task.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to add tags to this task.");
        }

        return await this.tagRepository.AddTagAsync(taskId, tagName);
    }

    public async Task<bool> RemoveTagAsync(int taskId, string tagName, int userId)
    {
        var task = await this.taskRepository.GetTaskByIdAsync(taskId) ?? throw new KeyNotFoundException("Task not found.");
        var canEdit = await this.todoListService.CanEditAsync(task.TodoListId, userId);
        if (!canEdit)
        {
            throw new UnauthorizedAccessException("You don't have permission to remove tags from this task.");
        }

        return await this.tagRepository.RemoveTagAsync(taskId, tagName);
    }

    public async Task<List<TagDto>> GetTagsForUserAsync(int userId)
    {
        var tags = await this.tagRepository.GetTagsForUserAsync(userId);
        return tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
        }).ToList();
    }
}
