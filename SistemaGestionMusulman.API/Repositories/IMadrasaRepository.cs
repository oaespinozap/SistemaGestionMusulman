using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    public interface IMadrasaRepository
    {
        Task<ClaseMadrasa> AbrirClaseAsync(ClaseMadrasa clase);
        Task<IEnumerable<ClaseMadrasa>> ObtenerClasesRecientesAsync();
        Task RegistrarAsistenciaAsync(AsistenciaMadrasa asistencia);
    }
}