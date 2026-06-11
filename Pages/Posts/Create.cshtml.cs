using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using toad.Repositories;

namespace toad.Pages;

public class Create : PageModel
{
    private readonly IForumRepository _repo;

    public Create(IForumRepository repo)
    {
        _repo = repo;
    }

    [BindProperty]
    public string Topic { get; set; } = string.Empty;

    [BindProperty]
    public string PostContent { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        int? currentUserId = HttpContext.Session.GetInt32("UserId");

        if (currentUserId == null)
        {
            return RedirectToPage("/Account/Login");
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

        // Voláme metodu přímo z repozitáře
        await _repo.AddPostAsync(currentUserId.Value, Topic, PostContent);

        return RedirectToPage("/ViewPosts");
    }
}
