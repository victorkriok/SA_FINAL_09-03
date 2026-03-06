namespace api.model;

public class Perfil
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }

    public Perfil(string nome, string descricao)
    {
        
        Id = Guid.NewGuid();
        Nome = nome;
        Descricao = descricao;
    }    
}