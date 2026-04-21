using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionMusulman.API.Models
{
    // Definimos los estados posibles del alumno en la clase
    public enum EstadoInscripcion
    {
        Activo,      // 0
        Completado,  // 1
        Retirado     // 2
    }

    public class InscripcionClase
    {
        [Key]
        public Guid Id { get; set; }

        // --- 1. CONEXIÓN CON EL ESTUDIANTE ---
        [Required]
        public Guid EstudianteId { get; set; }

        [ForeignKey("EstudianteId")]
        public PerfilMusulman? Estudiante { get; set; }

        // --- 2. CONEXIÓN CON LA CLASE ---
        [Required]
        public Guid ClaseId { get; set; }

        [ForeignKey("ClaseId")]
        public ClaseMadrasa? Clase { get; set; }

        // --- 3. DATOS DE LA FICHA ---
        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;

        public EstadoInscripcion Estado { get; set; } = EstadoInscripcion.Activo;
    }
}