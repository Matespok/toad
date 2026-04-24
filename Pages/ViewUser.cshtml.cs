using hopefullyAWebForum.Pages.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hopefullyAWebForum.Pages.Shared;

public class ViewUser : PageModel
{
    private readonly DbMethods _db = new();

    public User SelectedUser { get; set; } = new();

    public List<Post> Posts { get; set; } = new();

    public List<Comment> Comments { get; set; } = new();

    public async Task OnGetAsync(int id)
    {
        SelectedUser = await _db.GetUserInfoAsync(id);
        Posts = await _db.GetUserPostsAsync(id);
        Comments = await _db.GetUserCommentsAsync(id);
    }
}
