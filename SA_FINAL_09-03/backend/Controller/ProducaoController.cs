using api.Repository;
using api.Model;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProducaoController : ControllerBase
    {
        private readonly ProducaoRepository _producao;

        public ProducaoController(ProducaoRepository producao)
        {
            _producao = producao;
        }

        // ==================== ETAPAS DE PRODUCAO ====================

        [HttpGet("etapas")]
        [AutorizarPerfil("Admin", "SupervisorProducao")]
        public IActionResult ListarEtapas()
        {
            try { return Ok(_producao.ListarEtapasProducao()); }
            catch (Exception ex) { return StatusCode(500, new { erro = ex.Message }); }
        }

        [HttpPost("etapas")]
        [AutorizarPerfil("Admin", "SupervisorProducao")]
        public IActionResult CadastrarEtapa([FromBody] EtapaProducao etapa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(etapa.NomeEtapa))
                    return BadRequest(new { erro = "Nome da etapa é obrigatório." });
                if (etapa.Ordem <= 0)
                    return BadRequest(new { erro = "Ordem deve ser maior que zero." });

                _producao.CadastrarEtapaProducao(etapa);
                return Created("", new { mensagem = "Etapa cadastrada com sucesso." });
            }
            catch (Exception ex) { return StatusCode(500, new { erro = ex.Message }); }
        }

        [HttpDelete("etapas/{id}")]
        [AutorizarPerfil("Admin")]
        public IActionResult DeletarEtapa(int id)
        {
            try
            {
                _producao.DeletarEtapaProducao(id);
                return Ok(new { mensagem = "Etapa removida com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrada"))
            { return NotFound(new { erro = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { erro = ex.Message }); }
        }

        // ==================== ORDENS DE PRODUCAO ====================

        [HttpGet("ordens")]
        [AutorizarPerfil("Admin", "SupervisorProducao")]
        public IActionResult ListarOrdens()
        {
            try { return Ok(_producao.ListarOrdensProducao()); }
            catch (Exception ex) { return StatusCode(500, new { erro = ex.Message }); }
        }

        [HttpPost("ordens")]
        [AutorizarPerfil("Admin", "SupervisorProducao")]
        public IActionResult CriarOrdem([FromBody] OrdemProducao op)
        {
            try
            {
                if (op.NumeroOP <= 0) return BadRequest(new { erro = "Número da OP é obrigatório." });
                if (op.ProdutoId <= 0) return BadRequest(new { erro = "Produto é obrigatório." });
                if (op.Quantidade <= 0) return BadRequest(new { erro = "Quantidade deve ser maior que zero." });

                _producao.CriarOrdemProducao(op);
                return Created("", new { mensagem = $"OP #{op.NumeroOP} criada com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("Nenhuma EtapaProducao"))
            { return BadRequest(new { erro = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { erro = ex.Message }); }
        }

        [HttpGet("ordens/{opId}/etapas")]
        [AutorizarPerfil("Admin", "SupervisorProducao")]
        public IActionResult ListarEtapasDaOP(int opId)
        {
            try { return Ok(_producao.ListarEtapasDaOP(opId)); }
            catch (Exception ex) { return StatusCode(500, new { erro = ex.Message }); }
        }

        // ==================== ETAPAS OP ====================

        [HttpPatch("etapas-op/{id}/iniciar")]
        [AutorizarPerfil("Admin", "SupervisorProducao")]
        public IActionResult IniciarEtapa(int id)
        {
            try
            {
                _producao.IniciarEtapa(id);
                return Ok(new { mensagem = "Etapa iniciada com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrada") || ex.Message.Contains("já foi iniciada"))
            { return BadRequest(new { erro = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { erro = ex.Message }); }
        }

        [HttpPatch("etapas-op/{id}/concluir")]
        [AutorizarPerfil("Admin", "SupervisorProducao")]
        public IActionResult ConcluirEtapa(int id)
        {
            try
            {
                _producao.ConcluirEtapa(id);
                return Ok(new { mensagem = "Etapa concluída com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrada") || ex.Message.Contains("não está em andamento"))
            { return BadRequest(new { erro = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { erro = ex.Message }); }
        }
    }
}