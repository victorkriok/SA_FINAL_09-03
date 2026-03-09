using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIeloT_grupo_4.Models;

[Table("Indicador")]
public class Indicador
{
    [Key]
    public int id { get; set; }

    public int equipamentoId { get; set; }
    public double? temperatura { get; set; }
    public double? vibracao { get; set; }
    public int? horasUso { get; set; }

    public DateTime dataRegistro { get; set; } = DateTime.Now;
}