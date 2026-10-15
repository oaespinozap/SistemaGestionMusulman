using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;

namespace SistemaGestionMusulman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaseMadrasaController : ControllerBase
    {
        private readonly IMadrasaService _madrasaService;

        public ClaseMadrasaController(IMadrasaService madrasaService)
        {
            _madrasaService = madrasaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClaseMadrasa>>> Get()
        {
            return Ok(await _madrasaService.ObtenerClasesRecientesAsync());
        }

        [HttpPost]
        public async Task<ActionResult<ClaseMadrasa>> Post(ClaseMadrasa clase)
        {
            clase.Id = Guid.NewGuid();
            clase.Fecha = DateTime.UtcNow; // Ponemos la estampa de tiempo real

            var nuevaClase = await _madrasaService.AbrirClaseAsync(clase);
            return Ok(nuevaClase);
        }
    }
}