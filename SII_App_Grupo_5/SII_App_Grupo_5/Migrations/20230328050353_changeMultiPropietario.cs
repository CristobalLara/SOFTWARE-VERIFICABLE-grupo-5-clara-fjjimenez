using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SII_App_Grupo_5.Migrations
{
    /// <inheritdoc />
    public partial class changeMultiPropietario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "predio",
                table: "MultiPropietarios",
                newName: "Predio");

            migrationBuilder.AlterColumn<string>(
                name: "Fojas",
                table: "MultiPropietarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Predio",
                table: "MultiPropietarios",
                newName: "predio");

            migrationBuilder.AlterColumn<int>(
                name: "Fojas",
                table: "MultiPropietarios",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
