using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;

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
    }
}