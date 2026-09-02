using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotenv.net;
using Microsoft.IdentityModel.Tokens;

namespace toad.Service;

public class JwtService
{
    private readonly string _secretKey;

    public JwtService()
    {
        var envVars = DotEnv.Read();
        if (envVars.TryGetValue("JWTKEY", out var jwtKey))
        {
            _secretKey = jwtKey;
        }
        else
        {
            throw new InvalidOperationException(
                "JWTKEY is missing from the environment variables."
            );
        }
    }

    public string GenerateToken(int userId, string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
        };

        var token = new JwtSecurityToken(
            issuer: "toad-forum-api",
            audience: "toad-frontend",
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
