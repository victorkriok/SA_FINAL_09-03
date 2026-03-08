namespace api.Model
{
    public class EtapaOP
    {
        public int Id{get;set;}
        public int OpId{get;set;}
        public int EtapaId{get;set;}
        public DateTime? Data_Inicio{get;set;}
        public DateTime? Data_Fim{get;set;}
        public string Status{get;set;} = "Pendente";

        public OrdemProducao OrdemProducao { get; set; } = null!;
        public EtapaProducao EtapaProducao { get; set; } = null!;

        public EtapaOP(int id, int opId, int etapaId)
        {
            Id = id;
            OpId = opId;
            EtapaId = etapaId;
            Status = "Pendente";
        }
    }
}