using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionMusulman.API.Models
{
    public class PerfilMusulman
    {
        [Key] // Esto le dice a la base de datos que este es el ID principal
        public Guid Id { get; set; }

        [Required]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        public string NumeroCedula { get; set; } = string.Empty;

        public DateTime FechaNacimiento { get; set; }

        public DateTime FechaShajada { get; set; }

        public string EstadoCivil { get; set; } = string.Empty;

        public string DireccionVivienda { get; set; } = string.Empty;

        public string NumeroTelefono { get; set; } = string.Empty;

        // Por ahora mantendremos el modelo MVP simple para nuestra primera prueba de conexión
    }
}