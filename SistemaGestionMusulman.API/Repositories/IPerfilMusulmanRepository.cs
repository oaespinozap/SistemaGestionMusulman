using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    // La "I" inicial indica que esto es un Contrato (Interfaz)
    // Solo define QUÉ debe saber hacer el obrero, no CÓMO lo hace.
    public interface IPerfilMusulmanRepository
    {
        Task<IEnumerable<PerfilMusulman>> ObtenerTodosAsync();
        Task<PerfilMusulman> ObtenerPorIdAsync(Guid id);
        Task AgregarAsync(PerfilMusulman perfil);
        Task ActualizarAsync();
        Task EliminarAsync(PerfilMusulman perfil);
    }
}