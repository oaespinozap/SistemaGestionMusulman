using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Repositories;

namespace SistemaGestionMusulman.API.Services
{
    public class SadaqahService : ISadaqahService
    {
        // El Gerente necesita el teléfono del Obrero (Repositorio) para darle órdenes
        private readonly ISadaqahRepository _repository;

        public SadaqahService(ISadaqahRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DonacionSadaqah>> ListarInventarioAsync()
        {
            // Le pide al obrero que traiga todo lo que hay en la bodega
            return await _repository.ObtenerTodasAsync();
        }

        public async Task<DonacionSadaqah> RegistrarEntradaAsync(DonacionSadaqah donacion)
        {
            // Le ordena al obrero que guarde la nueva donación
            await _repository.AgregarAsync(donacion);
            return donacion;
        }
    }
}