using hopefullyAWebForum.Pages.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hopefullyAWebForum.Pages;


public class ViewPosts : PageModel
{
    public List<Post> Posts { get; set; } = new();

    public void OnGet()
    {
        Posts = DbMethods.ReadPost();
    }
}

