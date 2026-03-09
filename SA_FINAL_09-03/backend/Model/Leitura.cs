using System;

namespace api.Model
{
    public class Leitura
    {
        public int id_leitura { get; set; }
        public int id_sensor { get; set; }
        public string tipo_leitura { get; set; } = string.Empty;
        public float valor { get; set; }
        public string unidade { get; set; } = string.Empty;
        public string descricao { get; set; } = string.Empty;
        public DateTime data_leitura { get; set; }
    }
}