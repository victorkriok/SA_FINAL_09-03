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
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        // Admin, SupervisorProducao e Almoxarife podem listar
        [HttpGet]
        [AutorizarPerfil("Admin", "SupervisorProducao", "Almoxarife")]
        public IActionResult ListarProdutos()
        {
            try
            {
                var produtos = _context.ListarProdutos();
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // Admin e Almoxarife podem cadastrar
        [HttpPost]
        [AutorizarPerfil("Admin", "Almoxarife")]
        public IActionResult CadastrarProduto([FromBody] CadastroProdutoRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Produto.Nome))
                    return BadRequest(new { erro = "Nome do produto é obrigatório." });

                _context.CadastroProduto(request.Produto, request.Estoque);
                return Created("", new { mensagem = "Produto cadastrado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // Apenas Admin pode deletar
        [HttpDelete("{id}")]
        [AutorizarPerfil("Admin")]
        public IActionResult DeletarProduto(int id)
        {
            try
            {
                _context.DeletarProduto(id);
                return Ok(new { mensagem = "Produto removido com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }

    public class CadastroProdutoRequest
    {
        public Produto Produto { get; set; } = null!;
        public Estoque Estoque { get; set; } = null!;
    }
}