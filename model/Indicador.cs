namespace api.Model
{
    public class Indicador
    {
        public Guid Id{get;set;}
        public int EquipamentoId{get;set;}
        public double Temperatura{get;set;}
        public double Vibracao{get;set;}
        public double HorasUso{get;set;}
        public DateTime DataRegistro{get;set;}

        public Indicador(int equipamentoId, double temperatura, double vibracao, double horasUso, DateTime dataRegistro)
        {
            Id = Guid.NewGuid();
            EquipamentoId = equipamentoId;
            Temperatura = temperatura;
            Vibracao = vibracao;
            HorasUso = horasUso;
            DataRegistro = dataRegistro;
        }
    }
}