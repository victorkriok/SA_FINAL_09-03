namespace api.Model
{
    public class MovimentacaoEstoque
    {
        public Guid Id{get;set;}
        public int IdItem {get;set;}
        public int Quantidade{get;set;}
        public DateTime Data_Hora{get;set;}
        public string Responsavel{get;set;}
        public string Observacao{get;set;}

        public Estoque ItemEstoque{get;set;}

        public MovimentacaoEstoque(int idItem, int quantidade, DateTime dataHora, string responsavel, string observacao)
        {
            Id = Guid.NewGuid();
            IdItem = idItem;
            Quantidade = quantidade;
            Data_Hora = dataHora;
            Responsavel = responsavel;
            Observacao = observacao;
        }
    }
}