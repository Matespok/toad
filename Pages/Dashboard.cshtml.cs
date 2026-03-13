using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hopefullyAWebForum.Pages;

public class Dashboard : PageModel
{
    public int? LoggedUserId { get; set; }
    public string? LoggedUserName { get; set; }
    public void OnGet()
    {
        LoggedUserId = HttpContext.Session.GetInt32("UserId");
        LoggedUserName = HttpContext.Session.GetString("Username");
    }
}