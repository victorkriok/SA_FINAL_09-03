using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace SistemaManutencao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManutencaoController : ControllerBase
    {
        private static List<Alerta> _tabelaAlertas = new List<Alerta>();

        // 1. ENDPOINT PARA RECEBER DADOS DO SENSOR (O POST)
        [HttpPost("RegistrarLeitura")]
        public IActionResult RegistrarLeitura([FromBody] Indicador novoIndicador)
        {
            bool gerouAlerta = false;

            // Regras da Equipe 2
            if (novoIndicador.Temperatura > 80)
            {
                CriarAlerta(novoIndicador.EquipamentoId, "Temperatura Alta", $"Temperatura: {novoIndicador.Temperatura}°C");
                gerouAlerta = true;
            }

            if (novoIndicador.Vibracao > 2.0m)
            {
                CriarAlerta(novoIndicador.EquipamentoId, "Vibração Alta", $"Vibração: {novoIndicador.Vibracao}");
                gerouAlerta = true;
            }

            if (novoIndicador.HorasUso > 500)
            {
                CriarAlerta(novoIndicador.EquipamentoId, "Manutenção Preventiva", $"Horas de uso: {novoIndicador.HorasUso}");
                gerouAlerta = true;
            }

            return Ok(new { Sucesso = true, AlertaGerado = gerouAlerta, Mensagem = "Leitura registrada com sucesso." });
        }

        // 2. ENDPOINT PARA O SEU FRONT-END BUSCAR OS ALERTAS (O GET)
        [HttpGet("Alertas")]
        public IActionResult GetAlertas()
        {
            // Retorna a lista de alertas para o seu HTML consumir
            return Ok(_tabelaAlertas);
        }

        // Método interno de apoio
        private void CriarAlerta(int equipamentoId, string tipo, string descricao)
        {
            var alerta = new Alerta
            {
                EquipamentoId = equipamentoId,
                TipoAlerta = tipo,
                Descricao = descricao,
                DataAlerta = DateTime.Now
            };
            _tabelaAlertas.Add(alerta);
        }
    }

    // Classes de apoio que a Equipe 2 fez
    public class Alerta
    {
        public int EquipamentoId { get; set; }
        public string TipoAlerta { get; set; }
        public string Descricao { get; set; }
        public DateTime DataAlerta { get; set; }
    }

    public class Indicador
    {
        public int EquipamentoId { get; set; }
        public int HorasUso { get; set; }
        public decimal Temperatura { get; set; }
        public decimal Vibracao { get; set; }
    }
}