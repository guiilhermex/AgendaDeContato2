using AgendaContato.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaContato.Data.Mappings
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario", schema: "Seguranca");

            builder.HasKey(x => x.IdUsuario);

            builder.Property(x => x.IdUsuario)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(x => x.TokenId)
                .HasColumnType("uniqueidentifier")
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            builder.HasIndex(x => x.Email).HasDatabaseName("IX_Usuario_Email");

            builder.Property(x => x.SenhaHash)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(255);

            builder.Property(x => x.CriadoEm)
                .HasColumnType("DATETIME")
                .HasDefaultValueSql("GETDATE()");


            builder.HasMany(x => x.Contatos)
                   .WithOne(c => c.Usuario)
                   .HasForeignKey(c => c.IdUsuario)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Grupos)
                   .WithOne(g => g.Usuario)
                   .HasForeignKey(g => g.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
