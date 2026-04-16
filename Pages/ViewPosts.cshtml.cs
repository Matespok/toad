using hopefullyAWebForum.Pages.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hopefullyAWebForum.Pages;

public class ViewPosts : PageModel
{
    private readonly DbMethods _db = new();
    public List<Post> Posts { get; set; } = new();

    public async Task OnGetAsync()
    {
        Posts = await _db.ReadPostAsync();
    }
}
