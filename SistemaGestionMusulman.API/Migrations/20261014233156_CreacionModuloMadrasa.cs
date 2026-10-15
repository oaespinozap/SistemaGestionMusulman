using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionMusulman.API.Migrations
{
    /// <inheritdoc />
    public partial class CreacionModuloMadrasa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM \"ClasesMadrasa\";");
            migrationBuilder.DropTable(
                name: "InscripcionesClases");

            migrationBuilder.DropColumn(
                name: "Horario",
                table: "ClasesMadrasa");

            migrationBuilder.DropColumn(
                name: "NombreMateria",
                table: "ClasesMadrasa");

            migrationBuilder.DropColumn(
                name: "Profesor",
                table: "ClasesMadrasa");

            migrationBuilder.AddColumn<bool>(
                name: "PruebaCicloBasicoAprobada",
                table: "PerfilesMusulmanes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsMasterClass",
                table: "ClasesMadrasa",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "ClasesMadrasa",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "MateriaId",
                table: "ClasesMadrasa",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "TemaDictado",
                table: "ClasesMadrasa",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AsistenciasMadrasa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaseMadrasaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PerfilMusulmanId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsistenciasMadrasa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsistenciasMadrasa_ClasesMadrasa_ClaseMadrasaId",
                        column: x => x.ClaseMadrasaId,
                        principalTable: "ClasesMadrasa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsistenciasMadrasa_PerfilesMusulmanes_PerfilMusulmanId",
                        column: x => x.PerfilMusulmanId,
                        principalTable: "PerfilesMusulmanes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    EsObligatoria = table.Column<bool>(type: "boolean", nullable: false),
                    MetaClases = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClasesMadrasa_MateriaId",
                table: "ClasesMadrasa",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsistenciasMadrasa_ClaseMadrasaId",
                table: "AsistenciasMadrasa",
                column: "ClaseMadrasaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsistenciasMadrasa_PerfilMusulmanId",
                table: "AsistenciasMadrasa",
                column: "PerfilMusulmanId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClasesMadrasa_Materias_MateriaId",
                table: "ClasesMadrasa",
                column: "MateriaId",
                principalTable: "Materias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClasesMadrasa_Materias_MateriaId",
                table: "ClasesMadrasa");

            migrationBuilder.DropTable(
                name: "AsistenciasMadrasa");

            migrationBuilder.DropTable(
                name: "Materias");

            migrationBuilder.DropIndex(
                name: "IX_ClasesMadrasa_MateriaId",
                table: "ClasesMadrasa");

            migrationBuilder.DropColumn(
                name: "PruebaCicloBasicoAprobada",
                table: "PerfilesMusulmanes");

            migrationBuilder.DropColumn(
                name: "EsMasterClass",
                table: "ClasesMadrasa");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "ClasesMadrasa");

            migrationBuilder.DropColumn(
                name: "MateriaId",
                table: "ClasesMadrasa");

            migrationBuilder.DropColumn(
                name: "TemaDictado",
                table: "ClasesMadrasa");

            migrationBuilder.AddColumn<string>(
                name: "Horario",
                table: "ClasesMadrasa",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreMateria",
                table: "ClasesMadrasa",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Profesor",
                table: "ClasesMadrasa",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InscripcionesClases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaseId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstudianteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
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
    }
}
