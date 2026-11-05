using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionMusulman.API.Migrations
{
    /// <inheritdoc />
    public partial class KardexV2_Auditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Destinatario",
                table: "MovimientosInventario",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Inventario",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destinatario",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Inventario");
        }
    }
}
