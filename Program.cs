using System.Text;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Potřebné pro JWT
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using toad.Data;
using toad.Endpoints;
using toad.Repositories;
using toad.Service;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRACE SLUŽEB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);
builder.Services.AddScoped<IForumRepository, ForumRepository>();

// JWT Autentizace
var envVars = DotEnv.Read();
if (!envVars.TryGetValue("JWTKEY", out var jwtKey))
{
    throw new InvalidOperationException("JWTKEY is missing from the environment variables.");
}

builder.Services.AddSingleton<JwtService>();
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inicializace DB
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// 2. MIDDLEWARE PIPELINE
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowAll"); // CORS politika musí být zde
app.UseAuthentication(); // Autentizace musí být před autorizací
app.UseAuthorization();
app.UseSession();

// 3. MAPOVÁNÍ ENDPOINTŮ
app.MapRazorPages();

app.MapCommentEndpoints();
app.MapUserEndpoints();
app.MapPostEndpoints();

app.Run();
