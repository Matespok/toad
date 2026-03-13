using hopefullyAWebForum.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hopefullyAWebForum.Pages;

public class FocusPost : PageModel
{
    public Post focusedPost { get; set; }
    public List<Comment>? CommentList { get; set; } = new();
    
    [BindProperty]
    public string Reply { get; set; }
    
    [BindProperty]
    public int? ParentComment { get; set; }

    public void OnGet(int id)
    {
        focusedPost = DbMethods.ShowPost(id);
        CommentList = DbMethods.ShowComments(id);
    }

    public IActionResult OnPost(int id)
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId");
        if (currentUserId.HasValue && !string.IsNullOrEmpty(Reply))
        {
            DbMethods.AddComment(currentUserId.Value, id, ParentComment, Reply);
            return RedirectToPage(new { id = id });
        }
    
        return RedirectToPage(new { id = id });
    }
}
