using System.ComponentModel.DataAnnotations;

namespace SistemaGestionMusulman.API.Models
{
    // Formulario 1: Para cuando alguien es nuevo
    public class RegistroDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }
    }

    // Formulario 2: Para cuando alguien ya tiene cuenta
    public class LoginDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }

       
     // Formulario 3: Para ascender a un usuario
    public class AsignarRolDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; }
        }

    }
}