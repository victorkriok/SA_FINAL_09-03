using Microsoft.EntityFrameworkCore;
using api.data;
using api.model;
using api.dto;
using BCrypt.Net;
using api.security;

namespace api.routes;

public static class UsuarioRoutes
{
    public static void UsuarioRoute(this WebApplication app)
    {
        app.MapPost("/usuarios",
        async (ApiContext context, UsuarioDTO req) =>
        {
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(req.senha);

            var user = new Usuario(
                req.nome,
                req.login,
                senhaHash,
                req.perfilId
            );

            await context.Usuarios.AddAsync(user);
            await context.SaveChangesAsync();

            return Results.Ok(user);
        });

        app.MapPost("/login",
        async (ApiContext context, LoginDTO req) =>
        {
            var user = await context.Usuarios
                .FirstOrDefaultAsync(u => u.Login == req.login);

            if (user == null)
                return Results.Ok("User incorreto");

            var senhaValida = BCrypt.Net.BCrypt.Verify(req.senha, user.SenhaHash);

            if (!senhaValida)
                return Results.Ok("User incorreto");

            var token = JwtService.GenerateToken(user);

            return Results.Ok(new
            {
                message = "User liberado",
                token = token
            });
        });
    }
}