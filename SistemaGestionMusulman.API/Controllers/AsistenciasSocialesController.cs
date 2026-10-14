using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;
using SistemaGestionMusulman.Backend.Models;

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
    }
}