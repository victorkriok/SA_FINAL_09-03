using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using api.model;

namespace api.security;

public class JwtService
{
    private const string KEY = "sua_chave_super_secreta_aqui_123456";

    public static string GenerateToken(Usuario user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim("userId", user.Id.ToString()),
            new Claim("perfilId", user.PerfilId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}