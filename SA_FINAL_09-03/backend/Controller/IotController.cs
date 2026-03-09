using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Services; // Puxando a pasta onde criamos o IotApiService
using System;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class IotController : ControllerBase
    {
        private readonly IotApiService _iotApiService;

        // O C# injeta o nosso "telefone" aqui
        public IotController(IotApiService iotApiService)
        {
            _iotApiService = iotApiService;
        }

        // A rota que a sua tela (Frontend) vai chamar
        [HttpGet("painel")]
        // [AutorizarPerfil("Admin", "SupervisorProducao")] // Se quiser restringir por perfil igual fez na Producao
        public async Task<IActionResult> ObterDadosIot()
        {
            try 
            {
                // Pede pro serviço buscar as leituras lá no DevTunnels
                var leituras = await _iotApiService.BuscarLeituras();

                if (leituras == null || leituras.Count == 0)
                {
                    return Ok(new { mensagem = "Nenhuma leitura recebida dos sensores ainda." });
                }

                return Ok(leituras);
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { erro = "Falha ao conectar com a API dos sensores: " + ex.Message }); 
            }
        }
    }
}