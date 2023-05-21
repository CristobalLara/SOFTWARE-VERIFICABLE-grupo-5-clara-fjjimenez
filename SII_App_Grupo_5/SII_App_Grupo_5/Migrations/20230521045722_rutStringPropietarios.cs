using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SII_App_Grupo_5.Migrations
{
    /// <inheritdoc />
    public partial class rutStringPropietarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Propietario",
                table: "MultiPropietarios",
                newName: "RutPropietario");

            migrationBuilder.AlterColumn<string>(
                name: "Rut",
                table: "Enajenantes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Rut",
                table: "Adquirientes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RutPropietario",
                table: "MultiPropietarios",
                newName: "Propietario");

            migrationBuilder.AlterColumn<int>(
                name: "Rut",
                table: "Enajenantes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Rut",
                table: "Adquirientes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
