using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;
using ClosedXML.Excel; // 👈 NUEVO: La maquinaria de Excel
using System.IO;       // 👈 NUEVO: Para manejar el archivo en memoria
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SistemaGestionMusulman.API.Controllers
{
    [Authorize] // <--- CANDADO: Nadie entra sin token
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilesController : ControllerBase
    {
        // El Recepcionista solo conoce al Gerente (Service), NO a la base de datos
        private readonly IPerfilMusulmanService _service;

        public PerfilesController(IPerfilMusulmanService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerfilMusulman>>> ObtenerTodosLosPerfiles()
        {
            var perfiles = await _service.ObtenerTodosAsync();
            return Ok(perfiles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPerfilPorId(Guid id)
        {
            var perfil = await _service.ObtenerPorIdAsync(id);
            if (perfil == null) return NotFound(new { mensaje = "El perfil no fue encontrado." });

            return Ok(perfil);
        }

        [HttpPost]
        public async Task<ActionResult<PerfilMusulman>> CrearPerfil(PerfilMusulman perfil)
        {
            var nuevoPerfil = await _service.CrearPerfilAsync(perfil);
            return CreatedAtAction(nameof(ObtenerPerfilPorId), new { id = nuevoPerfil.Id }, nuevoPerfil);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarPerfil(Guid id, PerfilMusulman perfilActualizado)
        {
            var exito = await _service.ActualizarPerfilAsync(id, perfilActualizado);
            if (!exito) return BadRequest(new { mensaje = "Error al actualizar. Verifica el ID." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPerfil(Guid id)
        {
            var exito = await _service.EliminarPerfilAsync(id);
            if (!exito) return NotFound(new { mensaje = "El perfil que intentas eliminar no existe." });

            return NoContent();
        }

        // =====================================================================
        // 🚀 NUEVA SECCIÓN DE REPORTES
        // =====================================================================

        [HttpGet("exportar/excel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // 1. Buscamos todos los datos de la base de datos
            var perfiles = await _service.ObtenerTodosAsync();

            // 2. Creamos un nuevo libro de Excel vacío
            using var workbook = new XLWorkbook();

            // 3. Creamos una hoja llamada "Directorio"
            var worksheet = workbook.Worksheets.Add("Directorio Musulmán");

            // 4. Escribimos los títulos (Fila 1)
            worksheet.Cell(1, 1).Value = "Cédula";
            worksheet.Cell(1, 2).Value = "Nombres";
            worksheet.Cell(1, 3).Value = "Apellidos";
            worksheet.Cell(1, 4).Value = "Sexo";
            worksheet.Cell(1, 5).Value = "Teléfono";
            worksheet.Cell(1, 6).Value = "Estado Civil";
            worksheet.Cell(1, 7).Value = "Fecha de Nacimiento";

            // ¡Toque de Ingeniero Senior! 🎨 Pintamos la cabecera de color verde oscuro corporativo
            var cabecera = worksheet.Range("A1:G1");
            cabecera.Style.Font.Bold = true;
            cabecera.Style.Fill.BackgroundColor = XLColor.FromHtml("#0b2e13"); // Verde de Al Musulmán
            cabecera.Style.Font.FontColor = XLColor.FromHtml("#d4af37"); // Letras doradas
            cabecera.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // 5. Llenamos las filas con los datos reales
            int fila = 2; // Empezamos en la fila 2 porque la 1 es la cabecera
            foreach (var perfil in perfiles)
            {
                worksheet.Cell(fila, 1).Value = perfil.NumeroCedula;
                worksheet.Cell(fila, 2).Value = perfil.Nombres;
                worksheet.Cell(fila, 3).Value = perfil.Apellidos;
                worksheet.Cell(fila, 4).Value = perfil.Sexo ?? "N/A";
                worksheet.Cell(fila, 5).Value = perfil.NumeroTelefono;
                worksheet.Cell(fila, 6).Value = perfil.EstadoCivil;
                worksheet.Cell(fila, 7).Value = perfil.FechaNacimiento.ToString("dd/MM/yyyy");

                fila++;
            }

            // 6. Ajustamos el ancho de las columnas automáticamente para que se vea profesional
            worksheet.Columns().AdjustToContents();

            // 7. Empaquetamos el Excel en la memoria RAM para enviarlo por internet
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var contenido = stream.ToArray();

            // 8. Se lo despachamos al navegador del usuario
            return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Directorio_Musulman.xlsx");

        }

        // =====================================================================
        // 🚀 SECCIÓN DE REPORTES: PDF (QuestPDF)
        // =====================================================================

        [HttpGet("exportar/pdf")]
        public async Task<IActionResult> ExportarPdf()
        {
            // 1. Configuramos la licencia gratuita comunitaria obligatoria de QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            // 2. Buscamos los datos
            var perfiles = await _service.ObtenerTodosAsync();

            // 3. Empezamos a dibujar el documento
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // 4. DIBUJAMOS EL ENCABEZADO
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("Directorio de Perfiles Musulmanes")
                                .FontSize(20).SemiBold().FontColor("#0b2e13"); // Verde corporativo
                            column.Item().Text($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                    });

                    // 5. DIBUJAMOS LA TABLA
                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        // Estructura de las columnas
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(70);  // Cédula
                            columns.RelativeColumn();    // Nombres
                            columns.RelativeColumn();    // Apellidos
                            columns.ConstantColumn(60);  // Sexo
                            columns.RelativeColumn();    // Teléfono
                        });

                        // Encabezados de la tabla (Pintados de verde y oro)
                        table.Header(header =>
                        {
                            header.Cell().Background("#0b2e13").Padding(3).Text("Cédula").FontColor("#d4af37").SemiBold();
                            header.Cell().Background("#0b2e13").Padding(3).Text("Nombres").FontColor("#d4af37").SemiBold();
                            header.Cell().Background("#0b2e13").Padding(3).Text("Apellidos").FontColor("#d4af37").SemiBold();
                            header.Cell().Background("#0b2e13").Padding(3).Text("Sexo").FontColor("#d4af37").SemiBold();
                            header.Cell().Background("#0b2e13").Padding(3).Text("Teléfono").FontColor("#d4af37").SemiBold();
                        });

                        // Filas con los datos
                        foreach (var perfil in perfiles)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(perfil.NumeroCedula);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(perfil.Nombres);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(perfil.Apellidos);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(perfil.Sexo ?? "N/A");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(perfil.NumeroTelefono);
                        }
                    });

                    // 6. PIE DE PÁGINA (Números de página)
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            // 7. Generamos el archivo PDF en la memoria RAM
            var pdfBytes = document.GeneratePdf();

            // 8. Despachamos al Frontend
            return File(pdfBytes, "application/pdf", "Directorio_Musulman.pdf");
        }
    }

}

   