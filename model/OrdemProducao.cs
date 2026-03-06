namespace api.Model
{
    public class OrdemProducao
    {
        public Guid Id{get;set;}
        public int NumeroOP{get;set;}
        public int ProdutoID{get;set;}
        public int Quantidade{get;set;}
        public string Status{get;set;}
        
        
        public OrdemProducao(int numeropOP, int produtoId, int quantidade, string status)
        {
            Id = Guid.NewGuid();
            NumeroOP = numeropOP;
            ProdutoID = produtoId;
            Quantidade = quantidade;
            Status = status;
        }
    }
}