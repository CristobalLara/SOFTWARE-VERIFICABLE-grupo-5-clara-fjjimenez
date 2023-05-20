using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SII_App_Grupo_5.Migrations
{
    /// <inheritdoc />
    public partial class addComunaToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enajenantes_Inscripciones_InscripcionId",
                table: "Enajenantes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enajenantes",
                table: "Enajenantes");

            migrationBuilder.RenameTable(
                name: "Enajenantes",
                newName: "Enajenante");

            migrationBuilder.RenameIndex(
                name: "IX_Enajenantes_InscripcionId",
                table: "Enajenante",
                newName: "IX_Enajenante_InscripcionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enajenante",
                table: "Enajenante",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enajenante_Inscripciones_InscripcionId",
                table: "Enajenante",
                column: "InscripcionId",
                principalTable: "Inscripciones",
                principalColumn: "Folio",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enajenante_Inscripciones_InscripcionId",
                table: "Enajenante");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enajenante",
                table: "Enajenante");

            migrationBuilder.RenameTable(
                name: "Enajenante",
                newName: "Enajenantes");

            migrationBuilder.RenameIndex(
                name: "IX_Enajenante_InscripcionId",
                table: "Enajenantes",
                newName: "IX_Enajenantes_InscripcionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enajenantes",
                table: "Enajenantes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enajenantes_Inscripciones_InscripcionId",
                table: "Enajenantes",
                column: "InscripcionId",
                principalTable: "Inscripciones",
                principalColumn: "Folio",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
