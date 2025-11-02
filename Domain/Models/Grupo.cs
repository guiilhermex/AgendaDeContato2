using AgendaContato.Domain.Models;

namespace AgendaContato.Domain.Models
{
    public class Grupo
    {
        public Guid TokenId { get; set; }
        public int IdGrupo { get; set; }
        public string NomeGrupo { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public ICollection<GrupoContato> GrupoContatos { get; set; }
    }
}