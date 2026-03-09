using api.Data;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AuthController(AppDbContext db) => _db = db;

        // ── POST /api/auth/login ──────────────────────────────────
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Senha))
                    return BadRequest(new { erro = "Login e senha são obrigatórios." });

                var usuario = _db.BuscarUsuarioPorLogin(request.Login);

                if (usuario == null)
                    return Unauthorized(new { erro = "Usuário não encontrado." });

                if (!usuario.Ativo)
                    return Unauthorized(new { erro = "Usuário desativado." });

                if (!PasswordService.VerifyPassword(request.Senha, usuario.SenhaHash))
                    return Unauthorized(new { erro = "Senha inválida." });

                var token = JwtService.GenerateToken(usuario);

                return Ok(new
                {
                    token,
                    usuario = new
                    {
                        usuario.Id,
                        usuario.Nome,
                        usuario.Login,
                        usuario.PerfilId,
                        usuario.NomePerfil
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO LOGIN: " + ex.ToString());
                return StatusCode(500, new { erro = ex.Message, detalhe = ex.InnerException?.Message });
            }
        }

        // ── POST /api/auth/registrar ──────────────────────────────
        // Apenas Admin pode chamar (verificado pelo AutorizarPerfil)
        [HttpPost("registrar")]
        [AutorizarPerfil("Admin")]
        public IActionResult Registrar([FromBody] RegistrarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                return BadRequest(new { erro = "Nome é obrigatório." });

            if (string.IsNullOrWhiteSpace(request.Login))
                return BadRequest(new { erro = "Login é obrigatório." });

            if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 6)
                return BadRequest(new { erro = "Senha deve ter no mínimo 6 caracteres." });

            if (request.PerfilId <= 0)
                return BadRequest(new { erro = "Perfil é obrigatório." });

            try
            {
                // BCrypt gera o hash+salt internamente — não precisamos de salt separado
                var senhaHash = PasswordService.HashPassword(request.Senha);
                _db.CriarUsuario(request.Nome, request.Login, senhaHash, request.PerfilId);

                return Ok(new { mensagem = $"Usuário '{request.Login}' criado com sucesso." });
            }
            catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062)
            {
                return Conflict(new { erro = $"Login '{request.Login}' já está em uso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }

    public class LoginRequest
    {
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class RegistrarRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public int PerfilId { get; set; }
    }
}