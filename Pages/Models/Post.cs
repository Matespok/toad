namespace hopefullyAWebForum.Pages.Models;

public class Post
{
    public int PostId { get; set; }
    public string AuthorName { get; set; } // UserId -> join
    public string Topic { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}
