using System.ComponentModel.DataAnnotations;

namespace SistemaGestionMusulman.API.Models
{
    public class Materia
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Nombre { get; set; } = string.Empty; // Ej: Sira, Creencia, Hadiz

        public bool EsObligatoria { get; set; } // True para Sira/Creencia, False para opcionales

        public int MetaClases { get; set; } // Ej: 10. Si es opcional puede ser 0 (infinita)
    }
}