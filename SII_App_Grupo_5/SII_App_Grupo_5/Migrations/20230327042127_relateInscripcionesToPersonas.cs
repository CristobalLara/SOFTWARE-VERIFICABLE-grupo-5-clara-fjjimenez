using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SII_App_Grupo_5.Migrations
{
    /// <inheritdoc />
    public partial class relateInscripcionesToPersonas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Personas",
                table: "Personas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inscripciones",
                table: "Inscripciones");

            migrationBuilder.RenameTable(
                name: "Inscripciones",
                newName: "Inscripcion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Personas",
                table: "Personas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inscripcion",
                table: "Inscripcion",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InscripcionPersona",
                columns: table => new
                {
                    InscripcionesId = table.Column<int>(type: "int", nullable: false),
                    PersonasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscripcionPersona", x => new { x.InscripcionesId, x.PersonasId });
                    table.ForeignKey(
                        name: "FK_InscripcionPersona_Inscripcion_InscripcionesId",
                        column: x => x.InscripcionesId,
                        principalTable: "Inscripcion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InscripcionPersona_Personas_PersonasId",
                        column: x => x.PersonasId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InscripcionPersona_PersonasId",
                table: "InscripcionPersona",
                column: "PersonasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InscripcionPersona");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Personas",
                table: "Personas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inscripcion",
                table: "Inscripcion");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Inscripcion");

            migrationBuilder.RenameTable(
                name: "Inscripcion",
                newName: "Inscripciones");

            migrationBuilder.AlterColumn<int>(
                name: "RUN",
                table: "Personas",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Folio",
                table: "Inscripciones",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Personas",
                table: "Personas",
                column: "RUN");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inscripciones",
                table: "Inscripciones",
                column: "Folio");
        }
    }
}
