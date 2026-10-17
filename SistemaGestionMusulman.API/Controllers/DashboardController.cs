using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Models.DTOs;

namespace SistemaGestionMusulman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🛡️ Protegemos la ruta para que solo usuarios logueados puedan ver los totales
    public class DashboardController : ControllerBase
    {
        // 👇 1. Corregimos el nombre a ApplicationDbContext
        private readonly ApplicationDbContext _context;

        // 👇 2. Corregimos el constructor
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("resumen")]
        public async Task<ActionResult<DashboardSummaryDTO>> GetResumen()
        {
            try
            {
                // 👇 3. Usamos los nombres EXACTOS de tus DbSet
                var totalPerfiles = await _context.PerfilesMusulmanes.CountAsync();
                var totalKardex = await _context.Inventario.CountAsync();
                var totalAsistencias = await _context.AsistenciasSociales.CountAsync();
                var totalMaterias = await _context.Materias.CountAsync();

                var resumen = new DashboardSummaryDTO
                {
                    TotalPerfiles = totalPerfiles,
                    TotalItemsKardex = totalKardex,
                    TotalAsistencias = totalAsistencias,
                    TotalMaterias = totalMaterias
                };

                return Ok(resumen);
            }
            catch (Exception ex)
            {
                // En caso de error, devolvemos un 500
                return StatusCode(500, $"Error interno al calcular el resumen: {ex.Message}");
            }
        }
    }
}