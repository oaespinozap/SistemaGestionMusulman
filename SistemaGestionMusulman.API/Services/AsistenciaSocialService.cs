using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Repositories;
using SistemaGestionMusulman.Backend.Models;

namespace SistemaGestionMusulman.API.Services
{
    public class AsistenciaSocialService : IAsistenciaSocialService
    {
        private readonly IAsistenciaSocialRepository _repository;

        public AsistenciaSocialService(IAsistenciaSocialRepository repository)
        {
            _repository = repository;
        }

        // ¡Aquí está nuestra ÚNICA versión de ObtenerTodasAsync!
        public async Task<IEnumerable<AsistenciaSocial>> ObtenerTodasAsync()
        {
            // 1. Buscamos los registros en la base de datos
            var asistencias = await _repository.ObtenerTodasAsync();

            // 2. Le armamos el nombre a cada registro
            foreach (var asistencia in asistencias)
            {
                if (asistencia.PerfilMusulman != null)
                {
                    // Unimos Nombre y Apellido
                    asistencia.NombreBeneficiario = $"{asistencia.PerfilMusulman.Nombres} {asistencia.PerfilMusulman.Apellidos}";
                }
                else
                {
                    asistencia.NombreBeneficiario = "Desconocido";
                }
            }

            // 3. Devolvemos la lista lista para usar
            return asistencias;
        }

        public async Task<AsistenciaSocial> ObtenerPorIdAsync(Guid id)
        {
            return await _repository.ObtenerPorIdAsync(id);
        }

        public async Task AgregarAsync(AsistenciaSocial asistencia)
        {
            // Aquí el supervisor puede aplicar reglas antes de guardar. 
            // Por ejemplo, asegurar que se genere un ID nuevo.
            asistencia.Id = Guid.NewGuid();
            await _repository.AgregarAsync(asistencia);
        }

        public async Task ActualizarAsync(AsistenciaSocial asistencia)
        {
            await _repository.ActualizarAsync(asistencia);
        }

        public async Task EliminarAsync(Guid id)
        {
            await _repository.EliminarAsync(id);
        }
    }
}