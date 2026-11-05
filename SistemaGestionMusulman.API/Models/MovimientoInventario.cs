using SistemaGestionMusulman.Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SistemaGestionMusulman.API.Models
{
    public class MovimientoInventario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ArticuloInventarioId { get; set; }

        [ForeignKey("ArticuloInventarioId")]
        [JsonIgnore]
        public ArticuloInventario? ArticuloInventarioAsociado { get; set; }

        [Required]
        public string TipoMovimiento { get; set; } = string.Empty; // "Entrada", "Salida", "Baja"

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;

        [Required]
        public string Motivo { get; set; } = string.Empty;

        // <-- NUEVO: Para saber a quién se le entregó el artículo en una Salida -->
        [StringLength(150)]
        public string? Destinatario { get; set; }

        public Guid? AsistenciaSocialId { get; set; }

        // Lo dejaremos por defecto en "Sistema", pero nuestro Controlador ahora lo llenará con el JWT real
        public string UsuarioResponsable { get; set; } = "Sistema";
    }
}