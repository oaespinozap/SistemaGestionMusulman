using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // El recepcionista pide la llave del puente (context) para poder trabajar
        public PerfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. OÍDOS (GET): Cuando internet pregunte "Dame todos los perfiles"
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerfilMusulman>>> ObtenerTodosLosPerfiles()
        {
            return await _context.PerfilesMusulmanes.ToListAsync();
        }

        // 2. BOCA (POST): Cuando internet diga "Toma, guarda este nuevo perfil"
        [HttpPost]
        public async Task<ActionResult<PerfilMusulman>> RegistrarNuevoPerfil(PerfilMusulman nuevoPerfil)
        {
            // Le decimos al puente que agregue el nuevo registro
            _context.PerfilesMusulmanes.Add(nuevoPerfil);

            // Guardamos los cambios en PostgreSQL
            await _context.SaveChangesAsync();

            // Respondemos que todo salió bien y mostramos lo que se guardó
            return Ok(nuevoPerfil);
        }
    }
}