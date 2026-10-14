using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionMusulman.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSexoPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sexo",
                table: "PerfilesMusulmanes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "PerfilesMusulmanes");
        }
    }
}
