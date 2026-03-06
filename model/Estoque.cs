namespace api.Model
{
    public class Estoque
    {
        public Guid Id{get;set;}
        public int Codigo{get;set;}
        public string Categoria{get;set;}
        public int Quantidade{get;set;}
        public int EstoqueMinimo{get;set;}
        public string UnidadeMedida{get;set;}

        public Estoque(int codigo, string categoria, int quantidade, int estoqueMinimo, string unidadeMedida)
        {
            Id = Guid.NewGuid();
            Codigo = codigo;
            Categoria = categoria;
            Quantidade = quantidade;
            EstoqueMinimo = estoqueMinimo;
            UnidadeMedida = unidadeMedida;
        }
    }
}