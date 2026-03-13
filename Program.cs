using hopefullyAWebForum.Pages.Data;
using hopefullyAWebForum.Pages.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection string a DB Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// 2. KONFIGURACE COOKIES (Tohle řeší tvé podezření s GDPR)
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false; // Session bude fungovat hned
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// 3. REGISTRACE SESSION SLUŽEB (Tohle ti tam chybělo!)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Označíme cookie jako nezbytnou
});

builder.Services.AddRazorPages();

// Inicializace tvých metod
DbMethods.SetConnectionString(connectionString);
DbMethods.InitializeDb();

var app = builder.Build();

// --- MIDDLEWARE PIPELINE ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Pořadí je svaté: Nejdřív Routing, pak Session, pak Authorization
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

