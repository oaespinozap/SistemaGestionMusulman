using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionMusulman.API.Models
{
    public class AsistenciaMadrasa
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ClaseMadrasaId { get; set; }

        [ForeignKey("ClaseMadrasaId")]
        public ClaseMadrasa? Clase { get; set; }

        [Required]
        public Guid PerfilMusulmanId { get; set; }

        // Enlazamos directamente con el perfil que ya existe
        [ForeignKey("PerfilMusulmanId")]
        public PerfilMusulman? Perfil { get; set; }
    }
}