using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionMusulman.API.Models;
using SistemaGestionMusulman.API.Services;

namespace SistemaGestionMusulman.API.Controllers
{
    [Authorize(Roles = "Administrador")] // <--- Solo el jefe entra aquí
    [Route("api/[controller]")]
    [ApiController]

    public class SadaqahController : ControllerBase
    {
        // El Recepcionista solo se comunica con el Gerente de Sadaqah
        private readonly ISadaqahService _service;

        public SadaqahController(ISadaqahService service)
        {
            _service = service;
        }

        // GET: Ver todo el inventario de donaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonacionSadaqah>>> ObtenerInventario()
        {
            var inventario = await _service.ListarInventarioAsync();
            return Ok(inventario); // Devuelve código 200 con la lista
        }

        // POST: Ingresar una nueva donación a la bodega
        [HttpPost]
        public async Task<ActionResult<DonacionSadaqah>> RegistrarDonacion(DonacionSadaqah donacion)
        {
            var nuevaDonacion = await _service.RegistrarEntradaAsync(donacion);
            return Ok(nuevaDonacion); // Devuelve código 200 con el item registrado
        }
    }
}