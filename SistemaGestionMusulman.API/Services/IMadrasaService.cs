using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Services
{
    public interface IMadrasaService
    {
        Task<ClaseMadrasa> AbrirClaseAsync(ClaseMadrasa clase);
        Task<IEnumerable<ClaseMadrasa>> ObtenerClasesRecientesAsync();
        Task RegistrarAsistenciaAsync(AsistenciaMadrasa asistencia);
    }
}