namespace api.Model
{
    public class Perfis
    {
        public Guid Id{get;set;}
        public string Nome {get;set;}
        public string Cargo{get;set;}

        public Perfis(string nome, string cargo)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Cargo = cargo;
        }
    }
}