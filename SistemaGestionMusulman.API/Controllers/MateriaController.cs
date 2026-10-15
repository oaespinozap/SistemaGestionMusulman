using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;
using ClosedXML.Excel;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SistemaGestionMusulman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] -> Lo dejamos comentado por ahora para hacer pruebas fáciles
    public class MateriaController : ControllerBase
    {
        private readonly IMateriaService _materiaService;

        public MateriaController(IMateriaService materiaService)
        {
            _materiaService = materiaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Materia>>> GetMaterias()
        {
            var materias = await _materiaService.GetAllAsync();
            return Ok(materias);
        }

        [HttpPost]
        public async Task<ActionResult<Materia>> PostMateria(Materia materia)
        {
            // Nos aseguramos de generar un ID nuevo
            materia.Id = Guid.NewGuid();
            var nuevaMateria = await _materiaService.AddAsync(materia);
            return CreatedAtAction(nameof(GetMaterias), new { id = nuevaMateria.Id }, nuevaMateria);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMateria(Guid id)
        {
            await _materiaService.DeleteAsync(id);
            return NoContent();
        }
        // =====================================================================
        // 🚀 SECCIÓN DE REPORTES: MADRASA (PLAN DE ESTUDIOS)
        // =====================================================================

        [HttpGet("exportar/excel")]
        public async Task<IActionResult> ExportarMadrasaExcel()
        {
            // 1. CORREGIDO: Usamos _materiaService en lugar de _context
            var materiasData = await _materiaService.GetAllAsync();
            var materias = materiasData.OrderBy(m => m.Nombre).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Materias Madrasa");

            // 2. CORREGIDO: Títulos basados en tu modelo real
            worksheet.Cell(1, 1).Value = "Nombre de la Materia";
            worksheet.Cell(1, 2).Value = "Tipo de Materia";
            worksheet.Cell(1, 3).Value = "Meta de Clases";

            // Diseño de cabecera
            var cabecera = worksheet.Range("A1:C1");
            cabecera.Style.Font.Bold = true;
            cabecera.Style.Fill.BackgroundColor = XLColor.FromHtml("#0b2e13"); // Verde Corporativo
            cabecera.Style.Font.FontColor = XLColor.FromHtml("#d4af37");     // Dorado
            cabecera.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Datos
            int fila = 2;
            foreach (var item in materias)
            {
                worksheet.Cell(fila, 1).Value = item.Nombre;
                // Transformamos el "booleano" (true/false) en un texto amigable
                worksheet.Cell(fila, 2).Value = item.EsObligatoria ? "Obligatoria" : "Opcional";

                var celdaMeta = worksheet.Cell(fila, 3);
                celdaMeta.Value = item.MetaClases;
                if (item.MetaClases == 0) celdaMeta.Value = "Infinita (Sin meta)";

                fila++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var contenido = stream.ToArray();

            return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Madrasa_Materias.xlsx");
        }

        [HttpGet("exportar/pdf")]
        public async Task<IActionResult> ExportarMadrasaPdf()
        {
            // 1. CORREGIDO: Usamos _materiaService
            var materiasData = await _materiaService.GetAllAsync();
            var materias = materiasData.OrderBy(m => m.Nombre).ToList();

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(ComposeMadrasaHeader);
                    page.Content().Element(x => ComposeMadrasaContent(x, materias));
                    page.Footer().Element(ComposeMadrasaFooter);
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", "Madrasa_Materias.pdf");
        }

        // --- Funciones auxiliares para dibujar el PDF de la Madrasa ---
        private void ComposeMadrasaHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("MADRASA AL MUSULMÁN").FontSize(22).SemiBold().FontColor("#0b2e13");
                    column.Item().Text("Catálogo Oficial de Materias").FontSize(14).FontColor(Colors.Grey.Medium);
                    column.Item().PaddingTop(5).Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
                });
            });
        }

        private void ComposeMadrasaContent(IContainer container, List<Materia> materias)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);  // Materia (Más ancho)
                        columns.RelativeColumn(3);  // Tipo (Obligatoria/Opcional)
                        columns.RelativeColumn(2);  // Meta de clases
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#0b2e13").Padding(3).Text("Nombre de la Materia").FontColor("#d4af37").Bold();
                        header.Cell().Background("#0b2e13").Padding(3).Text("Tipo").FontColor("#d4af37").Bold();
                        header.Cell().Background("#0b2e13").Padding(3).Text("Meta de Clases").FontColor("#d4af37").Bold();
                    });

                    foreach (var item in materias)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.Nombre);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.EsObligatoria ? "Obligatoria" : "Opcional");
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.MetaClases == 0 ? "Infinita" : item.MetaClases.ToString());
                    }
                });
            });
        }

        private void ComposeMadrasaFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Catálogo de Materias - Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        }
    }





    }

