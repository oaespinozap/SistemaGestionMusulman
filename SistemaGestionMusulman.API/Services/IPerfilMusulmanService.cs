using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Services
{
    // El contrato del Gerente. Define los servicios que ofrece.
    public interface IPerfilMusulmanService
    {
        Task<IEnumerable<PerfilMusulman>> ObtenerTodosAsync();
        Task<PerfilMusulman> ObtenerPorIdAsync(Guid id);
        Task<PerfilMusulman> CrearPerfilAsync(PerfilMusulman perfil);
        Task<bool> ActualizarPerfilAsync(Guid id, PerfilMusulman perfilActualizado);
        Task<bool> EliminarPerfilAsync(Guid id);
    }
}