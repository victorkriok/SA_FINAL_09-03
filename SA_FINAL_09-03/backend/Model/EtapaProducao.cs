namespace api.Model
{
    public class EtapaProducao
    {
        public int Id { get; set; }
        public string NomeEtapa { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Ordem { get; set; }

        public ICollection<EtapaOP> EtapasOP { get; set; } = new List<EtapaOP>();

        public EtapaProducao(int id, string nomeEtapa, string descricao, int ordem)
        {
            Id = id;
            NomeEtapa = nomeEtapa;
            Descricao = descricao;
            Ordem = ordem;
        }
    }
}