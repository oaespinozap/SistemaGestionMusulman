using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;
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

        public InventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================================
        // 1. CATÁLOGO MAESTRO (Solo muestra artículos ACTIVOS)
        // =====================================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticuloInventario>>> ObtenerInventario()
        {
            // 🚀 NUEVO: Solo traemos los que NO han sido dados de baja
            return await _context.Inventario
                .Where(i => i.Activo == true)
                .OrderByDescending(i => i.FechaIngreso)
                .ToListAsync();
        }

        // =====================================================================
        // 2. CREAR NUEVO ARTÍCULO (Catálogo inicial)
        // =====================================================================
        [HttpPost]
        public async Task<ActionResult<ArticuloInventario>> RegistrarArticulo(ArticuloInventario nuevoArticulo)
        {
            if (nuevoArticulo.Id == Guid.Empty) nuevoArticulo.Id = Guid.NewGuid();

            nuevoArticulo.Activo = true; // Nos aseguramos de que nazca activo
            _context.Inventario.Add(nuevoArticulo);

            // Obtenemos el usuario real del Token de Seguridad
            var usuarioActual = User.Identity?.Name ?? "Usuario Desconocido";

            var movimientoEntrada = new MovimientoInventario
            {
                ArticuloInventarioId = nuevoArticulo.Id,
                TipoMovimiento = "Entrada",
                Cantidad = nuevoArticulo.CantidadDisponible,
                Motivo = "Registro inicial en catálogo",
                UsuarioResponsable = usuarioActual // <-- Auditoría real
            };

            _context.MovimientosInventario.Add(movimientoEntrada);
            await _context.SaveChangesAsync();

            return Ok(nuevoArticulo);
        }

        // =====================================================================
        // 3. REGISTRAR ENTRADA O SALIDA (El motor del Kárdex real)
        // =====================================================================
        [HttpPost("{id}/movimientos")]
        public async Task<IActionResult> RegistrarMovimiento(Guid id, [FromBody] MovimientoInventario dtoMovimiento)
        {
            var articulo = await _context.Inventario.FindAsync(id);
            if (articulo == null || !articulo.Activo)
                return NotFound(new { Mensaje = "El artículo no existe o está dado de baja." });

            if (dtoMovimiento.TipoMovimiento == "Salida" && articulo.CantidadDisponible < dtoMovimiento.Cantidad)
                return BadRequest(new { Mensaje = "No hay stock suficiente para realizar esta salida." });

            // Actualizamos el Stock Maestro
            if (dtoMovimiento.TipoMovimiento == "Entrada")
                articulo.CantidadDisponible += dtoMovimiento.Cantidad;
            else if (dtoMovimiento.TipoMovimiento == "Salida")
                articulo.CantidadDisponible -= dtoMovimiento.Cantidad;
            else
                return BadRequest(new { Mensaje = "Tipo de movimiento inválido. Debe ser 'Entrada' o 'Salida'." });

            // Configuramos y guardamos la traza de auditoría
            dtoMovimiento.ArticuloInventarioId = id;
            dtoMovimiento.FechaMovimiento = DateTime.UtcNow;
            dtoMovimiento.UsuarioResponsable = User.Identity?.Name ?? "Usuario Desconocido";

            _context.MovimientosInventario.Add(dtoMovimiento);
            await _context.SaveChangesAsync();

            return Ok(articulo);
        }

        // =====================================================================
        // 4. DAR DE BAJA UN ARTÍCULO (Soft Delete)
        // =====================================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarArticulo(Guid id, [FromQuery] string motivo = "Artículo desincorporado del inventario")
        {
            var articulo = await _context.Inventario.FindAsync(id);
            if (articulo == null || !articulo.Activo) return NotFound();

            // 🚀 NUEVO: Soft Delete. No lo borramos, lo ocultamos.
            articulo.Activo = false;

            // Registramos quién ejecutó la orden de baja
            var usuarioActual = User.Identity?.Name ?? "Usuario Desconocido";
            var movimientoBaja = new MovimientoInventario
            {
                ArticuloInventarioId = id,
                TipoMovimiento = "Baja",
                Cantidad = articulo.CantidadDisponible,
                Motivo = motivo,
                UsuarioResponsable = usuarioActual
            };

            // Vaciamos el stock para que no quede dinero/mercancía flotando
            articulo.CantidadDisponible = 0;

            _context.MovimientosInventario.Add(movimientoBaja);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================================
        // 5. OBTENER HISTORIAL DE UN ARTÍCULO
        // =====================================================================
        [HttpGet("{id}/movimientos")]
        public async Task<ActionResult<IEnumerable<MovimientoInventario>>> ObtenerHistorialArticulo(Guid id)
        {
            var movimientos = await _context.MovimientosInventario
                .Where(m => m.ArticuloInventarioId == id)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            return Ok(movimientos);
        }

        // =====================================================================
        // 🚀 SECCIÓN DE REPORTES: INVENTARIO Y ALMACÉN (Solo Activos)
        // =====================================================================
        [HttpGet("exportar/excel")]
        public async Task<IActionResult> ExportarInventarioExcel()
        {
            var articulos = await _context.Inventario.Where(i => i.Activo).OrderByDescending(i => i.FechaIngreso).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Estado de Inventario");

            worksheet.Cell(1, 1).Value = "Artículo";
            worksheet.Cell(1, 2).Value = "Categoría";
            worksheet.Cell(1, 3).Value = "Stock Disponible";
            worksheet.Cell(1, 4).Value = "Fecha Ingreso";
            worksheet.Cell(1, 5).Value = "Fecha Caducidad";

            var cabecera = worksheet.Range("A1:E1");
            cabecera.Style.Font.Bold = true;
            cabecera.Style.Fill.BackgroundColor = XLColor.FromHtml("#0b2e13");
            cabecera.Style.Font.FontColor = XLColor.FromHtml("#d4af37");
            cabecera.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int fila = 2;
            foreach (var item in articulos)
            {
                worksheet.Cell(fila, 1).Value = item.Nombre;
                worksheet.Cell(fila, 2).Value = item.Categoria;

                var cellStock = worksheet.Cell(fila, 3);
                cellStock.Value = item.CantidadDisponible;
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
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Inventario_AlMusulman.xlsx");
        }

        [HttpGet("exportar/pdf")]
        public async Task<IActionResult> ExportarInventarioPdf()
        {
            var articulos = await _context.Inventario.Where(i => i.Activo).OrderByDescending(i => i.FechaIngreso).ToListAsync();

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

            return File(document.GeneratePdf(), "application/pdf", "Inventario_AlMusulman.pdf");
        }

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
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
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