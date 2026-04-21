using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Services
{
    public interface ISadaqahService
    {
        // El gerente ofrece dos servicios: ver el inventario y registrar una nueva entrada
        Task<IEnumerable<DonacionSadaqah>> ListarInventarioAsync();
        Task<DonacionSadaqah> RegistrarEntradaAsync(DonacionSadaqah donacion);
    }
}
