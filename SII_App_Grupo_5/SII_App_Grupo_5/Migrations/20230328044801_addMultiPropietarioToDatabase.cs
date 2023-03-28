using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SII_App_Grupo_5.Migrations
{
    /// <inheritdoc />
    public partial class addMultiPropietarioToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MultiPropietarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comuna = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manzana = table.Column<int>(type: "int", nullable: false),
                    predio = table.Column<int>(type: "int", nullable: false),
                    Propietario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PorcentajeDerecho = table.Column<int>(type: "int", nullable: false),
                    Fojas = table.Column<int>(type: "int", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnoInscripcion = table.Column<int>(type: "int", nullable: false),
                    NumeroInscripcion = table.Column<int>(type: "int", nullable: false),
                    AnoVigenciaInicial = table.Column<int>(type: "int", nullable: false),
                    AnoVigenciaFinal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiPropietarios", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultiPropietarios");
        }
    }
}
