using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionMusulman.API.Models
{
    public enum CategoriaSadaqah
    {
        InsumosMedicos, // Sillas de ruedas, muletas
        Alimentos,      // Proteínas, víveres
        Textil,         // Ropa, calzado
        Educacion,      // Cuadernos, guías
        Otros
    }

    public class DonacionSadaqah
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public CategoriaSadaqah Categoria { get; set; }

        [Required]
        [StringLength(200)]
        public string Item { get; set; }

        [Required]
        public decimal Cantidad { get; set; }

        public string UnidadMedida { get; set; } = "Unidades";

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Relación con quien RECIBE (opcional)
        public Guid? BeneficiarioId { get; set; }

        [ForeignKey("BeneficiarioId")]
        public PerfilMusulman? Beneficiario { get; set; }

        public string? Observaciones { get; set; }
    }
}