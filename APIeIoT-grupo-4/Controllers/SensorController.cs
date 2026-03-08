using Microsoft.AspNetCore.Mvc;
using APIeloT_grupo_4.Data;
using APIeloT_grupo_4.Models;

namespace APIeloT_grupo_4.Controllers;

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
        // salva leitura normal
        _context.Leituras.Add(dados);
        _context.SaveChanges();

        Console.WriteLine($"[ESP32] -> Sensor ID: {dados.id_sensor} | {dados.tipo_leitura}: {dados.valor} {dados.unidade} salvo!");

        // se passar desses valores, manda um alerta
        string tipoLimpo = (dados.tipo_leitura ?? "").Trim().ToUpper();

        if ((tipoLimpo.Contains("TEMP") && dados.valor > 35) ||
            (tipoLimpo.Contains("VIBRA") && dados.valor > 80))
        {
            var novoAlerta = new alerta
            {
                id_sensor = dados.id_sensor,
                tipo_alerta = "Crítico",
                valor_detectado = (decimal)dados.valor,
                // Define o limite baseado no tipo que disparou
                limite_configurado = tipoLimpo.Contains("TEMP") ? 35m : 80m,
                mensagem = $"Perigo! {dados.tipo_leitura} atingiu nível crítico: {dados.valor}",
                data_alerta = DateTime.Now
            };

            _context.alerta.Add(novoAlerta);
            _context.SaveChanges();

            // mostra no terminal o alerta
            Console.BackgroundColor = ConsoleColor.Red; // deixa o fundo vermelho
            Console.ForegroundColor = ConsoleColor.White;
            // aviadado pra ficar bonitinho
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine($"⚠️  ALERTA REGISTRADO NO BANCO PARA O SENSOR {dados.id_sensor}!");
            Console.WriteLine($"Valor: {dados.valor} | Tipo: {dados.tipo_leitura}");
            Console.WriteLine("---------------------------------------------------------");
            Console.ResetColor();
        }

        return Ok(new
        {
            mensagem = "Leitura salva com sucesso no banco!",
            id_gerado = dados.id_leitura
        });
    }
}