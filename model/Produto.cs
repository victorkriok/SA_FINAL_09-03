namespace api.Model
{
    public class Produto
    {
        public Guid Id{get;set;}
        public string Nome{get;set;}
        public string Descricao{get;set;}
        public double Preco{get;set;}
        
        public Produto(string nome, string descricao, double preco)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
        }
        
    }
}