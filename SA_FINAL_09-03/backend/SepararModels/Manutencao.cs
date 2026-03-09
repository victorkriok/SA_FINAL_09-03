namespace api.Model
{
    public class Manutencao
    {
        public Guid Id{get;set;}
        public int EquipamentoId{get;set;}
        public int Tipo{get;set;}
        public string Descricao{get;set;}
        public DateTime DataManutencao{get;set;}
        public int ResponsavelId{get;set;}

        public Manutencao(int equipamentoId, int tipo, string descricao, DateTime dataManutencao, int responsavelId)
        {
            Id = Guid.NewGuid();
            EquipamentoId = equipamentoId;
            Tipo = tipo;
            Descricao = descricao;
            DataManutencao = dataManutencao;
            ResponsavelId = responsavelId;
        }
    }
}