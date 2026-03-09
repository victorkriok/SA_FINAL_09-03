public class LeituraIotDto
{
    public int id_leitura { get; set; }
    public int id_sensor { get; set; }
    public string tipo_leitura { get; set; }
    public decimal valor { get; set; }
    public string unidade { get; set; }
    public string descricao { get; set; }
    public DateTime data_leitura { get; set; }
}