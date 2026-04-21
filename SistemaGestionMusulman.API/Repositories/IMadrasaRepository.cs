using SistemaGestionMusulman.API.Models;

namespace SistemaGestionMusulman.API.Repositories
{
    public interface IMadrasaRepository
    {
        // 1. Ver todas las clases
        Task<IEnumerable<ClaseMadrasa>> ObtenerClasesAsync();

        // 2. Crear una nueva clase (Ej: "Árabe Básico")
        Task AgregarClaseAsync(ClaseMadrasa clase);

        // 3. Guardar la ficha de un alumno en una clase
        Task InscribirEstudianteAsync(InscripcionClase inscripcion);
    }
}