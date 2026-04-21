using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionMusulman.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloSadaqah : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DonacionesSadaqah",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Item = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    UnidadMedida = table.Column<string>(type: "text", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BeneficiarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonacionesSadaqah", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DonacionesSadaqah_PerfilesMusulmanes_BeneficiarioId",
                        column: x => x.BeneficiarioId,
                        principalTable: "PerfilesMusulmanes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonacionesSadaqah_BeneficiarioId",
                table: "DonacionesSadaqah",
                column: "BeneficiarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonacionesSadaqah");
        }
    }
}
