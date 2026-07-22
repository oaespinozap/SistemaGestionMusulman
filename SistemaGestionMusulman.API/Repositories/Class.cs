using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.Backend.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    public class AsistenciaSocialRepository : IAsistenciaSocialRepository
    {
        private readonly ApplicationDbContext _context;

        public AsistenciaSocialRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AsistenciaSocial>> ObtenerTodasAsync()
        {
            // Magia pura: Traemos la asistencia y de una vez los datos del beneficiario
            return await _context.AsistenciasSociales
                                 .Include(a => a.PerfilMusulman)
                                 .ToListAsync();
        }

        public async Task<AsistenciaSocial> ObtenerPorIdAsync(Guid id)
        {
            return await _context.AsistenciasSociales
                                 .Include(a => a.PerfilMusulman)
                                 .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AgregarAsync(AsistenciaSocial asistencia)
        {
            _context.AsistenciasSociales.Add(asistencia);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(AsistenciaSocial asistencia)
        {
            _context.AsistenciasSociales.Update(asistencia);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(Guid id)
        {
            var asistencia = await _context.AsistenciasSociales.FindAsync(id);
            if (asistencia != null)
            {
                _context.AsistenciasSociales.Remove(asistencia);
                await _context.SaveChangesAsync();
            }
        }
    }
}