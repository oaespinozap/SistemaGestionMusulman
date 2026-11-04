using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionMusulman.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SistemaGestionMusulman.API.Models.LoginDto;
using SistemaGestionMusulman.API.Models.DTOs;

namespace SistemaGestionMusulman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDto modelo)
        {
            var user = new IdentityUser { UserName = modelo.Email, Email = modelo.Email };
            var result = await _userManager.CreateAsync(user, modelo.Password);

            if (result.Succeeded)
                return Ok(new { Mensaje = "¡Usuario creado exitosamente!" });

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto modelo)
        {
            var user = await _userManager.FindByEmailAsync(modelo.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, modelo.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiracion = token.ValidTo
                });
            }
            return Unauthorized(new { Mensaje = "Credenciales incorrectas" });
        }

        [HttpPost("crear-roles-iniciales")]
        public async Task<IActionResult> CrearRoles()
        {
            if (!await _roleManager.RoleExistsAsync("AdministradorMaster"))
                await _roleManager.CreateAsync(new IdentityRole("AdministradorMaster"));

            if (!await _roleManager.RoleExistsAsync("TrabajadorSocial"))
                await _roleManager.CreateAsync(new IdentityRole("TrabajadorSocial"));

            if (!await _roleManager.RoleExistsAsync("ProfesorMadrasa"))
                await _roleManager.CreateAsync(new IdentityRole("ProfesorMadrasa"));

            return Ok(new { Mensaje = "Roles creados correctamente en la base de datos." });
        }

        [HttpPost("asignar-rol")]
        public async Task<IActionResult> AsignarRol([FromBody] AsignarRolDto modelo)
        {
            var user = await _userManager.FindByEmailAsync(modelo.Email);
            if (user == null) return NotFound(new { Mensaje = "Usuario no encontrado" });

            if (await _roleManager.RoleExistsAsync(modelo.Rol))
            {
                await _userManager.AddToRoleAsync(user, modelo.Rol);
                return Ok(new { Mensaje = $"Rol {modelo.Rol} asignado correctamente a {modelo.Email}" });
            }

            return BadRequest(new { Mensaje = "El rol especificado no existe." });
        }

        // =================================================================
        // ENDPOINTS PARA EL PANEL DE SUPER ADMINISTRADOR
        // =================================================================

        [HttpGet("usuarios")]
        // [Authorize(Roles = "AdministradorMaster")] // Descomentar después de la primera prueba
        public async Task<IActionResult> ObtenerTodosLosUsuarios()
        {
            var usuarios = _userManager.Users.ToList();
            var listaUsuariosDTO = new List<object>();

            foreach (var user in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(user);
                listaUsuariosDTO.Add(new
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return Ok(listaUsuariosDTO);
        }

        [HttpGet("roles")]
        // [Authorize(Roles = "AdministradorMaster")] // Descomentar después de la primera prueba
        public IActionResult ObtenerTodosLosRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok(roles);
        }

        [HttpPut("actualizar-roles")]
        // [Authorize(Roles = "AdministradorMaster")] // Descomentar después de la primera prueba
        public async Task<IActionResult> ActualizarRolesUsuario([FromBody] ActualizarRolesDto modelo)
        {
            var user = await _userManager.FindByIdAsync(modelo.UserId);
            if (user == null) return NotFound(new { Mensaje = "Usuario no encontrado" });

            var rolesActuales = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, rolesActuales);

            var resultado = await _userManager.AddToRolesAsync(user, modelo.RolesNuevos);

            if (resultado.Succeeded)
                return Ok(new { Mensaje = "Roles actualizados correctamente." });

            return BadRequest(resultado.Errors);
        }

        [HttpPut("restablecer-password")]
        public async Task<IActionResult> RestablecerPassword([FromBody] RestablecerPasswordDto modelo)
        {
            var user = await _userManager.FindByIdAsync(modelo.UserId);
            if (user == null) return NotFound(new { Mensaje = "Usuario no encontrado" });

            // Generamos un token interno para saltarnos la contraseña vieja
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, modelo.NuevaPassword);

            if (result.Succeeded)
                return Ok(new { Mensaje = "Contraseña actualizada correctamente." });

            return BadRequest(result.Errors);

        }


        public class ActualizarRolesDto
        {
            public string UserId { get; set; } = string.Empty;
            public List<string> RolesNuevos { get; set; } = new List<string>();
        }
        public class RestablecerPasswordDto
        {
            public string UserId { get; set; } = string.Empty;
            public string NuevaPassword { get; set; } = string.Empty;
        }
    }
}