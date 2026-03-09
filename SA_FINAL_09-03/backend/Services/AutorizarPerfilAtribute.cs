using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Services
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AutorizarPerfilAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _perfisPermitidos;

        public AutorizarPerfilAttribute(params string[] perfisPermitidos)
        {
            _perfisPermitidos = perfisPermitidos;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!(user.Identity?.IsAuthenticated ?? false))
            {
                context.Result = new UnauthorizedObjectResult(new { erro = "Token inválido ou não informado." });
                return;
            }

            // CORREÇÃO 5: a claim gravada pelo JwtService agora se chama "cargo"
            // e contém o NOME do perfil (ex: "Admin", "Almoxarife").
            // Antes estava lendo "cargo" mas o JWT só gravava "perfilId" (número),
            // então a comparação nunca funcionava → todos os endpoints retornavam 403.
            var cargo = user.FindFirst("cargo")?.Value;

            if (string.IsNullOrEmpty(cargo) || !_perfisPermitidos.Contains(cargo))
            {
                context.Result = new ObjectResult(new { erro = "Você não tem permissão para acessar este recurso." })
                {
                    StatusCode = 403
                };
            }
        }
    }
}