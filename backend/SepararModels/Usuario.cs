namespace api.Model
{
    public class Usuario
    {
        public Guid Id{get;set;}
        public string Nome{get;set;}
        public string Login{get;set;}
        public string SenhaHash{get;set;}
        public bool Ativo{get;set;}
        public DateTime CriadoEm{get;set;}
        public int PerfilId {get;set;}

        public Usuario(string nome, string login, string senhaHash, int perfilId)
        {
        Id = Guid.NewGuid();
        Nome = nome;
        Login = login;
        SenhaHash = senhaHash;
        PerfilId = perfilId;
        Ativo = true;
        CriadoEm = DateTime.Now;
        }
    }
}