using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.Backend.Models;

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
    }
}