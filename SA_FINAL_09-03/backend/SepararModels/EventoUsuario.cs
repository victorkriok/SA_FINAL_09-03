namespace api.Model
{
    public class EventoUsuario
    {
        public Guid Id {get;set;}
        public int UsuarioId{get;set;}
        public string TipoEvento{get;set;}
        public string Descricao{get;set;}
        public string Ip{get;set;}

        public EventoUsuario(int usuarioId, string tipoEvento, string descricao, string ip)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            TipoEvento = tipoEvento;
            Descricao = descricao;
            Ip = ip;
        }
    }
}