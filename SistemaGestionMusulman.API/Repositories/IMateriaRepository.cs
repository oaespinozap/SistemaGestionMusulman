using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    public interface IMateriaRepository
    {
        Task<IEnumerable<Materia>> GetAllAsync();
        Task<Materia?> GetByIdAsync(Guid id);
        Task<Materia> AddAsync(Materia materia);
        Task UpdateAsync(Materia materia);
        Task DeleteAsync(Guid id);
    }
}