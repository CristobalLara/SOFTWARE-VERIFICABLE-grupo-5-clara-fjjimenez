using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SII_App_Grupo_5.Migrations
{
    /// <inheritdoc />
    public partial class addNewModelToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inscripciones",
                columns: table => new
                {
                    Folio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NaturalezaEscritura = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Comuna = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Manzana = table.Column<int>(type: "int", nullable: false),
                    Predio = table.Column<int>(type: "int", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fojas = table.Column<int>(type: "int", nullable: false),
                    NumeroInscripcion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripciones", x => x.Folio);
                });

            migrationBuilder.CreateTable(
                name: "MultiPropietarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comuna = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manzana = table.Column<int>(type: "int", nullable: false),
                    Predio = table.Column<int>(type: "int", nullable: false),
                    Propietario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcentajeDerecho = table.Column<int>(type: "int", nullable: false),
                    Fojas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnoInscripcion = table.Column<int>(type: "int", nullable: false),
                    NumeroInscripcion = table.Column<int>(type: "int", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnoVigenciaInicial = table.Column<int>(type: "int", nullable: false),
                    AnoVigenciaFinal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiPropietarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Adquirientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InscripcionId = table.Column<int>(type: "int", nullable: false),
                    Rut = table.Column<int>(type: "int", nullable: false),
                    PorcentajeDerecho = table.Column<int>(type: "int", nullable: false),
                    Acreditado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adquirientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adquirientes_Inscripciones_InscripcionId",
                        column: x => x.InscripcionId,
                        principalTable: "Inscripciones",
                        principalColumn: "Folio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enajenantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InscripcionId = table.Column<int>(type: "int", nullable: false),
                    Rut = table.Column<int>(type: "int", nullable: false),
                    PorcentajeDerecho = table.Column<int>(type: "int", nullable: false),
                    Acreditado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enajenantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enajenantes_Inscripciones_InscripcionId",
                        column: x => x.InscripcionId,
                        principalTable: "Inscripciones",
                        principalColumn: "Folio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adquirientes_InscripcionId",
                table: "Adquirientes",
                column: "InscripcionId");

            migrationBuilder.CreateIndex(
                name: "IX_Enajenantes_InscripcionId",
                table: "Enajenantes",
                column: "InscripcionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adquirientes");

            migrationBuilder.DropTable(
                name: "Enajenantes");

            migrationBuilder.DropTable(
                name: "MultiPropietarios");

            migrationBuilder.DropTable(
                name: "Inscripciones");
        }
    }
}
