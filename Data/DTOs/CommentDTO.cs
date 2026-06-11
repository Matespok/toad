namespace toad.DTOs;

public class CommentDTO
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int PostId { get; set; }
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CommentedAt { get; set; }
}
