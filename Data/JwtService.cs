using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace toad.Service;

public class JwtService
{
    private const string SecretKey = "moje-super-tajne-dlouhe-heslo-pro-tokery-123456789"; 

    public string GenerateToken(int userId, string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username)
        };

        var token = new JwtSecurityToken(
            issuer: "toad-forum-api",
            audience: "toad-frontend",
            claims: claims,
            expires: DateTime.Now.AddDays(1), // Platnost tokenu 1 den
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}