using AgendaContato.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaContato.Data.Mappings
{
    public class ContatoMap : IEntityTypeConfiguration<Contato>
    {
        public void Configure(EntityTypeBuilder<Contato> builder)
        {
            builder.ToTable("Contato", schema: "Sistema");

            builder.HasKey(x => x.IdContato);

            builder.Property(x => x.IdContato)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            builder.Property(x => x.TokenId)
                .HasColumnType("uniqueidentifier")
                .HasDefaultValueSql("NEWID()");

            builder.Property(x => x.NomeContato)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);

            builder.HasIndex(x => x.NomeContato)
                .HasDatabaseName("IX_NomeContato");

            builder.Property(x => x.Telefone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(80);

            // Relacionamento com GrupoContato
            builder.HasMany(x => x.GrupoContatos)
                   .WithOne(gc => gc.Contato)
                   .HasForeignKey(gc => gc.IdContato);
        }
    }
}