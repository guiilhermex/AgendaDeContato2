using AgendaContato.Domain.Models;

namespace AgendaContato.Domain.Models
{
    public class GrupoContato
    {
        public Guid TokenId { get; set; }
        public int IdGrupoContato { get; set; }

        // Chaves estrangeiras
        public int IdGrupo { get; set; }
        public Grupo Grupo { get; set; }

        public int IdContato { get; set; }
        public Contato Contato { get; set; }
    }
}