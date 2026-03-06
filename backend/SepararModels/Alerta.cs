namespace api.Model
{
    public class Alerta
    {
        public Guid Id{get;set;}
        public int EquipamentoId{get;set;}
        public string Tipo{get;set;}
        public string Descricao{get;set;}
        public DateTime DataAlerta{get;set;}
    
        public Alerta(string nome, string login, int equipamentoId, string tipo, string descricao, DateTime dataAlerta)
        {
            Id = Guid.NewGuid();
            EquipamentoId = equipamentoId;
            Tipo = tipo;
            Descricao = descricao;
            DataAlerta = dataAlerta;
        }
    }
}