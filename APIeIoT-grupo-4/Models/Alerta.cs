using System.ComponentModel.DataAnnotations;

namespace APIeloT_grupo_4.Models;

public class alerta
{
    [Key]
    public int id_alerta { get; set; }
    public int id_sensor { get; set; }
    public string tipo_alerta { get; set; }
    public decimal valor_detectado { get; set; }
    public decimal limite_configurado { get; set; }
    public string mensagem { get; set; }
    public DateTime data_alerta { get; set; }
}