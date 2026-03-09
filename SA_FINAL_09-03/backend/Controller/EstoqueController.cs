using api.Data;
using api.Model;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EstoqueController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstoqueController(AppDbContext context)
        {
            _context = context;
        }

        // Admin e Almoxarife podem registrar movimentação
        [HttpPost("movimentacao")]
        [AutorizarPerfil("Admin", "Almoxarife")]
        public IActionResult RegistrarMovimentacao([FromBody] MovimentacaoEstoque movimentacao)
        {
            try
            {
                if (movimentacao.Tipo != "Entrada" && movimentacao.Tipo != "Saida")
                    return BadRequest(new { erro = "Tipo deve ser 'Entrada' ou 'Saida'." });

                if (movimentacao.Quantidade <= 0)
                    return BadRequest(new { erro = "Quantidade deve ser maior que zero." });

                _context.AdicionarMovimentacao(movimentacao);
                return Ok(new { mensagem = $"{movimentacao.Tipo} de {movimentacao.Quantidade} registrada com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(new { erro = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("insuficiente"))
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // Admin, SupervisorProducao e Almoxarife podem listar movimentações
        [HttpGet("{estoqueId}/movimentacoes")]
        [AutorizarPerfil("Admin", "SupervisorProducao", "Almoxarife")]
        public IActionResult ListarMovimentacoes(int estoqueId)
        {
            try
            {
                var movimentacoes = _context.ListarMovimentacoes(estoqueId);
                return Ok(movimentacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }
}