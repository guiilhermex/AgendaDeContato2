using AgendaContato.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AgendaContato.Domain.Models
{
    public class Usuario
    {
        public Guid TokenId { get; set; }
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public PerfilUsuario Perfil { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public ICollection<Contato> Contatos { get; set; } = new List<Contato>();
        public ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();
        public List<UsuarioRole> Roles { get; set; } = new();

        public class UsuarioRole
        {
            public int Id { get; set; }
            public string Slug { get; set; } = string.Empty; // Ex: "Admin", "User"
        }
    }
}
