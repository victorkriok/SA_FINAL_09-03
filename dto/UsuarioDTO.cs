namespace api.dto;

public record UsuarioDTO(
    string nome,
    string login,
    string senha,
    Guid perfilId
);  

public record LoginDTO(
    string login,
    string senha
);