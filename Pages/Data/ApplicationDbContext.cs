using hopefullyAWebForum.Pages.Models;
using Microsoft.EntityFrameworkCore; // Tohle chybělo!

namespace hopefullyAWebForum.Pages.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}

