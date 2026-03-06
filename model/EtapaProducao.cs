namespace api.Model
{  
    public class EtapaProducao
    {
        public Guid Id{get;set;}
        public string Nome_Etapa {get;set;}
        public string Descricao{get;set;}
        public int Ordem{get;set;}
        public ICollection<EtapaOP> EtapasOP {get;set;}

        public EtapaProducao(string nomeEtapa, string descricao, int ordem)
        {
            Id = Guid.NewGuid();
            Nome_Etapa = nomeEtapa;
            Descricao = descricao;
            Ordem = ordem;
        }
    }
}