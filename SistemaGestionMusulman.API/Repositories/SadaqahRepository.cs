using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    public class SadaqahRepository : ISadaqahRepository
    {
        private readonly ApplicationDbContext _context;

        public SadaqahRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonacionSadaqah>> ObtenerTodasAsync()
        {
            // El .Include(d => d.Beneficiario) hace magia: trae la donación y ADEMÁS
            // trae los datos del musulmán que la recibió (si es que alguien la recibió).
            return await _context.DonacionesSadaqah
                                 .Include(d => d.Beneficiario)
                                 .ToListAsync();
        }

        public async Task AgregarAsync(DonacionSadaqah donacion)
        {
            _context.DonacionesSadaqah.Add(donacion);
            await _context.SaveChangesAsync();
        }
    }
}