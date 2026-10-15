using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    public class MadrasaRepository : IMadrasaRepository
    {
        private readonly ApplicationDbContext _context;

        public MadrasaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClaseMadrasa> AbrirClaseAsync(ClaseMadrasa clase)
        {
            _context.ClasesMadrasa.Add(clase);
            await _context.SaveChangesAsync();
            return clase;
        }

        public async Task<IEnumerable<ClaseMadrasa>> ObtenerClasesRecientesAsync()
        {
            // Traemos las últimas 10 clases para un futuro historial
            return await _context.ClasesMadrasa
                .Include(c => c.Materia)
                .OrderByDescending(c => c.Fecha)
                .Take(10)
                .ToListAsync();
        }

        public async Task RegistrarAsistenciaAsync(AsistenciaMadrasa asistencia)
        {
            _context.AsistenciasMadrasa.Add(asistencia);
            await _context.SaveChangesAsync();
        }
    }
}