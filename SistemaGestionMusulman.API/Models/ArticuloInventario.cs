using System.ComponentModel.DataAnnotations;

namespace SistemaGestionMusulman.Backend.Models
{
    public class ArticuloInventario
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre del artículo es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty; // Ej: Alimentos, Medicamentos, Ropa, Equipos

        [Required]
        public int CantidadDisponible { get; set; } = 0;

        public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;

        // Permite valores nulos (?) para cosas que no caducan, como la ropa
        public DateTime? FechaCaducidad { get; set; }

        [StringLength(200)]
        public string? Observaciones { get; set; }
    }
}