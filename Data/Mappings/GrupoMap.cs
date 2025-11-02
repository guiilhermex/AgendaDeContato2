using AgendaContato.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaContato.Data.Mappings
{
    public class GrupoMap : IEntityTypeConfiguration<Grupo>
    {
        public void Configure(EntityTypeBuilder<Grupo> builder)
        {
            builder.ToTable("Grupo", schema: "Sistema");

            builder.HasKey(x => x.IdGrupo);

            builder.Property(x => x.IdGrupo)
                   .ValueGeneratedOnAdd()
                   .UseIdentityColumn();

            builder.Property(x => x.TokenId)
                   .HasColumnType("uniqueidentifier")
                   .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.NomeGrupo)
                   .IsRequired()
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(50);

            builder.HasIndex(x => x.NomeGrupo)
                .HasDatabaseName("IX_NomeGrupo");

            builder.HasMany(x => x.GrupoContatos)
                   .WithOne(gc => gc.Grupo)
                   .HasForeignKey(gc => gc.IdGrupo);
        }
    }
}