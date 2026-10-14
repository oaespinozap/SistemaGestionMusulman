using SistemaGestionMusulman.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionMusulman.Backend.Models
{
    public class AsistenciaSocial
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Conexión directa con el hermano/hermana que recibe la ayuda
        [Required]
        public Guid PerfilMusulmanId { get; set; }
        [NotMapped]
        public string? NombreBeneficiario { get; set; }

        [ForeignKey("PerfilMusulmanId")]
        public PerfilMusulman? PerfilMusulman { get; set; }

        // Las 8 Categorías del Zakat
        [Required]
        public string CategoriaZakat { get; set; } = string.Empty;

        // Salud, Alimentación, Vestimenta, etc.
        [Required]
        public string TipoDeAyuda { get; set; } = string.Empty;

        [Required]
        public DateTime FechaEntrega { get; set; } = DateTime.UtcNow;

        // El signo de interrogación '?' significa que es Opcional
        public decimal? MontoEstimado { get; set; }

        // Detalles adicionales
        public string? Observaciones { get; set; }

        // Conexión opcional con el módulo de Inventario
        public Guid? ArticuloInventarioId { get; set; }

        // Cantidad que se va a restar del almacén
        public int? CantidadExtraida { get; set; }


    }
}