using api.Data;
using api.Model;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstoqueController(AppDbContext context)
        {
            _context = context;
        }

        // POST api/estoque/movimentacao
        // Registra entrada ou saída de um item
        [HttpPost("movimentacao")]
        public IActionResult RegistrarMovimentacao([FromBody] MovimentacaoEstoque movimentacao)
        {
            try
            {
                if (movimentacao.Tipo != "Entrada" && movimentacao.Tipo != "Saida")
                    return BadRequest(new { erro = "Tipo deve ser 'Entrada' ou 'Saida'." });

                if (movimentacao.Quantidade <= 0)
                    return BadRequest(new { erro = "Quantidade deve ser maior que zero." });

                if (string.IsNullOrWhiteSpace(movimentacao.Responsavel))
                    return BadRequest(new { erro = "Responsável é obrigatório." });

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

        // GET api/estoque/{estoqueId}/movimentacoes
        // Lista todas as movimentações de um item
        [HttpGet("{estoqueId}/movimentacoes")]
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
