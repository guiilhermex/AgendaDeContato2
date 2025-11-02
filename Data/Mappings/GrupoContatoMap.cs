using AgendaContato.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaContato.Data.Mappings
{
    public class GrupoContatoMap : IEntityTypeConfiguration<GrupoContato>
    {
        public void Configure(EntityTypeBuilder<GrupoContato> builder)
        {
            builder.ToTable("GrupoContato", schema: "Sistema");

            builder.HasKey(gc => gc.IdGrupoContato);

            builder.Property(gc => gc.TokenId)
                .HasColumnType("uniqueidentifier")
                .HasDefaultValueSql("NEWID()");

            builder.HasOne(gc => gc.Grupo)
                .WithMany(g => g.GrupoContatos)
                .HasForeignKey(gc => gc.IdGrupo)
                .OnDelete(DeleteBehavior.Restrict); // <- muda de Cascade para Restrict

            builder.HasOne(gc => gc.Contato)
                   .WithMany(c => c.GrupoContatos)
                   .HasForeignKey(gc => gc.IdContato)
                   .OnDelete(DeleteBehavior.Restrict); // <- muda de Cascade para Restrict

        }
    }
}