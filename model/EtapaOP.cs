namespace api.Model
{
    public class EtapaOP
    {
        public Guid Id{get;set;}
        public int Op_id{get;set;}
        public int Etapa_id{get;set;}
        public DateTime Data_Inicio{get;set;}
        public DateTime Data_Fim{get;set;}
        public string Status{get;set;}
        public OrdemProducao OrdemProducao{get;set;}

        public EtapaOP(int op_Id, int etapa_Id,DateTime data_inicio, DateTime data_fim, string status)
        {
            Id = Guid.NewGuid();
            Op_id = op_Id;
            Etapa_id = etapa_Id;
            Data_Inicio = data_inicio;
            Data_Fim = data_fim;
            Status = status;
        }
    }
}