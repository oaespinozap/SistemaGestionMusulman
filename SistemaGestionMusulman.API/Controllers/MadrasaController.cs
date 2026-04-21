using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;

namespace SistemaGestionMusulman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MadrasaController : ControllerBase
    {
        private readonly IMadrasaService _service;

        public MadrasaController(IMadrasaService service)
        {
            _service = service;
        }

        // 1. GET: Ver todas las clases y sus alumnos
        [HttpGet("clases")]
        public async Task<ActionResult<IEnumerable<ClaseMadrasa>>> ObtenerClases()
        {
            var clases = await _service.ListarClasesAsync();
            return Ok(clases);
        }

        // 2. POST: Crear una nueva materia/clase
        [HttpPost("clases")]
        public async Task<ActionResult<ClaseMadrasa>> CrearClase(ClaseMadrasa clase)
        {
            var nueva = await _service.CrearClaseAsync(clase);
            return Ok(nueva);
        }

        // 3. POST: Inscribir un alumno en una clase
        [HttpPost("inscripciones")]
        public async Task<ActionResult<InscripcionClase>> Matricular(InscripcionClase inscripcion)
        {
            var nueva = await _service.MatricularEstudianteAsync(inscripcion);
            return Ok(nueva);
        }
    }
}