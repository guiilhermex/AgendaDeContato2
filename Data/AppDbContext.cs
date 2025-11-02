using AgendaContato.Data.Mappings;
using AgendaContato.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaContatos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Contato> Contatos { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<GrupoContato> GrupoContatos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GrupoMap());
            modelBuilder.ApplyConfiguration(new GrupoContatoMap());
            modelBuilder.ApplyConfiguration(new ContatoMap());
            modelBuilder.ApplyConfiguration(new UsuarioMap());
        }
    }
}