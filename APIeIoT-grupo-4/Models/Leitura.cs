using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIeloT_grupo_4.Models;

[Table("leituras")]
public class Leitura
{
    [Key]
    [Column("id_leitura")]
    public int id_leitura { get; set; }

    [Column("id_sensor")]
    public int id_sensor { get; set; }

    [Column("tipo_leitura")]
    public string tipo_leitura { get; set; }

    [Column("valor")]
    public decimal valor { get; set; }

    [Column("unidade")]
    public string unidade { get; set; }

    [Column("descricao")]
    public string? descricao { get; set; }

    [Column("data_leitura")]
    public DateTime data_leitura { get; set; } = DateTime.Now;
}