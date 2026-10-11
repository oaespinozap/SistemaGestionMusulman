using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;
using SistemaGestionMusulman.Backend.Models;

namespace SistemaGestionMusulman.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciasSocialesController : ControllerBase
    {
        private readonly IAsistenciaSocialService _service;

        // Aquí el Recepcionista "contrata" al Supervisor
        public AsistenciasSocialesController(IAsistenciaSocialService service)
        {
            _service = service;
        }

        // GET: api/AsistenciasSociales
        // Este método devuelve TODAS las asistencias (Para llenar tu tabla principal en pantalla)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AsistenciaSocial>>> Get()
        {
            var asistencias = await _service.ObtenerTodasAsync();
            return Ok(asistencias);
        }

        // GET: api/AsistenciasSociales/{un-id-largo}
        // Este método busca UNA asistencia específica (Para ver el detalle)
        [HttpGet("{id}")]
        public async Task<ActionResult<AsistenciaSocial>> Get(Guid id)
        {
            var asistencia = await _service.ObtenerPorIdAsync(id);
            if (asistencia == null)
            {
                return NotFound(); // Error 404
            }
            return Ok(asistencia);
        }

        // POST: api/AsistenciasSociales
        // Este método es para GUARDAR una NUEVA asistencia
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AsistenciaSocial asistencia)
        {
            await _service.AgregarAsync(asistencia);
            // Devuelve un código 201 (Creado exitosamente)
            return CreatedAtAction(nameof(Get), new { id = asistencia.Id }, asistencia);
        }

        // PUT: api/AsistenciasSociales/{un-id-largo}
        // Este método es para ACTUALIZAR una asistencia existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] AsistenciaSocial asistencia)
        {
            if (id != asistencia.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del objeto."); // Error 400
            }

            await _service.ActualizarAsync(asistencia);
            return NoContent(); // Código 204 (Todo bien, nada que devolver)
        }

        // DELETE: api/AsistenciasSociales/{un-id-largo}
        // Este método es para ELIMINAR
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.EliminarAsync(id);
            return NoContent(); // Código 204
        }
    }
}