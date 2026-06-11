using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using toad.Repositories; // Nezapomeň přidat odkaz na složku s repozitáři

namespace toad.Pages;

public class Login : PageModel
{
    private readonly IForumRepository _repo;

    public Login(IForumRepository repo)
    {
        _repo = repo;
    }

    [BindProperty]
    public string Uname { get; set; } = string.Empty;

    [BindProperty]
    public string Pass { get; set; } = string.Empty;

    public string? WrongLogin { get; set; }

    public async Task<IActionResult> OnPost()
    {
        int? userId = await _repo.AuthenticateUserAsync(Uname, Pass);

        if (userId != null)
        {
            HttpContext.Session.SetInt32("UserId", userId.Value);
            HttpContext.Session.SetString("Username", Uname);

            return RedirectToPage("/Posts/Index");
        }
        else
        {
            WrongLogin = "Bad username or password";
            return Page();
        }
    }
}
