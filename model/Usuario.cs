namespace api.model;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Login { get; set; }
    public string SenhaHash { get; set; }

    public Guid PerfilId { get; set; }
    public Perfil Perfil { get; set; }

    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }

    public Usuario(string nome, string login, string senhaHash, Guid perfilId)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Login = login;
        SenhaHash = senhaHash;
        PerfilId = perfilId;
        Ativo = true;
        CriadoEm = DateTime.UtcNow;
    }
}