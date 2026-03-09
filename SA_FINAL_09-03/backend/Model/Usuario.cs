namespace api.Model
{
    public class Usuario
    {
        public int      Id        { get; set; }
        public string   Nome      { get; set; } = string.Empty;
        public string   Login     { get; set; } = string.Empty;
        public string   SenhaHash { get; set; } = string.Empty;
        public string   Cargo     { get; set; } = string.Empty;
        public bool     Ativo     { get; set; }
        public DateTime CriadoEm  { get; set; }
        public int      PerfilId  { get; set; }
        public string   NomePerfil { get; set; } = string.Empty;

        // Navegação — necessário para o HasOne no ApiContext
        public Perfis? Perfil { get; set; }

        public Usuario() { }

        // Construtor com cargo (usado pelo AppDbContext ao fazer login)
        public Usuario(string nome, string login, string senhaHash, string cargo, int perfilId)
        {
            Nome      = nome;
            Login     = login;
            SenhaHash = senhaHash;
            PerfilId  = perfilId;
            Ativo     = true;
            CriadoEm  = DateTime.Now;
        }

        // Construtor sem cargo (usado pelo UsuarioRoutes ao registrar)
        public Usuario(string nome, string login, string senhaHash, int perfilId)
        {
            Nome      = nome;
            Login     = login;
            SenhaHash = senhaHash;
            PerfilId  = perfilId;
            Ativo     = true;
            CriadoEm  = DateTime.Now;
        }
    }
}