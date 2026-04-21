using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    public interface ISadaqahRepository
    {
        Task<IEnumerable<DonacionSadaqah>> ObtenerTodasAsync();
        Task AgregarAsync(DonacionSadaqah donacion);
    }
}