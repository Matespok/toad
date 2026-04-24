namespace hopefullyAWebForum.Pages.Models;

public class Comment
{
    /*
             * Table comments
             * CommentId will create itself,
             * CommentedAt will create itself
             *
             * Int UserId
             * Int ParentCommentId -> nullable
             * string content
             * */

    public string AuthorName { get; set; }
    public int PostId { get; set; }
    public int? ParentCommentId { get; set; }
    public DateTime CommentedAt { get; set; }
    public string Content { get; set; }
}
