namespace WebApp.Models.Comments;

public class TaskCommentModel
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public string AuthorName { get; set; } = string.Empty;

    public int AuthorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
