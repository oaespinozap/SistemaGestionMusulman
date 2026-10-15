using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.Backend.Models;
using ClosedXML.Excel;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SistemaGestionMusulman.Backend.Controllers
{
    [Authorize] // Protegemos el almacén
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Inyectamos la conexión a la base de datos
        public InventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Obtener todo el inventario (Para ver qué tenemos en el almacén)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloInventario>>> ObtenerInventario()
        {
            return await _context.Inventario.OrderByDescending(i => i.FechaIngreso).ToListAsync();
        }

        // 2. Registrar una nueva donación que entra al almacén
        [HttpPost]
        public async Task<ActionResult<ArticuloInventario>> RegistrarArticulo(ArticuloInventario nuevoArticulo)
        {
            _context.Inventario.Add(nuevoArticulo);
            await _context.SaveChangesAsync();

            return Ok(nuevoArticulo);
        }

        // 3. Actualizar inventario (Ej: Cuando restamos mercancía por una asistencia)
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarArticulo(Guid id, ArticuloInventario articuloActualizado)
        {
            if (id != articuloActualizado.Id)
            {
                return BadRequest("El ID del artículo no coincide.");
            }

            _context.Entry(articuloActualizado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();

        }
        // =====================================================================
        // 🚀 SECCIÓN DE REPORTES: INVENTARIO Y ALMACÉN
        // =====================================================================

        [HttpGet("exportar/excel")]
        public async Task<IActionResult> ExportarInventarioExcel()
        {
            var articulos = await _context.Inventario.OrderByDescending(i => i.FechaIngreso).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Estado de Inventario");

            // Títulos
            worksheet.Cell(1, 1).Value = "Artículo";
            worksheet.Cell(1, 2).Value = "Categoría";
            worksheet.Cell(1, 3).Value = "Stock Disponible";
            worksheet.Cell(1, 4).Value = "Fecha Ingreso";
            worksheet.Cell(1, 5).Value = "Fecha Caducidad";

            // Diseño de cabecera
            var cabecera = worksheet.Range("A1:E1");
            cabecera.Style.Font.Bold = true;
            cabecera.Style.Fill.BackgroundColor = XLColor.FromHtml("#0b2e13");
            cabecera.Style.Font.FontColor = XLColor.FromHtml("#d4af37");
            cabecera.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Datos
            int fila = 2;
            foreach (var item in articulos)
            {
                worksheet.Cell(fila, 1).Value = item.Nombre;
                worksheet.Cell(fila, 2).Value = item.Categoria;

                var cellStock = worksheet.Cell(fila, 3);
                cellStock.Value = item.CantidadDisponible;
                // Alerta de stock bajo en Excel
                if (item.CantidadDisponible <= 5)
                {
                    cellStock.Style.Font.FontColor = XLColor.Red;
                    cellStock.Style.Font.Bold = true;
                }

                worksheet.Cell(fila, 4).Value = item.FechaIngreso.ToShortDateString();
                worksheet.Cell(fila, 5).Value = item.FechaCaducidad?.ToShortDateString() ?? "N/A";
                fila++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var contenido = stream.ToArray();

            return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventario_AlMusulman.xlsx");
        }

        [HttpGet("exportar/pdf")]
        public async Task<IActionResult> ExportarInventarioPdf()
        {
            var articulos = await _context.Inventario.OrderByDescending(i => i.FechaIngreso).ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(x => ComposeContent(x, articulos.ToList()));
                    page.Footer().Element(ComposeFooter);
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", "Inventario_AlMusulman.pdf");
        }

        // --- Funciones auxiliares para dibujar el PDF de Inventario ---
        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("SISTEMA DE GESTIÓN AL MUSULMÁN").FontSize(20).SemiBold().FontColor("#0b2e13");
                    column.Item().Text("Reporte de Estado de Inventario y Almacén").FontSize(14).FontColor(Colors.Grey.Medium);
                    column.Item().PaddingTop(5).Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
                });
            });
        }

        private void ComposeContent(IContainer container, List<ArticuloInventario> articulos)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // Artículo
                        columns.RelativeColumn(2); // Categoría
                        columns.RelativeColumn(2); // Stock
                        columns.RelativeColumn(2); // Vencimiento
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#0b2e13").Padding(2).Text("Artículo").FontColor("#d4af37").Bold();
                        header.Cell().Background("#0b2e13").Padding(2).Text("Categoría").FontColor("#d4af37").Bold();
                        header.Cell().Background("#0b2e13").Padding(2).Text("Stock").FontColor("#d4af37").Bold();
                        header.Cell().Background("#0b2e13").Padding(2).Text("Vencimiento").FontColor("#d4af37").Bold();
                    });

                    foreach (var item in articulos)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.Nombre);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.Categoria);

                        // Resaltar en rojo si hay 5 o menos
                        if (item.CantidadDisponible <= 5)
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.CantidadDisponible.ToString()).FontColor(Colors.Red.Medium).Bold();
                        else
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.CantidadDisponible.ToString());

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2).Text(item.FechaCaducidad?.ToShortDateString() ?? "N/A");
                    }
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        }








    }


}

