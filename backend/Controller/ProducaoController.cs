using api.Data;
using api.Model;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProducaoController : ControllerBase
    {
        private readonly ProducaoRepository _producao;

        public ProducaoController(ProducaoRepository producao)
        {
            _producao = producao;
        }

        // ==================== ETAPAS DE PRODUCAO ====================

        // GET api/producao/etapas
        [HttpGet("etapas")]
        public IActionResult ListarEtapas()
        {
            try
            {
                var etapas = _producao.ListarEtapasProducao();
                return Ok(etapas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // POST api/producao/etapas
        [HttpPost("etapas")]
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
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // DELETE api/producao/etapas/{id}
        [HttpDelete("etapas/{id}")]
        public IActionResult DeletarEtapa(int id)
        {
            try
            {
                _producao.DeletarEtapaProducao(id);
                return Ok(new { mensagem = "Etapa removida com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrada"))
            {
                return NotFound(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [HttpGet("ordens")]
        public IActionResult ListarOrdens()
        {
            try
            {
                var ordens = _producao.ListarOrdensProducao();
                return Ok(ordens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [HttpPost("ordens")]
        public IActionResult CriarOrdem([FromBody] OrdemProducao op)
        {
            try
            {
                if (op.NumeroOP <= 0)
                    return BadRequest(new { erro = "Número da OP é obrigatório." });

                if (op.ProdutoId <= 0)
                    return BadRequest(new { erro = "Produto é obrigatório." });

                if (op.Quantidade <= 0)
                    return BadRequest(new { erro = "Quantidade deve ser maior que zero." });

                _producao.CriarOrdemProducao(op);
                return Created("", new { mensagem = $"OP #{op.NumeroOP} criada com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("Nenhuma EtapaProducao"))
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [HttpGet("ordens/{opId}/etapas")]
        public IActionResult ListarEtapasDaOP(int opId)
        {
            try
            {
                var etapas = _producao.ListarEtapasDaOP(opId);
                return Ok(etapas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // PATCH api/producao/etapas-op/{id}/iniciar
        [HttpPatch("etapas-op/{id}/iniciar")]
        public IActionResult IniciarEtapa(int id)
        {
            try
            {
                _producao.IniciarEtapa(id);
                return Ok(new { mensagem = "Etapa iniciada com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrada") || ex.Message.Contains("já foi iniciada"))
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        // PATCH api/producao/etapas-op/{id}/concluir
        [HttpPatch("etapas-op/{id}/concluir")]
        public IActionResult ConcluirEtapa(int id)
        {
            try
            {
                _producao.ConcluirEtapa(id);
                return Ok(new { mensagem = "Etapa concluída com sucesso." });
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrada") || ex.Message.Contains("não está em andamento"))
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }
    }
}
