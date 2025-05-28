using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Integrado_de_Registro.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginToDocente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Docentes",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contrasena",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "Contrasena",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Docentes");
        }
    }
}
