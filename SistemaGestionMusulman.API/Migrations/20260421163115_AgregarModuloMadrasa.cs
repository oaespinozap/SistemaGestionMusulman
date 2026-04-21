using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionMusulman.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloMadrasa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClasesMadrasa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NombreMateria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Profesor = table.Column<string>(type: "text", nullable: true),
                    Horario = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClasesMadrasa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InscripcionesClases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstudianteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscripcionesClases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InscripcionesClases_ClasesMadrasa_ClaseId",
                        column: x => x.ClaseId,
                        principalTable: "ClasesMadrasa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InscripcionesClases_PerfilesMusulmanes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "PerfilesMusulmanes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InscripcionesClases_ClaseId",
                table: "InscripcionesClases",
                column: "ClaseId");

            migrationBuilder.CreateIndex(
                name: "IX_InscripcionesClases_EstudianteId",
                table: "InscripcionesClases",
                column: "EstudianteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InscripcionesClases");

            migrationBuilder.DropTable(
                name: "ClasesMadrasa");
        }
    }
}
