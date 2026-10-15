using Microsoft.EntityFrameworkCore;
using SistemaGestionMusulman.API.Data;
using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    public class MateriaRepository : IMateriaRepository
    {
        private readonly ApplicationDbContext _context;

        public MateriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Materia>> GetAllAsync()
        {
            return await _context.Materias.ToListAsync();
        }

        public async Task<Materia?> GetByIdAsync(Guid id)
        {
            return await _context.Materias.FindAsync(id);
        }

        public async Task<Materia> AddAsync(Materia materia)
        {
            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();
            return materia;
        }

        public async Task UpdateAsync(Materia materia)
        {
            _context.Entry(materia).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var materia = await _context.Materias.FindAsync(id);
            if (materia != null)
            {
                _context.Materias.Remove(materia);
                await _context.SaveChangesAsync();
            }
        }
    }
}