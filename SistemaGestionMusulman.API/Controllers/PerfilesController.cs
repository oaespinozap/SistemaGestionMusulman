using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services; // ¡Ahora usa el Servicio!

namespace SistemaGestionMusulman.API.Controllers
{
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
    }
}