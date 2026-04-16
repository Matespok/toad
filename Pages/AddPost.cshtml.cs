using hopefullyAWebForum.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hopefullyAWebForum.Pages;

public class AddPost : PageModel
{
    private readonly DbMethods _db = new();

    [BindProperty]
    public string Topic { get; set; }

    [BindProperty]
    public string Content { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId");

        if (currentUserId == null)
        {
            return RedirectToPage("/Login");
        }
        else
        {
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId");

        if (currentUserId == null)
        {
            return RedirectToPage("/Login");
        }

        await _db.AddPostAsync(currentUserId.Value, Topic, Content);

        return RedirectToPage("/ViewPosts");
    }
}
