using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Services
{
    public interface IMadrasaService
    {
        // Los 3 servicios que ofrece la Madrasa
        Task<IEnumerable<ClaseMadrasa>> ListarClasesAsync();
        Task<ClaseMadrasa> CrearClaseAsync(ClaseMadrasa clase);
        Task<InscripcionClase> MatricularEstudianteAsync(InscripcionClase inscripcion);
    }
}