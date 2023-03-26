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
                name: "PK_Inscripciones",
                table: "Inscripciones");

            migrationBuilder.RenameTable(
                name: "Inscripciones",
                newName: "Inscripcion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inscripcion",
                table: "Inscripcion",
                column: "Folio");

            migrationBuilder.CreateTable(
                name: "InscripcionPersona",
                columns: table => new
                {
                    InscripcionesFolio = table.Column<int>(type: "int", nullable: false),
                    PersonasRUN = table.Column<int>(type: "int", nullable: false),
                    PorcentajeDerecho = table.Column<float>(type: "float", nullable: false),
                    Acreditado = table.Column<bool>(type: "bit", nullable: false),
                    Adquiriente = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscripcionPersona", x => new { x.InscripcionesFolio, x.PersonasRUN });
                    table.ForeignKey(
                        name: "FK_InscripcionPersona_Inscripcion_InscripcionesFolio",
                        column: x => x.InscripcionesFolio,
                        principalTable: "Inscripcion",
                        principalColumn: "Folio",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InscripcionPersona_Personas_PersonasRUN",
                        column: x => x.PersonasRUN,
                        principalTable: "Personas",
                        principalColumn: "RUN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InscripcionPersona_PersonasRUN",
                table: "InscripcionPersona",
                column: "PersonasRUN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InscripcionPersona");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inscripcion",
                table: "Inscripcion");

            migrationBuilder.RenameTable(
                name: "Inscripcion",
                newName: "Inscripciones");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inscripciones",
                table: "Inscripciones",
                column: "Folio");
        }
    }
}
