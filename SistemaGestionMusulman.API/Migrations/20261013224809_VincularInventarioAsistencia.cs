using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionMusulman.API.Migrations
{
    /// <inheritdoc />
    public partial class VincularInventarioAsistencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArticuloInventarioId",
                table: "AsistenciasSociales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CantidadExtraida",
                table: "AsistenciasSociales",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticuloInventarioId",
                table: "AsistenciasSociales");

            migrationBuilder.DropColumn(
                name: "CantidadExtraida",
                table: "AsistenciasSociales");
        }
    }
}
