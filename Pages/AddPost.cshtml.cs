using hopefullyAWebForum.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hopefullyAWebForum.Pages;

public class AddPost : PageModel
{
    [BindProperty]
    public string Topic { get; set; }

    [BindProperty]
    public string Content { get; set; }

    public void OnPost()
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId");

        if (currentUserId == null)
        {
            Response.Redirect("/Login");
            return;
        }

        Console.WriteLine($"User ID: {currentUserId} made a post: {Topic}");

        DbMethods.AddPost(currentUserId.Value, Topic, Content);

        Response.Redirect("/ViewPosts");
    }
}

