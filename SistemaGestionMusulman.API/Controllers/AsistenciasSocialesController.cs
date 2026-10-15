using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;
using SistemaGestionMusulman.Backend.Models;
using ClosedXML.Excel;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore; // Necesario para .Include()   

namespace SistemaGestionMusulman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciasSocialesController : ControllerBase
    {
        private readonly IAsistenciaSocialService _service;

        // 1. NUEVO: Contratamos al encargado de la Bóveda Principal
        private readonly ApplicationDbContext _context;

        // 2. ACTUALIZADO: El controlador ahora recibe a ambos supervisores
        public AsistenciasSocialesController(IAsistenciaSocialService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        // GET: api/AsistenciasSociales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AsistenciaSocial>>> Get()
        {
            var asistencias = await _service.ObtenerTodasAsync();
            return Ok(asistencias);
        }

        // GET: api/AsistenciasSociales/{un-id-largo}
        [HttpGet("{id}")]
        public async Task<ActionResult<AsistenciaSocial>> Get(Guid id)
        {
            var asistencia = await _service.ObtenerPorIdAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }
            return Ok(asistencia);
        }

        // POST: api/AsistenciasSociales
        // ACTUALIZADO: Aquí ocurre la magia de la deducción de inventario
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AsistenciaSocial asistencia)
        {
            // PASO A: Verificamos si la ayuda social trae mercancía física (Atamel, comida, etc.)
            if (asistencia.ArticuloInventarioId.HasValue && asistencia.CantidadExtraida.HasValue && asistencia.CantidadExtraida > 0)
            {
                // Buscamos la mercancía en los estantes físicos
                var articulo = await _context.Inventario.FindAsync(asistencia.ArticuloInventarioId);

                if (articulo == null)
                {
                    return BadRequest("El artículo seleccionado no existe en el inventario.");
                }

                // Protegemos el almacén: No podemos entregar más de lo que tenemos
                if (articulo.CantidadDisponible < asistencia.CantidadExtraida.Value)
                {
                    return BadRequest($"Stock insuficiente. Solo quedan {articulo.CantidadDisponible} unidades.");
                }

                // Matemáticas: Restamos lo entregado
                articulo.CantidadDisponible -= asistencia.CantidadExtraida.Value;

                // Guardamos el cambio de stock en la base de datos
                await _context.SaveChangesAsync();
            }

            // PASO B: Guardamos el registro de la asistencia social (como siempre lo ha hecho el sistema)
            await _service.AgregarAsync(asistencia);

            return CreatedAtAction(nameof(Get), new { id = asistencia.Id }, asistencia);
        }

        // PUT: api/AsistenciasSociales/{un-id-largo}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] AsistenciaSocial asistencia)
        {
            if (id != asistencia.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del objeto.");
            }

            await _service.ActualizarAsync(asistencia);
            return NoContent();
        }

        // DELETE: api/AsistenciasSociales/{un-id-largo}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
        // =====================================================================
        // 🚀 SECCIÓN DE REPORTES: ASISTENCIAS SOCIALES Y ZAKAT
        // =====================================================================

        [HttpGet("exportar/excel")]
        public async Task<IActionResult> ExportarAsistenciasExcel()
        {
            // Buscamos todas las asistencias e incluimos los datos del beneficiario
            var asistencias = await _context.AsistenciasSociales
                                            .Include(a => a.PerfilMusulman)
                                            .OrderByDescending(a => a.FechaEntrega)
                                            .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Auditoría de Ayudas");

            // Títulos
            worksheet.Cell(1, 1).Value = "Fecha";
            worksheet.Cell(1, 2).Value = "Beneficiario (Cédula)";
            worksheet.Cell(1, 3).Value = "Categoría Zakat";
            worksheet.Cell(1, 4).Value = "Tipo de Ayuda";
            worksheet.Cell(1, 5).Value = "Monto/Valor";
            worksheet.Cell(1, 6).Value = "Observaciones";

            // Diseño de cabecera
            var cabecera = worksheet.Range("A1:F1");
            cabecera.Style.Font.Bold = true;
            cabecera.Style.Fill.BackgroundColor = XLColor.FromHtml("#0b2e13"); // Verde Corporativo
            cabecera.Style.Font.FontColor = XLColor.FromHtml("#d4af37");     // Dorado
            cabecera.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Datos
            int fila = 2;
            foreach (var item in asistencias)
            {
                worksheet.Cell(fila, 1).Value = item.FechaEntrega.ToString("dd/MM/yyyy");

                // Formateamos el beneficiario: Nombres Apellidos (V-123456)
                string beneficiarioInfo = item.PerfilMusulman != null
                    ? $"{item.PerfilMusulman.Nombres} {item.PerfilMusulman.Apellidos} ({item.PerfilMusulman.NumeroCedula})"
                    : "Beneficiario No Encontrado";
                worksheet.Cell(fila, 2).Value = beneficiarioInfo;

                worksheet.Cell(fila, 3).Value = item.CategoriaZakat;
                worksheet.Cell(fila, 4).Value = item.TipoDeAyuda;

                // Formato de moneda para el monto
                var celdaMonto = worksheet.Cell(fila, 5);
                celdaMonto.Value = item.MontoEstimado;
                celdaMonto.Style.NumberFormat.Format = "$#,##0.00";

                worksheet.Cell(fila, 6).Value = item.Observaciones ?? "Ninguna";
                fila++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var contenido = stream.ToArray();

            return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Asistencias_Sociales_AlMusulman.xlsx");
        }

        [HttpGet("exportar/pdf")]
        public async Task<IActionResult> ExportarAsistenciasPdf()
        {
            var asistencias = await _context.AsistenciasSociales
                                            .Include(a => a.PerfilMusulman)
                                            .OrderByDescending(a => a.FechaEntrega)
                                            .ToListAsync();

            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape()); // Orientación Horizontal (Landscape) para que quepan más datos
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(ComposeAsistenciaHeader);
                    page.Content().Element(x => ComposeAsistenciaContent(x, asistencias));
                    page.Footer().Element(ComposeAsistenciaFooter);
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", "Auditoria_Asistencias_AlMusulman.pdf");
        }

        // --- Funciones auxiliares para dibujar el PDF de Asistencias ---
        private void ComposeAsistenciaHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("SISTEMA DE GESTIÓN AL MUSULMÁN").FontSize(22).SemiBold().FontColor("#0b2e13");
                    column.Item().Text("Reporte de Auditoría: Asistencias Sociales y Zakat").FontSize(14).FontColor(Colors.Grey.Medium);
                    column.Item().PaddingTop(5).Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
                });
            });
        }

        private void ComposeAsistenciaContent(IContainer container, List<AsistenciaSocial> asistencias)
        {
            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
            {
                column.Spacing(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(70);  // Fecha
                        columns.RelativeColumn(3);  // Beneficiario
                        columns.RelativeColumn(2);  // Categoría Zakat
                        columns.RelativeColumn(2);  // Tipo Ayuda
                        columns.ConstantColumn(80);  // Monto
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#0b2e13").Padding(3).Text("Fecha").FontColor("#d4af37").Bold();
                        header.Cell().Background("#0b2e13").Padding(3).Text("Beneficiario").FontColor("#d4af37").Bold();
                        header.Cell().Background("#0b2e13").Padding(3).Text("Categoría").FontColor("#d4af37").Bold();
                        header.Cell().Background("#0b2e13").Padding(3).Text("Ayuda").FontColor("#d4af37").Bold();
                        header.Cell().Background("#0b2e13").Padding(3).AlignRight().Text("Valor ($)").FontColor("#d4af37").Bold();
                    });

                    foreach (var item in asistencias)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.FechaEntrega.ToString("dd/MM/yyyy"));

                        string nombreBeneficiario = item.PerfilMusulman != null
                            ? $"{item.PerfilMusulman.Nombres} {item.PerfilMusulman.Apellidos}"
                            : "Desconocido";
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(nombreBeneficiario);

                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.CategoriaZakat);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(item.TipoDeAyuda);

                        // Mostramos el monto con formato o un guión si es null
                        string montoFormat = item.MontoEstimado.HasValue ? item.MontoEstimado.Value.ToString("C2", new System.Globalization.CultureInfo("en-US")) : "-";
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3).AlignRight().Text(montoFormat);
                    }
                });
            });
        }

        private void ComposeAsistenciaFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Auditoría Generada - Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
            });
        }






    }
}