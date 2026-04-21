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

        public async Task<IEnumerable<ClaseMadrasa>> ObtenerClasesAsync()
        {
            // Magia pura de Entity Framework: Traemos las clases, 
            // incluimos las fichas de inscripción, y de ahí extraemos al estudiante.
            return await _context.ClasesMadrasa
                                 .Include(c => c.Inscripciones)
                                    .ThenInclude(i => i.Estudiante)
                                 .ToListAsync();
        }

        public async Task AgregarClaseAsync(ClaseMadrasa clase)
        {
            _context.ClasesMadrasa.Add(clase);
            await _context.SaveChangesAsync();
        }

        public async Task InscribirEstudianteAsync(InscripcionClase inscripcion)
        {
            _context.InscripcionesClases.Add(inscripcion);
            await _context.SaveChangesAsync();
        }
    }
}