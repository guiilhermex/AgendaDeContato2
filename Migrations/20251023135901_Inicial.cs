using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaContato.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                schema: "Seguranca",
                table: "Usuario",
                type: "DATETIME",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<Guid>(
                name: "TokenId",
                schema: "Sistema",
                table: "GrupoContato",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "UsuarioRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioRole_Usuario_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalSchema: "Seguranca",
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                schema: "Seguranca",
                table: "Usuario",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_NomeGrupo",
                schema: "Sistema",
                table: "Grupo",
                column: "NomeGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_NomeContato",
                schema: "Sistema",
                table: "Contato",
                column: "NomeContato");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRole_UsuarioIdUsuario",
                table: "UsuarioRole",
                column: "UsuarioIdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioRole");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_Email",
                schema: "Seguranca",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_NomeGrupo",
                schema: "Sistema",
                table: "Grupo");

            migrationBuilder.DropIndex(
                name: "IX_NomeContato",
                schema: "Sistema",
                table: "Contato");

            migrationBuilder.DropColumn(
                name: "CriadoEm",
                schema: "Seguranca",
                table: "Usuario");

            migrationBuilder.AlterColumn<Guid>(
                name: "TokenId",
                schema: "Sistema",
                table: "GrupoContato",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWID()");
        }
    }
}
