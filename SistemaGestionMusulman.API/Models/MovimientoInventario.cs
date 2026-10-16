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
        public Guid ArticuloInventarioId { get; set; } // <--- Corregido aquí

        // Relación con el producto en el inventario
        [ForeignKey("ArticuloInventarioId")] // <--- Corregido aquí
        [JsonIgnore] // Para evitar ciclos al hacer consultas
        public ArticuloInventario? ArticuloInventarioAsociado { get; set; } // <--- Corregido aquí

        [Required]
        public string TipoMovimiento { get; set; } = string.Empty; // "Entrada" o "Salida"

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;

        [Required]
        public string Motivo { get; set; } = string.Empty; // Ej: "Donación recibida", "Asistencia Social", "Ajuste", "Vencimiento"

        // (Opcional) Si el movimiento fue por una Asistencia Social, guardamos el ID para tener la trazabilidad exacta a quién se le dio.
        public Guid? AsistenciaSocialId { get; set; }

        // (Opcional) Nombre de la persona que autorizó o realizó el movimiento (para auditoría)
        public string UsuarioResponsable { get; set; } = "Sistema";
    }
}