using api.Data;
using api.Model;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/produto
        [HttpGet]
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

        // POST api/produto
        [HttpPost]
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

        // DELETE api/produto/{id}
        [HttpDelete("{id}")]
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

    // DTO para cadastro de produto + estoque juntos
    public class CadastroProdutoRequest
    {
        public Produto Produto { get; set; } = null!;
        public Estoque Estoque { get; set; } = null!;
    }
}
