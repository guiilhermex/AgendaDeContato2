using AgendaContato.Domain.Models;

namespace AgendaContato.Domain.Models
{
    public class Contato
    {
        public Guid TokenId { get; set; }
        public int IdContato { get; set; }
        public string NomeContato { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<GrupoContato> GrupoContatos { get; set; }
    }
}