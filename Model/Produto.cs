namespace api.Model
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public double Preco { get; set; }

        public Estoque Estoque { get; set; } = null!;
        public ICollection<OrdemProducao> OrdensProducao { get; set; } = new List<OrdemProducao>();

        public Produto(string nome, string descricao, double preco)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
        }
    }
}