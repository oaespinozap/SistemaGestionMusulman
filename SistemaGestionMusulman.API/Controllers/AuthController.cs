using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionMusulman.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SistemaGestionMusulman.API.Models.LoginDto;

namespace SistemaGestionMusulman.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // <-- NUEVO: Gestor de Roles
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
                // 1. Buscamos qué roles tiene este usuario en la base de datos
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                // 2. ¡NUEVO! Escribimos los roles en la Pulsera VIP
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

        // --- 3. NUEVO: Crear los rangos oficiales de la Mezquita ---
        [HttpPost("crear-roles-iniciales")]
        public async Task<IActionResult> CrearRoles()
        {
            if (!await _roleManager.RoleExistsAsync("Administrador"))
                await _roleManager.CreateAsync(new IdentityRole("Administrador"));

            if (!await _roleManager.RoleExistsAsync("Estudiante"))
                await _roleManager.CreateAsync(new IdentityRole("Estudiante"));

            return Ok(new { Mensaje = "Roles creados correctamente en la base de datos." });
        }

        // --- 4. NUEVO: Asignarle un rango a un usuario ---
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
    }
}