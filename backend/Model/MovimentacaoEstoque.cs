namespace api.Model
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public int EstoqueId { get; set; } 
        public int UsuarioId { get; set; }
        public int Quantidade { get; set; }
        public string Tipo { get; set; } = string.Empty; 
        public DateTime DataMovimento { get; set; }
        public string Responsavel { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;

        public Estoque ItemEstoque { get; set; } = null!;

        public MovimentacaoEstoque(int id, int estoqueId, int usuarioId, int quantidade, DateTime dataMovimento, string tipo, string responsavel, string observacao)
        {
            Id = id;
            EstoqueId = estoqueId;
            UsuarioId = usuarioId;
            Quantidade = quantidade;
            DataMovimento = dataMovimento;
            Tipo = tipo;
            Responsavel = responsavel;
            Observacao = observacao;
        }
    }
}