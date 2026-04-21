using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Repositories;

namespace SistemaGestionMusulman.API.Services
{
    public class MadrasaService : IMadrasaService
    {
        // El Gerente tiene línea directa con el Obrero
        private readonly IMadrasaRepository _repository;

        public MadrasaService(IMadrasaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ClaseMadrasa>> ListarClasesAsync()
        {
            return await _repository.ObtenerClasesAsync();
        }

        public async Task<ClaseMadrasa> CrearClaseAsync(ClaseMadrasa clase)
        {
            await _repository.AgregarClaseAsync(clase);
            return clase;
        }

        public async Task<InscripcionClase> MatricularEstudianteAsync(InscripcionClase inscripcion)
        {
            // Nota de Arquitecto: Aquí en el futuro podríamos poner reglas de negocio como:
            // "Verificar que el estudiante no esté ya matriculado en esta clase" o "Cupo máximo alcanzado".

            await _repository.InscribirEstudianteAsync(inscripcion);
            return inscripcion;
        }
    }
}