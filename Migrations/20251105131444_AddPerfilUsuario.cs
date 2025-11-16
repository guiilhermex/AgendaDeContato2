using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaContato.Migrations
{
    /// <inheritdoc />
    public partial class AddPerfilUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Perfil",
                schema: "Seguranca",
                table: "Usuario",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perfil",
                schema: "Seguranca",
                table: "Usuario");
        }
    }
}
