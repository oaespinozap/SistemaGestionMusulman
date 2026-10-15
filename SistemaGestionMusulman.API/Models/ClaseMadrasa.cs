using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionMusulman.API.Models
{
    public class ClaseMadrasa
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid MateriaId { get; set; }

        [ForeignKey("MateriaId")]
        public Materia? Materia { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public string TemaDictado { get; set; } = string.Empty; // Pequeño resumen del día

        public bool EsMasterClass { get; set; } // El requerimiento VIP que pediste
    }
}