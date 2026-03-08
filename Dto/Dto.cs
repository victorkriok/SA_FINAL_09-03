namespace api.Dto
{
    public record PerfilDTO(
        string Nome,
        string Cargo,
        string Descricao
    );

    public record UsuarioDTO(
        string Nome,
        string Login,
        string Senha,
        int PerfilId
    );

    public record LoginDTO(
        string Login,
        string Senha
    );
}