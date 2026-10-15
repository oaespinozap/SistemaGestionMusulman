using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Repositories;

namespace SistemaGestionMusulman.API.Services
{
    public class MateriaService : IMateriaService
    {
        private readonly IMateriaRepository _repository;

        public MateriaService(IMateriaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Materia>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<Materia?> GetByIdAsync(Guid id) => await _repository.GetByIdAsync(id);

        public async Task<Materia> AddAsync(Materia materia) => await _repository.AddAsync(materia);

        public async Task UpdateAsync(Materia materia) => await _repository.UpdateAsync(materia);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);
    }
}