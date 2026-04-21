using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Repositories;

namespace SistemaGestionMusulman.API.Services
{
    public class PerfilMusulmanService : IPerfilMusulmanService
    {
        // El Gerente necesita comunicarse con el Obrero (Repositorio)
        private readonly IPerfilMusulmanRepository _repository;

        public PerfilMusulmanService(IPerfilMusulmanRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PerfilMusulman>> ObtenerTodosAsync()
        {
            return await _repository.ObtenerTodosAsync();
        }

        public async Task<PerfilMusulman> ObtenerPorIdAsync(Guid id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        public async Task<PerfilMusulman> CrearPerfilAsync(PerfilMusulman perfil)
        {
            // Aquí podríamos agregar reglas de negocio en el futuro (ej. validar cédula)
            await _repository.AgregarAsync(perfil);
            return perfil;
        }

        public async Task<bool> ActualizarPerfilAsync(Guid id, PerfilMusulman perfilActualizado)
        {
            if (id != perfilActualizado.Id) return false;

            var perfilExistente = await _repository.ObtenerPorIdAsync(id);
            if (perfilExistente == null) return false;

            // El Gerente actualiza los datos en memoria
            perfilExistente.Nombres = perfilActualizado.Nombres;
            perfilExistente.Apellidos = perfilActualizado.Apellidos;
            perfilExistente.NumeroCedula = perfilActualizado.NumeroCedula;
            perfilExistente.FechaNacimiento = perfilActualizado.FechaNacimiento;
            perfilExistente.FechaShajada = perfilActualizado.FechaShajada;
            perfilExistente.EstadoCivil = perfilActualizado.EstadoCivil;
            perfilExistente.DireccionVivienda = perfilActualizado.DireccionVivienda;
            perfilExistente.NumeroTelefono = perfilActualizado.NumeroTelefono;

            // Le dice al obrero que guarde los cambios físicos
            await _repository.ActualizarAsync();
            return true;
        }

        public async Task<bool> EliminarPerfilAsync(Guid id)
        {
            var perfilExistente = await _repository.ObtenerPorIdAsync(id);
            if (perfilExistente == null) return false;

            await _repository.EliminarAsync(perfilExistente);
            return true;
        }
    }
}