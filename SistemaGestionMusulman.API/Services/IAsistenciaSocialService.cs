using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.Backend.Models;

namespace SistemaGestionMusulman.API.Services
{
    public interface IAsistenciaSocialService
    {
        Task<IEnumerable<AsistenciaSocial>> ObtenerTodasAsync();
        Task<AsistenciaSocial> ObtenerPorIdAsync(Guid id);
        Task AgregarAsync(AsistenciaSocial asistencia);
        Task ActualizarAsync(AsistenciaSocial asistencia);
        Task EliminarAsync(Guid id);
    }
}