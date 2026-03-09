namespace api.Model
{
    public class OrdemProducao
    {
        public int Id { get; set; }
        public int NumeroOP { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public string Status { get; set; } = string.Empty;

        public Produto Produto { get; set; } = null!;
        public ICollection<EtapaOP> EtapasOP { get; set; } = new List<EtapaOP>();

        public OrdemProducao(int id, int numeroOP, int produtoId, int quantidade, string status)
        {
            Id = id;
            NumeroOP = numeroOP;
            ProdutoId = produtoId;
            Quantidade = quantidade;
            Status = status;
        }
    }
}