using hopefullyAWebForum.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hopefullyAWebForum.Pages;

public class FocusPost : PageModel
{
    private readonly DbMethods _db = new();

    public Post focusedPost { get; set; }
    public List<Comment>? CommentList { get; set; } = new();

    [BindProperty]
    public string Reply { get; set; }

    [BindProperty]
    public int? ParentComment { get; set; }

    public async Task OnGetAsync(int id)
    {
        focusedPost = await _db.ShowPostAsync(id);
        CommentList = await _db.ShowCommentsAsync(id);
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId");
        if (currentUserId.HasValue && !string.IsNullOrEmpty(Reply))
        {
            await _db.AddCommentAsync(currentUserId.Value, id, ParentComment, Reply);
            return RedirectToPage(new { id = id });
        }

        return RedirectToPage(new { id = id });
    }
}
