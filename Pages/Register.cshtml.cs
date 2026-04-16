using hopefullyAWebForum.Pages.Data;
using hopefullyAWebForum.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace hopefullyAWebForum.Pages;

public class Register : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly DbMethods _db = new();

    public void OnGet() { }

    public Register(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Uname { get; set; } = string.Empty;

    [BindProperty]
    public string Pass { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        await _db.AddUserAsync(Uname, Pass);

        return RedirectToPage("/Login");
    }
}
