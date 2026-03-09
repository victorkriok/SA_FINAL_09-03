using Microsoft.AspNetCore.Mvc;
using api.Data;  
using api.Model; 
using System;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorController : ControllerBase
{
    private readonly AppDbContext _context;

    public SensorController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult ReceberDados([FromBody] Leitura dados)
    {
        // IMPORTANTE: Ignora a página de aviso do DevTunnel para o ESP32 conseguir conectar
        Response.Headers.Add("X-Tunnel-Skip-Anti-Phishing-Page", "true");

        try 
        {
            // Validação simples
            if (dados == null) return BadRequest("Dados inválidos.");

            string tipoLimpo = (dados.tipo_leitura ?? "").Trim().ToUpper();
            bool isAlerta = false;
            float limite = 0f;

            // 1. O GUARDA DE TRÂNSITO: Analisa quem está chegando
            if (tipoLimpo.Contains("TEMP") && dados.valor > 35)
            {
                isAlerta = true;
                limite = 35.0f; // Limite da temperatura
            }
            else if (tipoLimpo.Contains("VIBRA") && dados.valor > 80)
            {
                isAlerta = true;
                limite = 80.0f; // Limite da vibração
            }

            // 2. TOMA A DECISÃO: Para qual tabela enviar?
            if (isAlerta)
            {
                // --- CAMINHO 1: DEU RUIM (Vai pra tabela Alerta) ---

                // Mostra no terminal o alerta em destaque
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"⚠️  NÍVEL CRÍTICO DETECTADO NO SENSOR {dados.id_sensor}!");
                Console.WriteLine($"Valor: {dados.valor} | Tipo: {dados.tipo_leitura}");
                Console.WriteLine("---------------------------------------------------------");
                Console.ResetColor();

                // Monta a mensagem bonitinha
                string msgAlerta = $"Perigo! {dados.tipo_leitura} atingiu nível crítico: {dados.valor}";

                // Manda pro método NOVO do AppDbContext!
                _context.SalvarAlerta(dados.id_sensor, "Crítico", dados.valor, limite, msgAlerta);

                Console.WriteLine("[DB] Alerta salvo EXCLUSIVAMENTE na tabela de Alertas!");

                return Ok(new { mensagem = "Alerta registrado com sucesso!", id_sensor = dados.id_sensor });
            }
            else
            {
                // --- CAMINHO 2: TUDO TRANQUILO (Vai pra tabela Leituras) ---
                
                _context.SalvarLeitura(dados);
                
                Console.WriteLine($"[ESP32] -> Sensor ID: {dados.id_sensor} | {dados.tipo_leitura}: {dados.valor} {dados.unidade} salvo na tabela Leituras!");
                
                return Ok(new { mensagem = "Leitura salva com sucesso no banco!", id_sensor = dados.id_sensor });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] -> {ex.Message}");
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult Listar()
    {
        try 
        {
            var leituras = _context.ListarLeituras();
            return Ok(leituras);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpGet("alertas")]
    public IActionResult ListarAlertas()
    {
        try 
        {
            var alertas = _context.ListarAlertas();
            return Ok(alertas);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}