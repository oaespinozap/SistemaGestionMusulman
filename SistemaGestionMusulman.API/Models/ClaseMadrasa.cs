using System.ComponentModel.DataAnnotations;

namespace SistemaGestionMusulman.API.Models
{
    public class ClaseMadrasa
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreMateria { get; set; } // Ej: "Árabe Básico", "Tajwid"

        public string? Profesor { get; set; } // Nombre del maestro o Shaij

        public string? Horario { get; set; } // Ej: "Sábados 9:00 AM"

        // RELACIÓN: Una clase va a tener MUCHAS fichas de inscripción
        public ICollection<InscripcionClase> Inscripciones { get; set; } = new List<InscripcionClase>();
    }
}