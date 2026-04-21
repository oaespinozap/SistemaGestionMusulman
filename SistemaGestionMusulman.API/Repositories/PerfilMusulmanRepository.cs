using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    // Aquí el Obrero firma el contrato ( : IPerfilMusulmanRepository )
    public class PerfilMusulmanRepository : IPerfilMusulmanRepository
    {
        // El obrero tiene la llave maestra de la base de datos
        private readonly ApplicationDbContext _context;

        public PerfilMusulmanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PerfilMusulman>> ObtenerTodosAsync()
        {
            return await _context.PerfilesMusulmanes.ToListAsync();
        }

        public async Task<PerfilMusulman> ObtenerPorIdAsync(Guid id)
        {
            return await _context.PerfilesMusulmanes.FindAsync(id);
        }

        public async Task AgregarAsync(PerfilMusulman perfil)
        {
            _context.PerfilesMusulmanes.Add(perfil);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync()
        {
            // Solo guarda, los cambios ya se hicieron en memoria en el Servicio
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(PerfilMusulman perfil)
        {
            _context.PerfilesMusulmanes.Remove(perfil);
            await _context.SaveChangesAsync();
        }
    }
}