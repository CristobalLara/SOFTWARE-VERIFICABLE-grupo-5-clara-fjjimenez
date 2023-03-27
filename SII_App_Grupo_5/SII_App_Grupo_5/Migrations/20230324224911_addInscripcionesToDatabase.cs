using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SII_App_Grupo_5.Migrations
{
    /// <inheritdoc />
    public partial class addInscripcionesToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inscripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<int>(type: "int", nullable: false),
                    NaturalezaEscritura = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Comuna = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Manzana = table.Column<int>(type: "int", nullable: false),
                    Predio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fojas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroInscripcion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripciones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inscripciones");
        }
    }
}
