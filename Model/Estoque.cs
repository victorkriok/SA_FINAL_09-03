namespace api.Model
{
    public class Estoque
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public int EstoqueMinimo { get; set; }
        public string UnidadeMedida { get; set; } = string.Empty;

        public Produto Produto { get; set; } = null!;
        public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();

        public Estoque(int id, int produtoId,int quantidade, int estoqueMinimo, string unidadeMedida)
        {
            Id = id;
            ProdutoId = produtoId;
            Quantidade = quantidade;
            EstoqueMinimo = estoqueMinimo;
            UnidadeMedida = unidadeMedida;
        }
    }
}