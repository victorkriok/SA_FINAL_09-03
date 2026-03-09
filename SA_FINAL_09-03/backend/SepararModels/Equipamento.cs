namespace api.Model
{
    public class Equipamento
    {
        public Guid Id{get;set;}
        public string Nome{get;set;}
        public int Setor{get;set;}
        public string Status{get;set;}

        public Equipamento(string nome, int setor, string status)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Setor = setor;
            Status = status;
        }
    }
}