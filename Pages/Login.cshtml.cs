using hopefullyAWebForum.Pages.Data;
using hopefullyAWebForum.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hopefullyAWebForum.Pages;

public class Login : PageModel
{
    private readonly DbMethods _db = new();

    [BindProperty]
    public string Uname { get; set; }

    [BindProperty]
    public string Pass { get; set; }

    public string? WrongLogin { get; set; }

    public async Task<IActionResult> OnPost()
    {
        int? userId = await _db.AuthenticateUserAsync(Uname, Pass);

        if (userId != null)
        {
            // OPRAVA: Názvy klíčů musí přesně odpovídat tomu, co čteš v Dashboardu
            HttpContext.Session.SetInt32("UserId", userId.Value);
            HttpContext.Session.SetString("Username", Uname);

            return RedirectToPage("/ViewPosts");
        }
        else
        {
            WrongLogin = "Bad username or password";
            return Page();
        }
    }
}
