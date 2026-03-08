using api.data;
using api.model;
using api.dto;
using Microsoft.EntityFrameworkCore;

namespace api.routes;

public static class PerfilRoute
{
    public static void PerfilRoutes(this WebApplication app)
    {
        app.MapPost("/perfis",
        async (ApiContext context, PerfilDTO req) =>
        {
            var perfil = new Perfil(
                req.nome,
                req.descricao
            );

            await context.Perfis.AddAsync(perfil);
            await context.SaveChangesAsync();

            return Results.Ok(perfil);
        });
    }
}