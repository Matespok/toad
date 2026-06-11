using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using toad.Repositories;

namespace toad.Pages;

public class Register : PageModel
{
    private readonly IForumRepository _repo;

    public Register(IForumRepository repo)
    {
        _repo = repo;
    }

    public void OnGet() { }

    [BindProperty]
    public string Uname { get; set; } = string.Empty;

    [BindProperty]
    public string Pass { get; set; } = string.Empty;

    public string? WrongLogin { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        bool alreadyRegistered = await _repo.IsRegisteredAsync(Uname);

        if (alreadyRegistered)
        {
            WrongLogin = "Username already used, pick something different";
            return Page();
        }
        else
        {
            await _repo.AddUserAsync(Uname, Pass);
            return RedirectToPage("/Account/Login");
        }
    }
}
