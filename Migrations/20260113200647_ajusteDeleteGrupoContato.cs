using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaContato.Migrations
{
    /// <inheritdoc />
    public partial class ajusteDeleteGrupoContato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrupoContato_Contato_IdContato",
                schema: "Sistema",
                table: "GrupoContato");

            migrationBuilder.DropForeignKey(
                name: "FK_GrupoContato_Grupo_IdGrupo",
                schema: "Sistema",
                table: "GrupoContato");

            migrationBuilder.AddForeignKey(
                name: "FK_GrupoContato_Contato_IdContato",
                schema: "Sistema",
                table: "GrupoContato",
                column: "IdContato",
                principalSchema: "Sistema",
                principalTable: "Contato",
                principalColumn: "IdContato");

            migrationBuilder.AddForeignKey(
                name: "FK_GrupoContato_Grupo_IdGrupo",
                schema: "Sistema",
                table: "GrupoContato",
                column: "IdGrupo",
                principalSchema: "Sistema",
                principalTable: "Grupo",
                principalColumn: "IdGrupo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrupoContato_Contato_IdContato",
                schema: "Sistema",
                table: "GrupoContato");

            migrationBuilder.DropForeignKey(
                name: "FK_GrupoContato_Grupo_IdGrupo",
                schema: "Sistema",
                table: "GrupoContato");

            migrationBuilder.AddForeignKey(
                name: "FK_GrupoContato_Contato_IdContato",
                schema: "Sistema",
                table: "GrupoContato",
                column: "IdContato",
                principalSchema: "Sistema",
                principalTable: "Contato",
                principalColumn: "IdContato",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GrupoContato_Grupo_IdGrupo",
                schema: "Sistema",
                table: "GrupoContato",
                column: "IdGrupo",
                principalSchema: "Sistema",
                principalTable: "Grupo",
                principalColumn: "IdGrupo",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
