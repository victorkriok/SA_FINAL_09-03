namespace api.Model
{
    public class Perfis
    {
        public int    Id       { get; set; }
        public string Nome     { get; set; } = string.Empty;
        public string Cargo    { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool   Ativo    { get; set; }

        public Perfis() { }

        public Perfis(string nome, string cargo)
        {
            Nome  = nome;
            Cargo = cargo;
            Ativo = true;
        }
    }
}