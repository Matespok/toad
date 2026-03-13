using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using hopefullyAWebForum.Pages.Models; 
using hopefullyAWebForum.Pages.Data;  
namespace hopefullyAWebForum.Pages;

public class Register : PageModel
{
    private readonly ApplicationDbContext _context;

    public Register(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Uname { get; set; } = string.Empty;
    
    [BindProperty]
    public string Pass { get; set; } = string.Empty;

    public void OnPost()
    {
        DbMethods.AddUser(Uname, Pass);
    }
}