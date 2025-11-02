using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaContato.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Sistema");

            migrationBuilder.EnsureSchema(
                name: "Seguranca");

            migrationBuilder.CreateTable(
                name: "Usuario",
                schema: "Seguranca",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Nome = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    SenhaHash = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Contato",
                schema: "Sistema",
                columns: table => new
                {
                    IdContato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    NomeContato = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(80)", maxLength: 80, nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contato", x => x.IdContato);
                    table.ForeignKey(
                        name: "FK_Contato_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalSchema: "Seguranca",
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grupo",
                schema: "Sistema",
                columns: table => new
                {
                    IdGrupo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    NomeGrupo = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupo", x => x.IdGrupo);
                    table.ForeignKey(
                        name: "FK_Grupo_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "Seguranca",
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoContato",
                schema: "Sistema",
                columns: table => new
                {
                    IdGrupoContato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: false),
                    IdContato = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoContato", x => x.IdGrupoContato);
                    table.ForeignKey(
                        name: "FK_GrupoContato_Contato_IdContato",
                        column: x => x.IdContato,
                        principalSchema: "Sistema",
                        principalTable: "Contato",
                        principalColumn: "IdContato",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GrupoContato_Grupo_IdGrupo",
                        column: x => x.IdGrupo,
                        principalSchema: "Sistema",
                        principalTable: "Grupo",
                        principalColumn: "IdGrupo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contato_IdUsuario",
                schema: "Sistema",
                table: "Contato",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_UsuarioId",
                schema: "Sistema",
                table: "Grupo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoContato_IdContato",
                schema: "Sistema",
                table: "GrupoContato",
                column: "IdContato");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoContato_IdGrupo",
                schema: "Sistema",
                table: "GrupoContato",
                column: "IdGrupo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrupoContato",
                schema: "Sistema");

            migrationBuilder.DropTable(
                name: "Contato",
                schema: "Sistema");

            migrationBuilder.DropTable(
                name: "Grupo",
                schema: "Sistema");

            migrationBuilder.DropTable(
                name: "Usuario",
                schema: "Seguranca");
        }
    }
}
