namespace api.Model
{
    public class EtapaOP
    {
        public Guid Id{get;set;}
        public int OpId{get;set;}
        public int EtapaId{get;set;}
        public DateTime Data_Inicio{get;set;}
        public DateTime Data_Fim{get;set;}
        public string Status{get;set;}
        public OrdemProducao OrdemProducao{get;set;}

        public EtapaOP(int opId, int etapa_Id,DateTime data_inicio, DateTime data_fim, string status)
        {
            Id = Guid.NewGuid();
            Opid = opId;
            EtapaId = etapaId;
            Data_Inicio = data_inicio;
            Data_Fim = data_fim;
            Status = status;
        }
    }

}
