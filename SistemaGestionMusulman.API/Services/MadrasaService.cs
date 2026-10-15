using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Repositories;

namespace SistemaGestionMusulman.API.Services
{
    public class MadrasaService : IMadrasaService
    {
        private readonly IMadrasaRepository _repository;

        public MadrasaService(IMadrasaRepository repository)
        {
            _repository = repository;
        }

        public async Task<ClaseMadrasa> AbrirClaseAsync(ClaseMadrasa clase) => await _repository.AbrirClaseAsync(clase);

        public async Task<IEnumerable<ClaseMadrasa>> ObtenerClasesRecientesAsync() => await _repository.ObtenerClasesRecientesAsync();

        public async Task RegistrarAsistenciaAsync(AsistenciaMadrasa asistencia) => await _repository.RegistrarAsistenciaAsync(asistencia);
    }
}