namespace api.model;

public class Perfil
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }

    public Perfil(Guid id, string nome, string descricao)
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
    }    
}