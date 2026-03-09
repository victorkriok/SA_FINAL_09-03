using System.IdentityModel.Tokens.Jwt;
using api.Model;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class JwtService
    {
        // CORREÇÃO 1: chave com no mínimo 32 caracteres (256 bits) para HmacSha256
        private const string KEY = "sua_chave_super_secreta_aqui_12345678";

        public static string GenerateToken(Usuario user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim("userId",   user.Id.ToString()),
                new Claim("perfilId", user.PerfilId.ToString()),

                // CORREÇÃO 2: AutorizarPerfilAttribute lê a claim "cargo".
                // Aqui gravamos o NOME do perfil para que a comparação
                // "Admin", "Almoxarife", etc. funcione corretamente.
                // O nome do perfil precisa vir junto ao usuário — adicionamos
                // NomePerfil na model Usuario e preenchemos no login.
                new Claim("cargo", user.NomePerfil ?? string.Empty)
            };

            var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims:            claims,
                expires:           DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}