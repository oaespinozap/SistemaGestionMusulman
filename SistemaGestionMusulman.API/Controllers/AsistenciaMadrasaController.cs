using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ¡NUEVO! Necesario para ToListAsync()
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;

namespace SistemaGestionMusulman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciaMadrasaController : ControllerBase
    {
        private readonly IMadrasaService _madrasaService;
        private readonly ApplicationDbContext _context; // ¡NUEVO! Lo declaramos a nivel de clase

        // ¡NUEVO! Modificamos el constructor para inyectar la base de datos (ApplicationDbContext)
        public AsistenciaMadrasaController(IMadrasaService madrasaService, ApplicationDbContext context)
        {
            _madrasaService = madrasaService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(AsistenciaMadrasa asistencia)
        {
            asistencia.Id = Guid.NewGuid();
            await _madrasaService.RegistrarAsistenciaAsync(asistencia);
            return Ok(); // Respondemos 200 OK
        }

        [HttpGet("progreso/{perfilId}")]
        public async Task<ActionResult> GetProgresoEstudiante(Guid perfilId)
        {
            // Ahora sí usamos el _context que vino desde el constructor
            var progreso = await _context.Materias
                .Select(m => new
                {
                    MateriaNombre = m.Nombre,
                    EsObligatoria = m.EsObligatoria,
                    MetaClases = m.MetaClases,
                    ClasesVistas = _context.AsistenciasMadrasa
                                    .Count(a => a.PerfilMusulmanId == perfilId && a.Clase.MateriaId == m.Id)
                })
                .ToListAsync();

            return Ok(progreso);
        }
    }
}